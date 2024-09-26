using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public GameObject connectedLever;
    private bool isActivated = false;
    private bool prevState = false;
    // Start is called before the first frame update
    void Start()
    {
        prevState = isActivated;
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedLever != null)
        {
            if (connectedLever.transform.rotation.x < 0)
            {
                isActivated = true;
            }
            else isActivated = false;
        }
        if (isActivated != prevState)
        {
            Debug.Log("Lever is ativated?: " + isActivated);
            prevState = isActivated;
        }

    }
}
