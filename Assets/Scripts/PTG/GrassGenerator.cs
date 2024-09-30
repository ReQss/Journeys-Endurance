using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
public class GrassObject
{
    public Mesh highPolyMesh;
    public Mesh lowPolyMesh;
    public Material material;
    public float probability;
    public Vector3 scale = Vector3.one;
}
[System.Serializable]
public class TerrainHeight
{
    public float grassHeight;
    public float rockHeight;
    public float sandHeight;
}
public class GrassGenerator : MonoBehaviour
{
    public TerrainHeight terrainHeightLevel;
    public Vector3 terrainCenter;
    public Camera playerCamera;
    public Transform terrain;
    public float chunkSize = 10f;
    private Dictionary<Vector2Int, List<Matrix4x4>> grassChunks;
    private Dictionary<Vector2Int, GrassObject[]> grassObjectsByChunk;

    public List<GrassObject> grassObjects;
    public int grassCount = 10000;
    public float areaSize = 100f;
    public LayerMask groundLayer;
    public Transform player;
    public float renderRadius = 50f;
    public float switchDistance = 30f;
    [SerializeField]
    private float adjustGrassHeight = 0.6f;
    private ComputeBuffer matrixBuffer;

    public float spacingFactor = 0.5f;

    void Start()
    {
        NormalizeProbabilities();
        GenerateGrass();
    }

    void NormalizeProbabilities()
    {
        float totalProbability = 0f;
        foreach (var grassObject in grassObjects)
        {
            totalProbability += grassObject.probability;
        }

        for (int i = 0; i < grassObjects.Count; i++)
        {
            grassObjects[i].probability /= totalProbability;
        }
    }

    GrassObject SelectGrassObject()
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var grassObject in grassObjects)
        {
            cumulativeProbability += grassObject.probability;
            if (randomValue < cumulativeProbability)
            {
                return grassObject;
            }
        }

        return grassObjects[grassObjects.Count - 1];
    }

    void GenerateGrass()
    {
        grassChunks = new Dictionary<Vector2Int, List<Matrix4x4>>();
        grassObjectsByChunk = new Dictionary<Vector2Int, GrassObject[]>();

        Debug.Log(terrainCenter);
        int rowCount = Mathf.CeilToInt(Mathf.Sqrt(grassCount));
        float cellSize = (areaSize / rowCount) * spacingFactor;

        int index = 0;

        for (int x = 0; x < rowCount; x++)
        {
            for (int z = 0; z < rowCount; z++)
            {
                if (index >= grassCount)
                    return;

                Vector3 position = new Vector3(
                    (x * cellSize) - (areaSize / 2f) + terrainCenter.x
                    ,
                    100f,
                    (z * cellSize) - (areaSize / 2f) + terrainCenter.z
                );

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
                {

                    position = hit.point;

                    if (position.y > terrainHeightLevel.rockHeight || position.y < terrainHeightLevel.sandHeight)
                    {
                        // index++;
                        continue;
                    }
                    position.y += adjustGrassHeight;

                    float randomScaleFactor = Random.Range(0.8f, 1.8f);
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    Vector2Int chunkCoord = new Vector2Int(
                        Mathf.FloorToInt(position.x / chunkSize),
                        Mathf.FloorToInt(position.z / chunkSize)
                    );

                    GrassObject selectedGrassObject = SelectGrassObject();
                    Vector3 finalScale = selectedGrassObject.scale * randomScaleFactor;
                    Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, finalScale);

                    if (!grassChunks.ContainsKey(chunkCoord))
                    {
                        grassChunks[chunkCoord] = new List<Matrix4x4>();
                        grassObjectsByChunk[chunkCoord] = new GrassObject[grassCount];
                    }

                    grassChunks[chunkCoord].Add(matrix);
                    grassObjectsByChunk[chunkCoord][grassChunks[chunkCoord].Count - 1] = selectedGrassObject;

                    index++;
                }
            }
        }

        matrixBuffer = new ComputeBuffer(grassCount, 64);
    }

    void Update()
    {
        Vector3 playerPosition = player.position;
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        Dictionary<GrassObject, List<Matrix4x4>> visibleHighPolyGrass = new Dictionary<GrassObject, List<Matrix4x4>>();
        Dictionary<GrassObject, List<Matrix4x4>> visibleLowPolyGrass = new Dictionary<GrassObject, List<Matrix4x4>>();

        foreach (var kvp in grassChunks)
        {
            Vector2Int chunkCoord = kvp.Key;
            Vector3 chunkCenter = new Vector3(
                chunkCoord.x * chunkSize + chunkSize / 2,
                0f,
                chunkCoord.y * chunkSize + chunkSize / 2
            );

            float minY = Mathf.Infinity;
            float maxY = -Mathf.Infinity;
            foreach (var matrix in kvp.Value)
            {
                float yPos = matrix.GetColumn(3).y;
                if (yPos < minY) minY = yPos;
                if (yPos > maxY) maxY = yPos;
            }

            float chunkHeight = maxY - minY;
            Bounds chunkBounds = new Bounds(
                new Vector3(chunkCenter.x, (minY + maxY) / 2, chunkCenter.z),
                new Vector3(chunkSize, chunkHeight, chunkSize)
            );

            if (GeometryUtility.TestPlanesAABB(frustumPlanes, chunkBounds))
            {
                if (Vector3.Distance(playerPosition, chunkCenter) <= renderRadius)
                {
                    for (int i = 0; i < kvp.Value.Count; i++)
                    {
                        Vector3 grassPosition = kvp.Value[i].GetColumn(3);
                        float distanceToPlayer = Vector3.Distance(playerPosition, grassPosition);
                        GrassObject grassObject = grassObjectsByChunk[kvp.Key][i];

                        if (distanceToPlayer <= switchDistance)
                        {
                            if (!visibleHighPolyGrass.ContainsKey(grassObject))
                            {
                                visibleHighPolyGrass[grassObject] = new List<Matrix4x4>();
                            }
                            visibleHighPolyGrass[grassObject].Add(kvp.Value[i]);
                        }
                        else
                        {
                            if (!visibleLowPolyGrass.ContainsKey(grassObject))
                            {
                                visibleLowPolyGrass[grassObject] = new List<Matrix4x4>();
                            }
                            visibleLowPolyGrass[grassObject].Add(kvp.Value[i]);
                        }
                    }
                }
            }
        }

        // Render high-poly grass
        foreach (var kvp in visibleHighPolyGrass)
        {
            if (kvp.Value.Count > 0)
            {
                matrixBuffer.SetData(kvp.Value.ToArray());
                Graphics.DrawMeshInstanced(kvp.Key.highPolyMesh, 0, kvp.Key.material, kvp.Value.ToArray(), kvp.Value.Count, null, ShadowCastingMode.Off, receiveShadows: true);
            }
        }

        // Render low-poly grass
        foreach (var kvp in visibleLowPolyGrass)
        {
            if (kvp.Value.Count > 0)
            {
                matrixBuffer.SetData(kvp.Value.ToArray());
                Graphics.DrawMeshInstanced(kvp.Key.lowPolyMesh, 0, kvp.Key.material, kvp.Value.ToArray(), kvp.Value.Count, null, ShadowCastingMode.Off, receiveShadows: true);
            }
        }
    }


    void OnDestroy()
    {
        if (matrixBuffer != null)
            matrixBuffer.Release();
    }
}
