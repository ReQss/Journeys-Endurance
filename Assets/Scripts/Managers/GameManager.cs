using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int chaseTimer;
    public int enemyChaseSpeed;
    public int enemyPatrolSpeed;
    public bool huntingTime = false;
    public int playerHealth;
    public bool isPlayerInteracting = false;

}
