using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class Area
{
    public GameObject area;
    public int count;
    public GameObject collisionDetector;
    public float adjustPosition;
    public Vector2Int xSize;
    public Vector2Int zSize;
    public Vector2Int ySize;
    public float radius;
}

public class BuildingGenerator : MonoBehaviour
{

    public List<Area> areas;
    public LayerMask layerToExclude;
    private float radius = 0;
    private bool positionClear = true;
    public int numberOfObjects = 0;
    public GameObject instantiatedObjectsFolder;


    void Start()
    {

        for (int i = 0; i < numberOfObjects; i++)
        {
            Generate();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = positionClear ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void Generate()
    {

        foreach (Area area in areas)
        {
            for (int i = 0; i < area.count; i++)
            {
                int layersToExclude = (1 << gameObject.layer) | (1 << layerToExclude.value);
                bool positionFound = false;
                int maxAttempts = 10;
                int attempts = 0;
                Debug.Log("Areas spawned");
                while (!positionFound && attempts < maxAttempts)
                {
                    int randomX = UnityEngine.Random.Range(area.xSize.x, area.xSize.y);
                    int randomZ = UnityEngine.Random.Range(area.zSize.x, area.zSize.y);
                    int randomY = UnityEngine.Random.Range(area.ySize.x, area.ySize.y);
                    Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);
                    int g = 0;
                    while (Physics.CheckSphere(randomPosition, area.radius, layersToExclude))
                    {
                        // positionClear = false;
                        Debug.Log("Object is colliding with something");
                        area.adjustPosition += 40;
                        randomPosition.y += 40;
                        g++;
                        if (g == 10) break;
                    }
                    RaycastHit hit;
                    if (!Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity)) continue;
                    else positionClear = true;

                    if (hit.point.y < area.ySize.x || hit.point.y > area.ySize.y) continue;
                    if (hit.collider.tag == "Ground")
                    {

                        GameObject nextPrefab = area.area;
                        Debug.Log("Hit point : " + hit.point.y);
                        Vector3 objectSpawnPoint = hit.point;
                        objectSpawnPoint.y += area.adjustPosition;
                        GameObject spawnedObject = Instantiate(nextPrefab, objectSpawnPoint, Quaternion.identity);
                        spawnedObject.transform.SetParent(instantiatedObjectsFolder.transform);
                        positionFound = true;
                    }
                    attempts++;
                }

                if (!positionFound)
                {
                    Debug.Log("Could not find a valid position to instantiate the object.");
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }


}