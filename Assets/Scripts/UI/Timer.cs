using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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

    private void Start()
    {

        duration = GameManager.Instance.chaseTimer;
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
        OnEnd();
    }

    private void OnEnd()
    {
        GameManager.Instance.huntingTime = true;
        dayLight.SetActive(false);
        nightLight.SetActive(true);
        RenderSettings.fog = true;
        RenderSettings.skybox = null;
        print("End");
    }
}
