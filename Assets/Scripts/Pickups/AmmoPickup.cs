using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private bool collected;
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Player" && !collected)
        {
            //give ammo to player
            PlayerController.instance.activeGun.getAmmo();
            Destroy(gameObject);
            collected = true;
            AudioManager.instance.PlaySFX(3);
        }
    }
}
