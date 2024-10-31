using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureGenerator : ObjectGenerator
{
    public GameObject signalSmokePrefab;

    public override void Generate()
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
                Vector3 signalPosition = hit.point;
                signalPosition += new Vector3(2, 2, 0);
                GameObject signal = Instantiate(signalSmokePrefab, signalPosition, Quaternion.identity);
                positionFound = true;
            }


            Debug.DrawRay(randomPosition, Vector3.down * 10, Color.red, 1.0f);
            attempts++;
        }
    }
}
