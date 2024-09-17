using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Enemy : MonoBehaviour {

    public float health = 100f;

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }
    private void Die() {
        scr_GameManager.Instance.EnemyKilled();

        Destroy(gameObject);
    }

}