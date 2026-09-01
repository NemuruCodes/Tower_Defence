using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private int startingResources = 100;

    public int CurrentResources { get; private set; }

    public event Action<int> OnResourcesChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CurrentResources = startingResources;
    }

    public bool CanAfford(int amount) => CurrentResources >= amount;

    public bool Spend(int amount)
    {
        if (!CanAfford(amount)) return false;

        CurrentResources -= amount;
        OnResourcesChanged?.Invoke(CurrentResources);
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;

        CurrentResources += amount;
        OnResourcesChanged?.Invoke(CurrentResources);
    }
}
