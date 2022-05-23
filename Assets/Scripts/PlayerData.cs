using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData
{
    public string currentLevel;
    public float positionX, positionY, positionZ;

    public int health;
    public int sniperAmmo;
    public int pistolAmmo;
    public int repeaterAmmo;
    public int rocketAmmo;
    public int sniperRifleAmmo;
    public bool pickedUpSniper;

    public PlayerData ()
    {
        health = PlayerHealthController.instance.currentHealth;
        currentLevel = SceneManager.GetActiveScene().name;
        positionX = PlayerController.instance.transform.position.x;
        positionY = PlayerController.instance.transform.position.y;
        positionZ = PlayerController.instance.transform.position.z;
        pickedUpSniper = false;
        foreach (Gun gun in PlayerController.instance.allGuns)
        {
            switch(gun.gunName)
            {
                case "sniper":
                    sniperAmmo = gun.currentAmmo;
                    pickedUpSniper = true;
                    break;
                case "pistol":
                    pistolAmmo = gun.currentAmmo;
                    break;
                case "repeater":
                    repeaterAmmo = gun.currentAmmo;
                    break;
                default:
                    rocketAmmo = gun.currentAmmo;
                    break;
            }
        }

    }
}
