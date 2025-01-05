using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPortal : MonoBehaviour
{
    public Vector2 areaSize = new Vector2(10f, 10f);
    public Vector3 areaCenter = Vector3.zero;

    public LayerMask terrainLayer;
    public Transform lever;
    private bool activated = false;

    void Start()
    {
        MoveToRandomPosition();
    }

    void MoveToRandomPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
            float randomZ = Random.Range(areaCenter.z - areaSize.y / 2, areaCenter.z + areaSize.y / 2);
            Vector3 randomPosition = new Vector3(randomX, areaCenter.y, randomZ);

            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer))
            {
                transform.position = hit.point;
                // Debug.Log("Znaleziono nową pozycję: " + hit.point);
                return;
            }
        }

        // Debug.LogWarning("Nie udało się znaleźć odpowiedniej pozycji dla obiektu.");
    }


    void Update()
    {
        Debug.Log(activated);

        float rotationX = lever.localEulerAngles.x;

        if (rotationX > 180)
        {
            rotationX -= 360;
        }

        if (rotationX < -20)
        {
            activated = false;
        }
        else if (rotationX > 20 && !activated)
        {
            activated = true;
            MoveToRandomPosition();
            Debug.Log("Position of portal changed");
        }
    }

}
