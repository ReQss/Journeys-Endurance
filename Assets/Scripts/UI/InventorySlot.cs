
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Image icon;

    Item item;
    [SerializeField]
    private Button closeButton;

    void Start()
    {
        // icon = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddItem(Item newItem)
    {
        if (newItem != null)
        {
            item = newItem;
            icon.enabled = true;
            icon.sprite = newItem.icon;
            closeButton.interactable = true;
        }
    }
    public void RemoveItem()
    {
        InventoryManager.Instance.removeItem(item);
        item = null;
        icon.enabled = false;
        icon.sprite = null;
        closeButton.interactable = false;
        // InventoryUI.
    }
    public void SetItemNull()
    {
        item = null;
        icon.enabled = false;
        icon.sprite = null;
        closeButton.interactable = false;
        icon.color = Color.white;
    }
    public void useItem()
    {
        Debug.Log("Using item" + item.name);
        GameObject newObject = item.gameObject;
        InventoryManager.Instance.itemToUse = newObject;
        RemoveItem();
        // newObject.SetActive(true);

    }
}
