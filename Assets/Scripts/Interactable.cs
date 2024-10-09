using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private string itemName;
    public float radius = 3f;
    private bool isNearbyPlayer = false;
    public Transform player;
    public LayerMask playerMask;
    public bool pickedUp = false;
    public Sprite icon;



    private void Update()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceFromPlayer < 3f)
            isNearbyPlayer = true;
        else isNearbyPlayer = false;
    }
}
