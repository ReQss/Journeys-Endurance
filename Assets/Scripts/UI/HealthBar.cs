using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 10;
    [SerializeField]
    private Gradient gradient;
    private int health;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image fill;
    [SerializeField]
    private bool isEnemy = true;
    [SerializeField]
    private Transform cam;


    // Start is called before the first frame update
    private void Awake()
    {
        cam = Camera.main.transform;
        SetMaxHealth(maxHealth);
    }
    private void LateUpdate()
    {
        if (isEnemy)
            transform.LookAt(transform.position + cam.forward);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        this.health = health;
        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    public void TakeDamage(int damage)
    {
        this.health -= damage;
        SetHealth(this.health);
        if (GameManager.Instance.playerHealth <= 0)
        {
            SceneManager.LoadScene("Menu");
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void AddHealth()
    {
        if (this.health >= maxHealth) return;
        this.health += 1;
        SetHealth(this.health);
    }

}
