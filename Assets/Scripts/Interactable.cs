using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private string itemName;
    public float radius = 3f;
    public bool isNearbyPlayer = false;
    public Transform player;
    public LayerMask playerMask;
    public bool pickedUp = false;
    public Sprite icon;


    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    private void Update()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        isNearbyPlayer = distanceFromPlayer < radius;
    }
}
