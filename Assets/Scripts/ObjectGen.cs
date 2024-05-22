using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectGen : MonoBehaviour
{
    [SerializeField]
    GameObject prefab; // Prefab of the cube you want to spawn
    // [SerializeField]
    public Vector2Int xSize;
    // public int ySize;
    public Vector2Int zSize;
    public Vector2Int ySize;
    public LayerMask layerToExclude;
    private float radius = 0;
    private bool positionClear = true;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Collider collider = prefab.GetComponent<Collider>();
        if (collider != null)
        {
            radius = collider.bounds.extents.magnitude;
        }
        else
        {
            Debug.LogWarning("Prefab does not have a collider component.");
        }
    }
    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = positionClear ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public void Generate()
    {
        int layersToExclude = (1 << gameObject.layer) | (1 << layerToExclude.value);
        bool positionFound = false;
        int maxAttempts = 10; // Maximum number of attempts to find a valid position
        int attempts = 0;

        while (!positionFound && attempts < maxAttempts)
        {
            int randomX = Random.Range(xSize.x, xSize.y);
            int randomZ = Random.Range(zSize.x, zSize.y);
            int randomY = Random.Range(ySize.x, ySize.y);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);
            RaycastHit hit;
            if (!Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity)) continue;
            if (Physics.CheckSphere(randomPosition, radius + 5f, layersToExclude))
            {
                positionClear = false;
                Debug.Log("Object is colliding with something");
                Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
                continue;
            }
            else positionClear = true;

            if (hit.point.y < ySize.x || hit.point.y > ySize.y) continue;
            if (hit.collider.tag == "Ground") // == ground?
            {

                Ray objectRay = new Ray(randomPosition, Vector3.down);
                Debug.DrawRay(randomPosition, Vector3.down * hit.distance, Color.red);
                // Debug.Log("Object touched ground at position: " + hit.point);
                Instantiate(prefab, hit.point, Quaternion.identity);
                positionFound = true;
            }


            Debug.DrawRay(randomPosition, Vector3.down * 10, Color.red, 1.0f);
            attempts++;
        }

        if (!positionFound)
        {
            Debug.Log("Could not find a valid position to instantiate the object.");
        }
    }


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }


}