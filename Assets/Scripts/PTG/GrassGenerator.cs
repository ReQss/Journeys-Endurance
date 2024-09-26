using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
public class GrassObject
{
    public Mesh highPolyMesh;    // Normal detailed grass mesh
    public Mesh lowPolyMesh;     // Low-poly version of the grass
    public Material material;
    public float probability;    // Probability of this grass object being selected
    public Vector3 scale = Vector3.one; // Scale of this grass object (default is 1,1,1)
}

public class GrassGenerator : MonoBehaviour
{
    public Camera playerCamera;
    public Transform terrain;   // Reference to the terrain's transform
    public float chunkSize = 10f; // Size of each chunk
    private Dictionary<Vector2Int, List<Matrix4x4>> grassChunks; // Store grass by chunk
    private Dictionary<Vector2Int, GrassObject[]> grassObjectsByChunk; // Store the grass object associated with each chunk

    public List<GrassObject> grassObjects; // List of different grass objects to use
    public int grassCount = 10000;         // Number of grass blades to generate
    public float areaSize = 100f;          // Size of the area to cover with grass
    public LayerMask groundLayer;          // Layer mask to specify the ground mesh layer
    public Transform player;               // Reference to the player transform
    public float renderRadius = 50f;       // Radius within which grass will be rendered
    public float switchDistance = 30f;     // Distance at which to switch between high and low poly
    private Matrix4x4[] matrices;          // Array to store matrices for each grass blade
    private GrassObject[] selectedGrassObjects; // Array to store the corresponding grass object for each blade
    [SerializeField]
    private float adjustGrassHeight = 0.6f;
    private ComputeBuffer matrixBuffer; // Buffer for grass matrices

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
        float randomValue = Random.value; // Random value between 0 and 1
        float cumulativeProbability = 0f;

        foreach (var grassObject in grassObjects)
        {
            cumulativeProbability += grassObject.probability;
            if (randomValue < cumulativeProbability)
            {
                return grassObject;
            }
        }

        return grassObjects[grassObjects.Count - 1]; // Fallback in case of rounding errors
    }

    void GenerateGrass()
    {
        grassChunks = new Dictionary<Vector2Int, List<Matrix4x4>>();
        grassObjectsByChunk = new Dictionary<Vector2Int, GrassObject[]>();

        Vector3 terrainCenter = terrain.position; // Get the terrain center

        int rowCount = Mathf.CeilToInt(Mathf.Sqrt(grassCount)); // Number of rows and columns
        float cellSize = (areaSize / rowCount) * spacingFactor;

        int index = 0;

        for (int x = 0; x < rowCount; x++)
        {
            for (int z = 0; z < rowCount; z++)
            {
                if (index >= grassCount)
                    return;

                // Adjust position based on the terrain center
                Vector3 position = new Vector3(
                    (x * cellSize) + terrainCenter.x - (areaSize / 2f),
                    100f, // Start raycasting from a high position above the ground
                    (z * cellSize) + terrainCenter.z - (areaSize / 2f)
                );

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
                {
                    position = hit.point;
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

        matrixBuffer = new ComputeBuffer(grassCount, 64); // 64 bytes per Matrix4x4
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

            Bounds chunkBounds = new Bounds(chunkCenter, new Vector3(chunkSize, 100f, chunkSize));

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

        // Use GPU instancing for high-poly and low-poly grass
        foreach (var kvp in visibleHighPolyGrass)
        {
            if (kvp.Value.Count > 0)
            {
                matrixBuffer.SetData(kvp.Value.ToArray());
                Graphics.DrawMeshInstanced(kvp.Key.highPolyMesh, 0, kvp.Key.material, kvp.Value.ToArray(), kvp.Value.Count, null, ShadowCastingMode.Off, receiveShadows: true);
            }
        }

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
