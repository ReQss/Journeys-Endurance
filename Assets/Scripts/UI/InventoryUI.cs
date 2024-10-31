using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    InventoryManager inventoryManager;
    public InventorySlot[] inventorySlots;
    [SerializeField]
    private Interactable pickedItem;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(inventorySlots.Length);
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < InventoryManager.Instance.itemsCount)
            {
                inventorySlots[i].AddItem(inventoryManager.items[i]);
            }
            // // else inventorySlots[i].
        }
    }

}
