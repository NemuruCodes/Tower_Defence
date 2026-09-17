using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TowerSelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask towerMask;
    [SerializeField] private TowerPlacementManager placementManager;




    private Tower selectedTower;
    private Outline hoveredOutline;

    private void Update()
    {
        if (placementManager != null && placementManager.IsPlacing)
        {
            ClearHover();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitTower = Physics.Raycast(ray, out RaycastHit hit, 100f, towerMask);
        Tower hoveredTower = hitTower ? hit.collider.GetComponentInParent<Tower>() : null;
        

        UpdateHover(hoveredTower);

        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredTower != null)
                SelectTower(hoveredTower);
            else
                DeselectCurrent();
        }
    }

    private void UpdateHover(Tower hoveredTower)
    {
        Outline outline = hoveredTower != null ? hoveredTower.GetComponentInChildren<Outline>() : null;
        
        if (outline == hoveredOutline) return;

        ClearHover();
        hoveredOutline = outline;
        if (hoveredOutline != null) hoveredOutline.enabled = true;

        

    }

    private void ClearHover()
    {
        if (hoveredOutline != null) 
        { 
            hoveredOutline.enabled = false;
        }

        hoveredOutline = null;
    }

    private void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            HealthBar oldHealth = selectedTower.GetComponentInChildren<HealthBar>(true);
            if (oldHealth != null) oldHealth.gameObject.SetActive(false);
            selectedTower?.Deselect();
        }

        selectedTower = tower;
        selectedTower.Select();

        HealthBar newHealth = selectedTower.GetComponentInChildren<HealthBar>(true);
        if (newHealth != null) 
        { 
            newHealth.gameObject.SetActive(true);
            
        }

    }

    private void DeselectCurrent()
    {
        if (selectedTower == null) return;

        if (selectedTower.CompareTag("MainTower"))
        {
            selectedTower?.Deselect();
            selectedTower = null;
        }
        else
        {
            HealthBar health = selectedTower.GetComponentInChildren<HealthBar>(true);
            if (health != null) health.gameObject.SetActive(false);

            selectedTower?.Deselect();
            selectedTower = null;
        }
    }
}
