using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private int startingResources = 100;

    public int CurrentResources { get; private set; }

    public event Action<int> OnResourcesChanged;

    private Label resourceLabel;
    private PanelRenderer panelRenderer;

    private void Awake()
    {


        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        panelRenderer = GetComponent<PanelRenderer>();

        if (panelRenderer == null) 
        {
            Debug.LogError("ResourceManager requires Panel Renderer");
            return;
        }

        CurrentResources = startingResources;

        UpdateResourceDisplay();
    }

    private void OnEnable()
    {
        if (panelRenderer != null)
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnDisable()
    {
        if (panelRenderer != null)
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        // Find the label inside the PanelRenderer's visual tree
        resourceLabel = root.Q<Label>("ResourceLabel");

        if (resourceLabel == null)
        {
            Debug.LogError("Could not find Label with name 'ResourceLabel'.");
            return;
        }

        // Update the label immediately
        UpdateResourceDisplay();
    }


    private void UpdateResourceDisplay()
    {
        if(resourceLabel != null)
        {
            resourceLabel.text = $"Magik: {CurrentResources}";
        }
    }

    //public bool CanAfford(int amount) => CurrentResources >= amount;

    public bool CanAfford(int amount)
    {
        return CurrentResources >= amount;
    }

    public bool Spend(int amount)
    {
        if (!CanAfford(amount)) return false;

        CurrentResources -= amount;

        OnResourcesChanged?.Invoke(CurrentResources);
        UpdateResourceDisplay();

        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;

        CurrentResources += amount;

        OnResourcesChanged?.Invoke(CurrentResources);
        UpdateResourceDisplay();
    }

    
}
