using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarVisualizationController : MonoBehaviour
{
    [SerializeField] private Mesh[] avaliableMeshes;
    [SerializeField] private MeshFilter bodyMeshFilter;

    private void Awake()
    {
        Mesh selectedMesh = avaliableMeshes[Random.Range(0, avaliableMeshes.Length)];
        bodyMeshFilter.mesh = selectedMesh;
    }

}
