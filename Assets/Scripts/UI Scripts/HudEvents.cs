using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class HudEvents : MonoBehaviour
{
    [SerializeField] private TowerData[] availableTowers;
    [SerializeField] private TowerPlacementManager placementManager;

    private PanelRenderer panelRenderer;
    private VisualElement panel;
    private VisualElement listContainer;
    private VisualElement placingContainer;
    private Button toggleButton;
    private bool isOpen = false;
    private bool isPlacing = false;

    private void Awake()
    {
        panelRenderer = GetComponent<PanelRenderer>();
    }

    private void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        panel = root.Q<VisualElement>("Container");
        listContainer = root.Q<VisualElement>("TowerMenu");
        toggleButton = root.Q<Button>("ToggleButton");
        placingContainer = root.Q<VisualElement>("PlaceOrCancel");

        toggleButton.clicked += ToggleMenu;

        //PopulateTowerList();
    }

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        listContainer.AddToClassList(isOpen ? "TowerMenuOpen" : "TowerMenuClosed");
        listContainer.RemoveFromClassList(isOpen ? "TowerMenuClosed" : "TowerMenuOpen");
        

       
    }

    private void TogglePrompt()
    {
        isPlacing = !isPlacing;
        placingContainer.AddToClassList(isPlacing ? "Placing" : "NotPlacing");
        placingContainer.RemoveFromClassList(isPlacing ? "NotPlacing" : "Placing");
    }

    private void PopulateTowerList()
    {
        listContainer.Clear();

        foreach (var towerData in availableTowers)
        {
            var entry = new VisualElement();
            entry.AddToClassList("tower-entry");

            entry.Add(new Label(towerData.towerName));
            entry.Add(new Label($"Cost: {towerData.buildCost}"));
            entry.Add(new Label($"DMG: {towerData.damage}  RNG: {towerData.range}"));

            entry.RegisterCallback<ClickEvent>(evt => OnTowerSelected(towerData));

            listContainer.Add(entry);
        }
    }

    private void OnTowerSelected(TowerData towerData)
    {
        placementManager.BeginPlacement(towerData);

        TogglePrompt();
        ToggleMenu();
    }
}
