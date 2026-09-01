using System.Resources;
using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private TowerSelectionManager selectionManager; // to disable during placement

    private GameObject ghostInstance;
    private RangeIndicator ghostRangeIndicator;
    private TowerData pendingTowerData;

    public bool IsPlacing => ghostInstance != null;

    // Called by your UI when the player picks a tower type to place
    public void BeginPlacement(TowerData towerData)
    {
        if (IsPlacing) CancelPlacement();

        pendingTowerData = towerData;
        ghostInstance = Instantiate(towerData.towerPrefab);
        SetGhostVisuals(ghostInstance); // disable colliders / real Tower script, tint transparent

        ghostRangeIndicator = ghostInstance.GetComponentInChildren<RangeIndicator>();
        ghostRangeIndicator.SetRadius(towerData.range);
        ghostRangeIndicator.Show();

        if (selectionManager != null) selectionManager.enabled = false;
    }

    private void Update()
    {
        if (!IsPlacing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitGround = Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask);
        if (hitGround)
            ghostInstance.transform.position = hit.point;

        if (Input.GetMouseButtonDown(0) && hitGround)
            TryConfirmPlacement(hit.point);
        else if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    private void TryConfirmPlacement(Vector3 position)
    {
       // if (!ResourceManager.Instance.CanAfford(pendingTowerData.cost)) return;

        //ResourceManager.Instance.Spend(pendingTowerData.cost);
        Instantiate(pendingTowerData.towerPrefab, position, Quaternion.identity);
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        ghostInstance = null;
        pendingTowerData = null;

        if (selectionManager != null) selectionManager.enabled = true;
    }

    private void SetGhostVisuals(GameObject ghost)
    {
        // disable the real Tower logic so the ghost doesn't attack/track health
        if (ghost.TryGetComponent(out Tower towerScript)) towerScript.enabled = false;

        // disable colliders so the ghost doesn't block its own raycast or get "selected"
        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = false;

        
    }
}
