using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradePathSO path;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private TowerUpgrader upgrader;

    private void Awake()
    {
        upgrader = GetComponentInParent<TowerUpgrader>();
        button.onClick.AddListener(() => upgrader.TryUpgrade(path));
    }

    private void OnEnable()
    {
        upgrader.OnUpgraded += HandleUpgraded;
        Refresh();   // the panel is toggled with SetActive, so refresh on show
    }

    private void OnDisable() => upgrader.OnUpgraded -= HandleUpgraded;

    private void HandleUpgraded(UpgradePathSO p, int tier) { if (p == path) Refresh(); }

    private void Refresh()
    {


        bool available = upgrader.Paths.Contains(path);   
        gameObject.SetActive(available);
        if (!available) return;

        UpgradeSO next = upgrader.GetNext(path);
        button.interactable = next != null;
        label.text = next != null ? $"{path.displayName}\n{next.cost}" : $"{path.displayName}\nMAX";
    }
}
