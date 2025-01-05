using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]

public class InventoryManager : MonoBehaviour
{
    public int currentDayNum = 0;
    // Start is called before the first frame update
    [SerializeField]
    public List<Item> gameItems;
    public static InventoryManager _instance;
    public int itemsCount = 0;
    public GameObject itemToUse = null;
    public float pickUpDistance = 80f;
    public int questItemNumber = 0;
    public bool isInventoryLoaded = false;

    public TextMeshProUGUI currentDay;
    public TextMeshProUGUI questItemNumberUI;
    public Camera mainCamera;
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

    public List<Item> items = new List<Item>();
    public List<string> listOfItemNames = new List<string>();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        isInventoryLoaded = false;
        LoadGameItemsPrefabs();
        LoadItems();
    }
    public void SaveItemNames()
    {
        listOfItemNames.Clear();
        foreach (Item item in items)
        {
            listOfItemNames.Add(item.itemName);
        }
    }
    public void LoadGameItemsPrefabs()
    {
        Debug.Log("Loading inventory games");
        gameItems.Clear();
        GameObject parentObject = GameObject.Find("GameItemPrefabs");
        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                // Debug.Log($"Child Name: {child.gameObject.name}");
                gameItems.Add(child.GetComponent<Item>());
            }
        }
        else
        {
            Debug.LogWarning("Parent GameObject not found in the scene.");
        }
    }
    public void LoadItems()
    {
        items.Clear();
        foreach (string itemName in listOfItemNames)
        {
            foreach (Item game_item in gameItems)
            {
                // Debug.Log(game_item.itemName + " + " + itemName);
                if (game_item.itemName.Equals(itemName))
                {
                    items.Add(game_item);
                }
            }
        }
    }
    void Update()
    {
        questItemNumber = countQuestItems();
        GameObject questItem_temp = GameObject.Find("QuestItemNum");
        if (questItem_temp != null)
        {
            questItemNumberUI = questItem_temp.GetComponent<TextMeshProUGUI>();
            questItemNumberUI.text = countQuestItems().ToString() + "/" + GameManager.Instance.questItemsRequired.ToString();
        }
        if (questItemNumber >= GameManager.Instance.questItemsRequired)
        {
            Debug.Log("End game");
        }
        if (Input.GetKeyDown(KeyCode.W) && isInventoryLoaded == false)
        {
            LoadGameItemsPrefabs();
            LoadItems();
            isInventoryLoaded = true;
        }
        // pick item
        if (Input.GetMouseButtonDown(0))
        {

            pickUp();
            SaveItemNames();
        }
        if (itemToUse && GameManager.cursorLocked == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                placeObject();
            }
        }
        GameObject finalNPC = GameObject.Find("FinalQuest");
        GameObject firstNPC = GameObject.Find("NPC");
        if (GameManager.Instance.questItemsRequired <= questItemNumber)
        {
            TextMeshProUGUI text = GameObject.Find("CurrentDay").GetComponent<TextMeshProUGUI>();
            currentDay = text;
            currentDay.text = "You have all the items";
            GameManager.Instance.isQuestAchieved = true;

            if (finalNPC != null)
            {
                finalNPC.SetActive(true);
                if (firstNPC)
                    firstNPC.SetActive(false);
                // Debug.Log("yy");
            }

        }
        else
        {
            if (finalNPC != null)
            {
                finalNPC.SetActive(false);
                firstNPC.SetActive(true);
            }
            // Debug.Log("xx");
        }
    }
    private void placeObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform raycast
        if (Physics.Raycast(ray, out hit, 20))
        {
            GameObject newGameObject = Instantiate(itemToUse, hit.point, Quaternion.identity);
            if (newGameObject.GetComponent<Item>().itemType == itemType.QUEST)
            {
                // newGameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                newGameObject.transform.position = new Vector3(hit.point.x, hit.point.y + 1.5f, hit.point.z);
            }
            newGameObject.SetActive(true);
            newGameObject.GetComponent<Item>().pickedUp = false;
            itemToUse = null;
        }
    }
    private void pickUp()
    {
        RaycastHit hit;
        Camera camera = mainCamera;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, pickUpDistance))
        {

            // Debug.Log(hit.collider.gameObject.name);
            Item interactable = hit.collider.GetComponent<Item>();

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
    private int countQuestItems()
    {
        int i = 0;
        foreach (Item item in items)
        {
            Debug.Log(item.itemType);
            if (item.itemType == itemType.QUEST)
            {
                i++;
            }
        }
        return i;
    }
    public void addItem(Item item)
    {
        Item newItem = item;
        items.Add(newItem);
        itemsCount++;

    }
    public void removeItem(Item item)
    {
        itemsCount--;
        items.Remove(item);
    }

}
