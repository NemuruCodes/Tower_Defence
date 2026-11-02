using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class SellScript : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private TowerStats stats;
    private Tower tower;

    private void Awake()
    {
        tower = GetComponentInParent<Tower>(true);
        stats = GetComponentInParent<TowerStats>(true);

        button.onClick.AddListener(tower.Sell);
    }

    private void OnEnable()
    {
        // The main tower can't be sold, so hide the button entirely
        if (!tower.CanSell)
        {
            gameObject.SetActive(false);
            return;
        }

        stats.OnStatChanged += HandleStatChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(TowerStat stat, float oldValue, float newValue)
    {
        if (stat == TowerStat.SellAmount) Refresh();
    }

    private void Refresh()
    {
        label.text = $"Sell +{stats.SellAmount}";
    }
}
