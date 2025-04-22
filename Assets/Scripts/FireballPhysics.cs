using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballPhysics : MonoBehaviour
{
    public GameObject hitEffectPrefab; // optional: for particle effect
    public float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Optional: spawn hit particles
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Try to find a LampGlow component in the collided object's parents
        LampGlow lamp = collision.gameObject.GetComponentInParent<LampGlow>();
        if (lamp != null)
        {
            lamp.Glow();
        }

        // Destroy the fireball
        Destroy(gameObject);
    }
}

