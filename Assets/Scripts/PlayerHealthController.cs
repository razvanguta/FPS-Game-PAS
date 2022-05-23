using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{

    public static PlayerHealthController instance;

    public int maxHealth, currentHealth;
    public int loadGameHP = 0;
    public float invincibleTime = 1f;
    private float invincibilityCounter;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log(currentHealth);
        if(loadGameHP != 0) {
            currentHealth = loadGameHP;
        }
        //setting health bar values
        UIController.instance.healthSlider.maxValue = maxHealth;
        UpdateHealthBar();
    }


    // Update is called once per frame
    void Update()
    {
        if(invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
        }
    }

    public void DamagePlayer(int damageAmount)
    {
        if (invincibilityCounter <= 0 && !GameManager.instance.levelEnding)
        {
            AudioManager.instance.PlaySFX(7);

            currentHealth -= damageAmount;

            UIController.instance.ShowDamage();

            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);
                currentHealth = 0;

                GameManager.instance.PlayerDied();

                AudioManager.instance.StopBGM();
                AudioManager.instance.PlaySFX(6);
                AudioManager.instance.StopSFX(7);

            }

            invincibilityCounter = invincibleTime;
            UpdateHealthBar();
        }
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString();
    }

    
}
