using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PowerBar : MonoBehaviour
{
    public Slider slider;

    private bool isRegenerating = false;


    private void Update()
    {
        if (slider.value < slider.maxValue)
        {
            if (isRegenerating == false)
                StartCoroutine(PowerRege());
        }
    }
    private IEnumerator PowerRege()
    {
        isRegenerating = true;
        yield return new WaitForSeconds(3f);

        slider.value++;
        isRegenerating = false;
    }
    public void UsePower()
    {
        slider.value -= 1;
    }

}
