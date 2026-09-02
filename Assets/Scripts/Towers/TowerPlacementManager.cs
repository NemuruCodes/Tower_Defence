using System.Resources;
using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private TowerSelectionManager selectionManager; // to disable during placement

    [SerializeField] private HudEvents hudEvents;

    [SerializeField] private Material ghostMaterial;

    private GameObject ghostInstance;
    private RangeIndicator ghostRangeIndicator;
    private TowerData pendingTowerData;
    private float ghostPivotOffset;

    public bool IsPlacing => ghostInstance != null;

    // Called by your UI when the player picks a tower type to place
    public void BeginPlacement(TowerData towerData)
    {
        if (IsPlacing) CancelPlacement();

        pendingTowerData = towerData;
        ghostInstance = Instantiate(towerData.towerPrefab);
        SetGhostVisuals(ghostInstance); // disable colliders / real Tower script, tint transparent

        ghostPivotOffset = CalculatePivotOffset(ghostInstance);

        ghostRangeIndicator = ghostInstance.GetComponentInChildren<RangeIndicator>();
        ghostRangeIndicator.SetRadius(towerData.range);
        ghostRangeIndicator.Show();

        if (selectionManager != null) selectionManager.enabled = false;
    }

    private float CalculatePivotOffset(GameObject instance)
    {
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;

        Bounds combined = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            combined.Encapsulate(renderers[i].bounds);

        // distance from the instance's pivot down to the lowest point of its mesh
        return instance.transform.position.y - combined.min.y;
    }

    private void Update()
    {
        if (!IsPlacing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitGround = Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask);

        if (hitGround)
        {
            Vector3 pos = hit.point;
            pos.y += ghostPivotOffset;
            ghostInstance.transform.position = pos;
        }

        if (Input.GetMouseButtonDown(0) && hitGround)
        {
            Vector3 placePos = hit.point;
            placePos.y += ghostPivotOffset;
            TryConfirmPlacement(placePos);
        }
        else if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    private void TryConfirmPlacement(Vector3 position)
    {
        if (!ResourceManager.Instance.CanAfford(pendingTowerData.buildCost)) return;

        ResourceManager.Instance.Spend(pendingTowerData.buildCost);
        Instantiate(pendingTowerData.towerPrefab, position, Quaternion.identity);
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        ghostInstance = null;
        pendingTowerData = null;

        if (selectionManager != null) selectionManager.enabled = true;

        hudEvents.OnPlacementEnd();
        
    }

    private void SetGhostVisuals(GameObject ghost)
    {
        // disable the real Tower logic so the ghost doesn't attack/track health
        if (ghost.TryGetComponent(out Tower towerScript)) towerScript.enabled = false;

        // disable colliders so the ghost doesn't block its own raycast or get "selected"
        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (ghostMaterial != null)
        {
            foreach (var renderer in ghost.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = ghostMaterial;
                renderer.materials = mats;
            }
        }
    }
}
