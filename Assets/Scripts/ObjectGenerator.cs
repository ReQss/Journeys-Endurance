using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public List<GameObject> prefabs;
    // [SerializeField]
    public Vector2Int xSize;
    // public int ySize;
    public Vector2Int zSize;
    public Vector2Int ySize;
    public LayerMask layerToExclude;
    public float radius = 0;
    public bool positionClear = true;
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
    public GameObject getRandomObject()
    {
        int randomValue = Random.Range(0, prefabs.Count);
        return prefabs[randomValue];
    }
    public virtual void Generate()
    {
        int layersToExclude = (1 << gameObject.layer) | (1 << layerToExclude.value);
        bool positionFound = false;
        int maxAttempts = 10;
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
                continue;
            }
            else positionClear = true;

            if (hit.point.y < ySize.x || hit.point.y > ySize.y) continue;
            if (hit.collider.tag == "Ground")
            {

                Ray objectRay = new Ray(randomPosition, Vector3.down);
                Debug.DrawRay(randomPosition, Vector3.down * hit.distance, Color.red);
                GameObject randomPrefab = getRandomObject();
                GameObject spawnedObject = Instantiate(randomPrefab, hit.point, Quaternion.identity);
                spawnedObject.transform.SetParent(instantiatedObjectsFolder.transform);
                positionFound = true;
            }


            Debug.DrawRay(randomPosition, Vector3.down * 10, Color.red, 1.0f);
            attempts++;
        }

    }



}