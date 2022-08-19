using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Health : MonoBehaviour
{
    public int maxHealth = 5;

    private int currentHealth;
    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void AddDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void AddHealth(int amount)
    {
        currentHealth += amount;
    }
    public void SetHealth(int amount)
    {
        currentHealth = amount;
    }
    public int GetCurrentHealth()
    {
        if (currentHealth <= 0)
        {
            return 0;
        }
        else
        {
            return currentHealth;
        }
    }
    private void Die()
    {
        if (gameObject.tag == "Enemy")
        {
            GameManager.onEnemyDie?.Invoke(GetComponent<Enemy>().score);
            GetComponent<Enemy>().FX();
        }
        if (gameObject.tag == "Player")
        {
            WaveManager.currentSpawnState = WaveManager.SpawnState.Counting;
            SceneManager.LoadScene(0);
        }
        
    }
}
