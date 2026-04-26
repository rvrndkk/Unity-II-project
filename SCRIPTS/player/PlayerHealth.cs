using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar; // Перетяни сюда HealthContainer из UI

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetupHearts(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (healthBar != null) healthBar.UpdateHearts(currentHealth);

        if (currentHealth <= 0) Die();
    }
    
    // Метод для лечения (артефакты!)
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthBar != null) healthBar.UpdateHearts(currentHealth);
    }

    void Die() { /* Твоя логика смерти */ }
}