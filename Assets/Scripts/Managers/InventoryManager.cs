using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static InventoryManager _instance;
    public int itemsCount = 0;
    public GameObject itemToUse = null;
    public float pickUpDistance = 80f;

    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Inventory Manager is null");
            }

            return _instance;
        }
    }
    public List<Interactable> items = new List<Interactable>();
    void Start()
    {

    }
    private void Awake()
    {
        _instance = this;
    }
    void Update()
    {
        // pick item
        if (Input.GetMouseButtonDown(0))
        {

            pickUp();
        }
        if (itemToUse && ThirdPersonMovement.cursorLocked == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                placeObject();
            }
        }
    }
    private void placeObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform raycast
        if (Physics.Raycast(ray, out hit, 20))
        {
            GameObject newGameObject = Instantiate(itemToUse, hit.point, Quaternion.identity);
            newGameObject.SetActive(true);
            newGameObject.GetComponent<Interactable>().pickedUp = false;
            itemToUse = null;
        }
    }
    private void pickUp()
    {
        RaycastHit hit;
        Camera camera = Camera.main;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, pickUpDistance))
        {

            Debug.Log(hit.collider.gameObject.name);
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                if (interactable.pickedUp == false && interactable.isNearbyPlayer)
                {
                    InventoryManager.Instance.addItem(interactable);
                    interactable.gameObject.SetActive(false);
                    interactable.pickedUp = true;
                }

                Debug.Log("Interact");
            }

        }
    }
    public void addItem(Interactable item)
    {
        Interactable newItem = item;
        items.Add(newItem);
        itemsCount++;
    }
    public void removeItem(Interactable item)
    {
        itemsCount--;
        items.Remove(item);
    }

}
