using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhackItem : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    private ItemSpawner spawner;
    private Transform occupiedTransform;
    public void Init(ItemSpawner spawner, Transform occupiedTransform, Material material, Mesh mesh)
    {
        this.spawner = spawner;
        this.occupiedTransform = occupiedTransform;
        meshFilter.mesh = mesh;
        meshRenderer.material = material;
    }

    public void FreeSpace()
    {
        spawner.FreeSpace(occupiedTransform);
    }

    private void OnDestroy()
    {
        FreeSpace();
    }
}
