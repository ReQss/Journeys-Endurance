using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRope : MonoBehaviour
{

    // Start is called before the first frame update
    public List<GameObject> gameObjects = new();
    void Start()
    {
        // for (int i = 0; i < transform.childCount; i++)
        // {
        //     ///  if(transform.GetChild(i).gameObject.)
        //     gameObjects.Add(transform.GetChild(i).gameObject);
        //     // transform.GetChild(i).
        // }
        // // transform.childCount
    }

    // Update is called once per frame
    bool flag = false;
    void Update()
    {

        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i].GetComponent<HingeJoint>() == null)
            {
                flag = true;
                // Debug.Log($"xx name {gameObjects[i].name}");
                break;
            }
        }
        if (flag)
        {
            // Destroy(gameObject);
        }
    }
}
