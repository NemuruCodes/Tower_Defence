using UnityEngine;
using UnityEngine.InputSystem;

public class TowerSelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask towerMask;
    private Tower selectedTower;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, towerMask))
        {
            Tower tower = hit.collider.GetComponentInParent<Tower>();
            if (tower != null)
            {
                selectedTower?.Deselect();
                selectedTower = tower;
                selectedTower.Select();
                return;
            }
        }

        // clicked empty space / non-tower — deselect
        selectedTower?.Deselect();
        selectedTower = null;
    }
}
