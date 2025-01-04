using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrassGenerator : MonoBehaviour
{
    public int instances;
    public Mesh mesh;
    public Material[] materials;
    public GameObject surfaceObject;
    private List<List<Matrix4x4>> batches = new();

    private void RenderBatches()
    {
        foreach (var batch in batches)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, materials[i], batch);
            }
        }
    }

    private void Start()
    {
        if (surfaceObject == null)
        {
            Debug.LogError("Surface Object is not assigned!");
            return;
        }

        Mesh surfaceMesh = surfaceObject.GetComponent<MeshFilter>()?.mesh;
        if (surfaceMesh == null)
        {
            Debug.LogError("The provided GameObject does not have a MeshFilter with a valid mesh!");
            return;
        }

        Vector3[] vertices = surfaceMesh.vertices;
        int addedMatrices = 0;

        for (int i = 0; i < instances; i++)
        {
            Vector3 position = surfaceObject.transform.TransformPoint(vertices[Random.Range(0, vertices.Length)]);

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

            if (addedMatrices < 1000 && batches.Count != 0)
            {
                batches[batches.Count - 1].Add(matrix);
                addedMatrices += 1;
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                addedMatrices = 0;
            }
        }
    }

    void Update()
    {
        RenderBatches();
    }
}
