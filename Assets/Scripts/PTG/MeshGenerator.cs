using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail, Gradient gradient)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        // Define flat area size and position (e.g., a square in the center)
        int flatAreaSize = width / 16;  // Adjust as needed
        int flatAreaStartX = (width - flatAreaSize) / 2;
        int flatAreaStartZ = (height - flatAreaSize) / 2;

        for (int i = 0; i < height; i += meshSimplificationIncrement)
        {
            for (int j = 0; j < width; j += meshSimplificationIncrement)
            {
                // Check if the vertex is within the flat area
                bool isWithinFlatArea = j >= flatAreaStartX && j <= flatAreaStartX + flatAreaSize &&
                                        i >= flatAreaStartZ && i <= flatAreaStartZ + flatAreaSize;

                // Set height to zero if within the flat area, otherwise use the height curve
                float heightValue = isWithinFlatArea ? 1f : heightCurve.Evaluate(heightMap[j, i]) * heightMultiplier;

                meshData.vertices[vertexIndex] = new Vector3(topLeftX + j, heightValue, topLeftZ - i);
                meshData.uvs[vertexIndex] = new Vector2(j / (float)width, i / (float)height);

                if (j < width - 1 && i < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }


}
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Color[] colors;  // Add this line
    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshHeight * meshWidth];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        colors = new Color[meshWidth * meshHeight];  // Add this line
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;  // Add this line
        mesh.RecalculateNormals();
        return mesh;
    }
}
