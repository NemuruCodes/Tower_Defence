using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class HudEvents : MonoBehaviour
{
    [System.Serializable]
    private struct TowerButtonBinding
    {
        public string buttonName;
        public TowerData towerData;
    }

    [SerializeField] private TowerButtonBinding[] towerButtons;
    [SerializeField] private TowerPlacementManager placementManager;

    private PanelRenderer panelRenderer;
    private VisualElement panel;
    private VisualElement listContainer;
    private VisualElement placingContainer;
   

    private Button toggleButton;

    private Button singleAttackTower;

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

        singleAttackTower = root.Q<Button>("SingleTarget");

        toggleButton.clicked += ToggleMenu;

        BindTowerButtons(root);
    }

    private void BindTowerButtons(VisualElement root)
    {
        foreach (var binding in towerButtons)
        {
            Button button = root.Q<Button>(binding.buttonName);
            if (button == null)
            {
                Debug.LogWarning($"HudEvents: no button named '{binding.buttonName}' found in UXML.");
                continue;
            }

            //Debug.Log(button);

            TowerData data = binding.towerData; // local copy for the closure
            button.clicked += () => OnTowerSelected(data);
        }
    }


    private void ToggleMenu()
    {
        isOpen = !isOpen;
        listContainer.AddToClassList(isOpen ? "TowerMenuOpen" : "TowerMenuClosed");
        listContainer.RemoveFromClassList(isOpen ? "TowerMenuClosed" : "TowerMenuOpen");
   
    }

    /*
    private void TogglePrompt()
    {
        //Debug.Log("Toggle Placement Prompt On");

        isPlacing = !isPlacing;
        placingContainer.AddToClassList(isPlacing ? "Placing" : "NotPlacing");
        placingContainer.RemoveFromClassList(isPlacing ? "NotPlacing" : "Placing");
    }
    */
    private void ShowPrompt()
    {
        if (isPlacing) return;
        isPlacing = true;
        placingContainer.AddToClassList("Placing");
        placingContainer.RemoveFromClassList("NotPlacing");
    }

    private void HidePrompt()
    {
        if (!isPlacing) return;
        isPlacing = false;
        placingContainer.AddToClassList("NotPlacing");
        //placingContainer.RemoveFromClassList("Placing");
    }

    private void OnTowerSelected(TowerData towerData)
    {
        placementManager.BeginPlacement(towerData);


        ShowPrompt();
        ToggleMenu();
    }

    public void OnPlacementEnd()
    {
        HidePrompt();
    }

    
}
