using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
public class GrassObject
{
    public Mesh mesh;
    public Material material;
    public float probability; // Probability of this grass object being selected
    public Vector3 scale = Vector3.one; // Scale of this grass object (default is 1,1,1)
}

public class GrassGenerator : MonoBehaviour
{
    public List<GrassObject> grassObjects; // List of different grass objects to use
    public int grassCount = 10000;         // Number of grass blades to generate
    public float areaSize = 100f;          // Size of the area to cover with grass
    public LayerMask groundLayer;          // Layer mask to specify the ground mesh layer
    public Transform player;               // Reference to the player transform
    public float renderRadius = 50f;       // Radius within which grass will be rendered

    private Matrix4x4[] matrices;          // Array to store matrices for each grass blade
    private GrassObject[] selectedGrassObjects; // Array to store the corresponding grass object for each blade
    [SerializeField]
    private float adjustGrassHeight = 0.6f;

    void Start()
    {
        NormalizeProbabilities();
        GenerateGrass();
    }

    // Normalize the probabilities so that they sum to 1
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

    // Select a GrassObject based on the assigned probabilities
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
        matrices = new Matrix4x4[grassCount];
        selectedGrassObjects = new GrassObject[grassCount];

        for (int i = 0; i < grassCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-areaSize / 2f, areaSize / 2f),
                100f, // Start raycasting from a high position above the ground
                Random.Range(-areaSize / 2f, areaSize / 2f)
            );

            // Perform a raycast downward to find the surface of the ground mesh
            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 position = hit.point;
                position.y += adjustGrassHeight;
                float randomScaleFactor = Random.Range(0.8f, 1.8f); // Random scale variation
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                // Select a GrassObject based on the defined probabilities
                GrassObject selectedGrassObject = SelectGrassObject();
                selectedGrassObjects[i] = selectedGrassObject;

                // Apply the scale of the selected GrassObject
                Vector3 finalScale = selectedGrassObject.scale * randomScaleFactor;

                matrices[i] = Matrix4x4.TRS(position, rotation, finalScale);
            }
            else
            {
                // If no ground was found, set the position to be below the ground so it won't be visible
                matrices[i] = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);
                selectedGrassObjects[i] = null; // No valid grass object
            }
        }
    }

    void Update()
    {
        Vector3 playerPosition = player.position;

        Dictionary<GrassObject, List<Matrix4x4>> visibleGrassByObject = new Dictionary<GrassObject, List<Matrix4x4>>();

        // Iterate over all grass blades and determine if they are within the render radius
        for (int i = 0; i < grassCount; i++)
        {
            if (selectedGrassObjects[i] == null) continue; // Skip if no valid grass object

            Vector3 grassPosition = matrices[i].GetColumn(3); // Get the position from the matrix

            float distanceToPlayer = Vector3.Distance(playerPosition, grassPosition);
            if (distanceToPlayer <= renderRadius)
            {
                // Add the matrix to the appropriate list in the dictionary
                if (!visibleGrassByObject.ContainsKey(selectedGrassObjects[i]))
                {
                    visibleGrassByObject[selectedGrassObjects[i]] = new List<Matrix4x4>();
                }
                visibleGrassByObject[selectedGrassObjects[i]].Add(matrices[i]);
            }
        }

        // Draw the visible grass using GPU instancing, grouped by GrassObject
        foreach (var kvp in visibleGrassByObject)
        {
            if (kvp.Value.Count > 0)
            {
                Graphics.DrawMeshInstanced(kvp.Key.mesh, 0, kvp.Key.material, kvp.Value, null, ShadowCastingMode.Off, receiveShadows: true);
            }
        }
    }
}
