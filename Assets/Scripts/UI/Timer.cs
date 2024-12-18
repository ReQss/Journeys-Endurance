using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class Timer : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        pause = !pause;
    }

    [SerializeField] private Image uiFill;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField]
    public GameObject dayLight;
    [SerializeField]
    public GameObject nightLight;

    private int duration;

    private int remainingDuration;

    private bool pause;
    public TextMeshProUGUI currentDay;
    public Animator lightAnimator;

    private void Start()
    {
        if (InventoryManager.Instance.currentDayNum != 0)
        {
            currentDay.text = "Day " + InventoryManager.Instance.currentDayNum.ToString();
        }
        else currentDay.text = "Day " + GameManager.Instance.currentDay.ToString();

        duration = GameManager.Instance.dayTimer;
        Being(duration);
    }

    private void Being(int Second)
    {
        remainingDuration = Second;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (remainingDuration >= 0)
        {
            if (!pause)
            {
                uiText.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";
                uiFill.fillAmount = Mathf.InverseLerp(0, duration, remainingDuration);
                remainingDuration--;
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
        if (GameManager.Instance.isDay)
            SetNightTime();
        else SetDayTime();
    }
    public void SetDayTime()
    {
        //gowno

        if (GameManager.Instance.currentDay > 7) return;
        GameManager.Instance.DestroyEnemies();
        switch (GameManager.Instance.difficultyLevel)
        {
            case 1:
                GameManager.Instance.GenerateEnemiesLevel1();
                Debug.Log("Gen level1");
                break;
            case 2:
                GameManager.Instance.GenerateEnemiesLevel2();
                Debug.Log("Gen level2");
                break;
            case 3:
                GameManager.Instance.GenerateEnemiesLevel3();
                Debug.Log("Gen level3");
                break;
            default:
                Debug.Log("level difficulty not found");
                break;
        }
        GameManager.Instance.huntingTime = false;
        GameManager.Instance.currentDay++;
        InventoryManager.Instance.currentDayNum = GameManager.Instance.currentDay;
        currentDay.text = "Day " + GameManager.Instance.currentDay.ToString();


        if (GameManager.Instance.currentDay <= 2)
        {
            GameManager.Instance.difficultyLevel = 1;
        }
        else if (GameManager.Instance.currentDay <= 5)
        {
            GameManager.Instance.difficultyLevel = 2;
        }
        else
        {
            GameManager.Instance.difficultyLevel = 3;
        }
        GameManager.Instance.isDay = true;
        Being(duration);
        lightAnimator.SetBool("isDay", false);
        RenderSettings.fog = false;
        // RenderSettings.skybox = RenderSettings.skybox;

    }
    private void SetNightTime()
    {
        GameManager.Instance.isDay = false;
        GameManager.Instance.huntingTime = true;
        // dayLight.SetActive(false);
        // nightLight.SetActive(true);
        lightAnimator.SetBool("isDay", true);
        RenderSettings.fog = true;
        // RenderSettings.skybox = null;
        Being(GameManager.Instance.nightTimer);
        // print("End");
    }


}
