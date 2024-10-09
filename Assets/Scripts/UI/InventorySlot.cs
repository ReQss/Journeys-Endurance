
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // Start is called before the first frame update
    Image icon;
    Interactable item;
    [SerializeField]
    private Button closeButton;

    void Start()
    {
        icon = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddItem(Interactable newItem)
    {
        if (newItem != null)
        {
            item = newItem;
            icon.enabled = true;
            icon.sprite = newItem.icon;
            closeButton.interactable = true;
        }
    }
}
