using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTrigger : MonoBehaviour
{
    public List<EnemiesController> enemies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemiesController enemy = collision.GetComponent<EnemiesController>();
            if (enemy != null && !enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemiesController enemy = collision.gameObject.GetComponent<EnemiesController>();
            if (enemy.currentHealth <= 0)
            {
                enemies.Remove(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemiesController enemy = collision.GetComponent<EnemiesController>();
            if (enemy != null)
            {
                enemies.Remove(enemy);
            }
        }
    }
}
