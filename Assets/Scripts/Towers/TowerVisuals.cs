using System;
using UnityEngine;

public class TowerVisuals : MonoBehaviour
{
    [Serializable]
    private class PathVisual
    {
        public UpgradePathSO path;
        public GameObject[] tierObjects;   // index 0 = tier 1, index 1 = tier 2...
    }

    [SerializeField] private PathVisual[] visuals;

    public void ShowTier(UpgradePathSO path, int tier)
    {
        foreach (var pv in visuals)
        {
            if (pv.path != path) continue;
            for (int i = 0; i < pv.tierObjects.Length; i++)
                if (pv.tierObjects[i] != null)
                    pv.tierObjects[i].SetActive(i == tier);   // only the current tier shows
        }
    }
}
