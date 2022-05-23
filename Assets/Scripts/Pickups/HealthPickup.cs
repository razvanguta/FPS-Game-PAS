using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private bool collected;

    public int healAmount;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !collected)
        {
            PlayerHealthController.instance.HealPlayer(healAmount);
            Destroy(gameObject);
            collected = true;
            AudioManager.instance.PlaySFX(5);
        } 
    }
}
