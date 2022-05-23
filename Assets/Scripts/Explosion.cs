using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage = 25;

    public bool damageEnemy, damagePlayer;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy" && damageEnemy)
        {
            other.gameObject.GetComponent<EnemyHealthController>().DamageEnemy(damage);
        }

        if (other.gameObject.tag == "Player" && damagePlayer)
        {
            PlayerHealthController.instance.DamagePlayer(damage);
        }
    }
}
