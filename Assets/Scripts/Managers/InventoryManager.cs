using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static InventoryManager _instance;
    public int itemsCount = 0;

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
    // Update is called once per frame
    void Update()
    {

    }
    public void addItem(Interactable item)
    {
        items.Add(item);
        itemsCount++;
    }
}
