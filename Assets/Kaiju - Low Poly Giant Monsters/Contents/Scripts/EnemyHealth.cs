using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f; // Default max health for the enemy
    private float currentHealth;

    public Slider healthSlider; // Health bar UI

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Method to apply damage to the enemy
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Handle enemy death
    private void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject); // Destroy the enemy game object
    }

    // Method to heal the enemy (if needed)
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reduce health when the enemy collides with the frog
            TakeDamage(10f); // Adjust the damage value as needed
        }
    }*/

}

