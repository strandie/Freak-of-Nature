using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FrogRagdollSetup : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    public bool startInRagdoll = false;
    public float limbMass = 0.5f;
    public float bodyMass = 2f;
    public float headMass = 1f;

    public List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
    public List<Collider> ragdollColliders = new List<Collider>();
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        InitializeRagdoll();
        ToggleRagdoll(startInRagdoll);
    }

    private void InitializeRagdoll()
    {
        // Clear existing lists
        ragdollRigidbodies.Clear();
        ragdollColliders.Clear();

        // Get all rigidbodies and colliders in children
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        Collider[] cols = GetComponentsInChildren<Collider>();

        // Exclude the main rigidbody and collider
        foreach (Rigidbody rb in rbs)
        {
            if (rb != GetComponent<Rigidbody>())
            {
                ragdollRigidbodies.Add(rb);
                rb.mass = GetMassForPart(rb.gameObject);
            }
        }

        foreach (Collider col in cols)
        {
            if (col != GetComponent<Collider>())
            {
                ragdollColliders.Add(col);
            }
        }
    }

    private float GetMassForPart(GameObject part)
    {
        if (part.name.Contains("Body")) return bodyMass;
        if (part.name.Contains("Head")) return headMass;
        if (part.name.Contains("Eye")) return 0.2f;
        return limbMass;
    }

    public void ToggleRagdoll(bool state)
    {
        // Enable/disable animator
        if (animator != null)
            animator.enabled = !state;

        // Main rigidbody and collider
        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb != null)
        {
            mainRb.isKinematic = state;
            mainRb.detectCollisions = !state;
        }

        Collider mainCol = GetComponent<Collider>();
        if (mainCol != null)
            mainCol.enabled = !state;

        // Ragdoll parts
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
            rb.detectCollisions = state;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = state;
        }

        // If enabling ragdoll, match positions
        if (state)
            MatchRagdollToAnimator();
    }

    private void MatchRagdollToAnimator()
    {
        // This matches the ragdoll position to the current animated pose
        Vector3 position = animator.rootPosition;
        Quaternion rotation = animator.rootRotation;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb.transform.parent != null && rb.transform.parent.TryGetComponent<Animator>(out _))
            {
                rb.MovePosition(animator.GetBoneTransform(HumanBodyBones.Head).position);
                rb.MoveRotation(animator.GetBoneTransform(HumanBodyBones.Head).rotation);
            }
            // Add more specific mappings for each body part
        }
    }
}
