using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerVisuals : MonoBehaviour
{
    /*
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
    */
    [SerializeField] private MeshFilter meshFilter;   // the one filter that always stays active
    [SerializeField] private Outline outline;         // the Outline on this tower

    private Mesh[] stageMeshes;
    private MeshRenderer meshRenderer;
    private Vector3 baseLocalPosition;   // where the mesh child sits in the prefab
    private float baseBottom;            // height of the base mesh's lowest point above the tower root

    public void Init(TowerData data)
    {
        stageMeshes = data.stageMeshes;
        meshRenderer = meshFilter.GetComponent<MeshRenderer>();

        // Runs while the base mesh is still showing, so this is our reference
        baseLocalPosition = meshFilter.transform.localPosition;
        baseBottom = GetBottom();
    }

    private float GetBottom() => meshRenderer.bounds.min.y - transform.position.y;

    public void SetStage(int stage)
    {
        if (stageMeshes == null || stageMeshes.Length == 0) return;

        int index = Mathf.Min(stage, stageMeshes.Length - 1);
        Mesh mesh = stageMeshes[index];
        if (mesh == null || meshFilter.sharedMesh == mesh) return;

        // 1. Reset the offset from any previous swap so shifts don't stack
        meshFilter.transform.localPosition = baseLocalPosition;

        // 2. Swap the mesh
        meshFilter.sharedMesh = mesh;

        // 3. Move the child so the new mesh's lowest point matches the base mesh's
        float difference = baseBottom - GetBottom();
        meshFilter.transform.position += Vector3.up * difference;

        // 4. Prepare the new mesh for the outline
        if (outline != null) outline.PrepareMesh();
    }
}

