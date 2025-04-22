using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballPhysics : MonoBehaviour
{
    public GameObject hitEffectPrefab; // For particle effect
    public float lifeTime = 3f;
    public float damage = 20f;

    private Collider fireballCollider; // Fireball collider reference

    private void Start()
    {
        // Destroy the fireball after the lifetime ends
        Destroy(gameObject, lifeTime);
        fireballCollider = GetComponent<Collider>(); // Get the fireball's collider
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object is an enemy and deal damage if so
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject); // Fireball disappears on impact
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with other objects (like the lamp, or even the player)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        // Handle other interactions like LampGlow or other objects
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        LampGlow lamp = collision.gameObject.GetComponentInParent<LampGlow>();
        if (lamp != null)
        {
            lamp.Glow();
        }

        Destroy(gameObject); // Destroy the fireball after collision
    }
}
