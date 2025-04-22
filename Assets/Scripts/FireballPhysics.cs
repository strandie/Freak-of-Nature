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
        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null && other.CompareTag("Enemy"))
        {
            targetHealth.TakeDamage(damage);
            Destroy(gameObject); // Fireball disappears on impact
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug log to check if collision is triggered with the player
        Debug.Log("Fireball collided with: " + collision.gameObject.name);

        // Check if the colliding object is the player's root (or any collider you want to ignore)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ignore collision between fireball and player collider (but continue with other collisions)
            Collider playerCollider = collision.gameObject.GetComponent<Collider>(); // Player's collider
            if (playerCollider != null)
            {
                Debug.Log("Ignoring collision with Player.");
                Physics.IgnoreCollision(fireballCollider, playerCollider);
            }
        }

        // Handle other collisions like with enemies, lamps, etc.
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Handle LampGlow or other interactions here
        LampGlow lamp = collision.gameObject.GetComponentInParent<LampGlow>();
        if (lamp != null)
        {
            lamp.Glow();
        }

        // Destroy the fireball if it hits something else
        Destroy(gameObject);
    }
}
