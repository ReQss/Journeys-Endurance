using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; }

    public int dayTimer;
    public int nightTimer;
    public bool isDay = true;
    public int currentDay;
    public int enemyChaseSpeed;
    public int enemyPatrolSpeed;
    public bool huntingTime = false;
    public int playerHealth;
    public bool isPlayerInteracting = false;
    public TreasureGenerator enemyGeneratorLevel1;

    public TreasureGenerator enemyGeneratorLevel2;

    public TreasureGenerator enemyGeneratorLevel3;
    public bool isInvincible = false;
    public int questItemsRequired = 5;
    public int difficultyLevel = 1;
    public bool isQuestAchieved = false;
    public GameObject finalNPC;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Duplicate GameManager destroyed: " + gameObject.name);
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (isQuestAchieved)
        {
            if (finalNPC != null)
            {
                finalNPC.SetActive(true);
            }

        }
        else
        {
            GameObject finalNPC = GameObject.Find("FinalQuest");
            if (finalNPC != null)
            {
                Renderer renderer = finalNPC.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false; // Make the object invisible
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject finalNPC = GameObject.Find("FinalQuest");
            if (finalNPC != null)
            {
                finalNPC.SetActive(true);
                Debug.Log("yy");
            }
            else
            {
                Debug.Log("xx");
            }
        }
    }
    private void Start()
    {
        if (InventoryManager.Instance.currentDayNum != 0)
        {
            GameManager.Instance.currentDay = InventoryManager.Instance.currentDayNum;
        }

    }
    public void GenerateEnemiesLevel1()
    {
        for (int i = 0; i < enemyGeneratorLevel1.numberOfObjects; i++)
        {
            enemyGeneratorLevel1.Generate();
        }
    }
    public void GenerateEnemiesLevel2()
    {
        for (int i = 0; i < enemyGeneratorLevel2.numberOfObjects; i++)
        {
            enemyGeneratorLevel2.Generate();
        }
    }
    public void GenerateEnemiesLevel3()
    {
        for (int i = 0; i < enemyGeneratorLevel3.numberOfObjects; i++)
        {
            enemyGeneratorLevel3.Generate();
        }
    }
    public void DestroyEnemies()
    {
        foreach (Transform child in enemyGeneratorLevel1.instantiatedObjectsFolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
