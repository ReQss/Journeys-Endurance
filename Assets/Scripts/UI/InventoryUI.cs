using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    InventoryManager inventoryManager;
    public InventorySlot[] inventorySlots;
    [SerializeField]
    private Item pickedItem;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInventory();
    }
    public void UpdateInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < InventoryManager.Instance.itemsCount)
            {
                inventorySlots[i].AddItem(inventoryManager.items[i]);
                GameObject childObject = inventorySlots[i].transform.GetChild(0).gameObject;
                CheckItemType(childObject, inventoryManager.items[i]);
            }
            else
            {
                GameObject childObject = inventorySlots[i].transform.GetChild(0).gameObject;
                childObject.GetComponent<Image>().color = Color.white;
                inventorySlots[i].SetItemNull();
            }
        }
    }
    public void CheckItemType(GameObject childObject, Item item)
    {
        switch (item.itemType)
        {
            case itemType.UNCOMMON:
                childObject.GetComponent<Image>().color = Color.green;
                break;
            case itemType.RARE:
                childObject.GetComponent<Image>().color = Color.blue;
                break;
            case itemType.EPIC:
                childObject.GetComponent<Image>().color = Color.red;
                break;
            case itemType.QUEST:
                childObject.GetComponent<Image>().color = Color.black;
                break;
            default:
                childObject.GetComponent<Image>().color = Color.white;
                break;

        }
    }

}
