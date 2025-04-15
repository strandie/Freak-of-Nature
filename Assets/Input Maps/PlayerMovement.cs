using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform vaultPole;  // Assign in inspector

    private PlayerControls controls;
    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 loc; // Local position of pole

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    private float rotationVelocity;

    [Header("Camera Settings")]
    public float cameraDistance = 6f;
    public float cameraHeight = 0.5f;
    public float lookSensitivity = 1f;
    public float minPitch = -40f;
    public float maxPitch = 80f;

    private float yaw = 0f;
    private float pitch = 0f;

    [Header("Vault Settings")]
    public float maxVaultForce = 15f;
    public float vaultChargeRate = 20f;
    public float vaultCooldown = 0.5f;

    private float currentVaultForce = 0f;
    private bool isChargingVault = false;
    private bool canVault = true;
    private bool isGrounded = true;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        controls.Land.Enable();

        controls.Land.WASD.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Land.WASD.canceled += ctx => moveInput = Vector2.zero;

        controls.Land.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Land.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Land.Vault.started += ctx => StartVaultCharge();
        controls.Land.Vault.canceled += ctx => ReleaseVault();
    }

    private void OnDisable()
    {
        controls.Land.Disable();
    }

    private void Update()
    {
        UpdateCameraRotation();
        UpdateCameraPosition();

        if (isChargingVault)
        {
            currentVaultForce += vaultChargeRate * Time.deltaTime;
            currentVaultForce = Mathf.Clamp(currentVaultForce, 0f, maxVaultForce);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        CheckGrounded();
    }

    private void HandleMovement()
    {
        if (moveInput.sqrMagnitude < 0.01f) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        moveDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void StartVaultCharge()
    {
        //if (!isGrounded || !canVault) return;
        Debug.Log("RIGHT CLICK STARTED");
        isChargingVault = true;
        currentVaultForce = 0f;
    }

    private void ReleaseVault()
    {
        //if (!isChargingVault || !isGrounded || !canVault) return;
        Debug.Log("RIGHT CLICK RELEASED");

        isChargingVault = false;
        Vault(currentVaultForce);
        currentVaultForce = 0f;
        StartCoroutine(ResetVaultCooldown());
    }

    private void Vault(float vaultForce)
    {
        if (vaultPole == null) return;

        // Plant pole in front of frog
        Vector3 plantPosition = transform.position + transform.forward * 1f + Vector3.down * 0.5f;
        vaultPole.position = plantPosition;
        vaultPole.rotation = Quaternion.LookRotation(-transform.forward + Vector3.up);

        // Launch the frog
        Vector3 vaultDirection = (transform.forward * 0.5f + Vector3.up * 1.2f).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(vaultDirection * vaultForce, ForceMode.Impulse);

        isGrounded = false;

        StartCoroutine(SimulatePoleRotation());
    }

    private IEnumerator SimulatePoleRotation()
    {
        float duration = 0.4f;
        float elapsed = 0f;

        Quaternion startRot = vaultPole.rotation;
        Quaternion endRot = Quaternion.Euler(vaultPole.eulerAngles + new Vector3(-80f, 0f, 0f));

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            vaultPole.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        loc = new Vector3(0.27f, 0.52f, -1.28f); // Pole coordinates

        vaultPole.localPosition = loc;
        vaultPole.localRotation = Quaternion.identity;
    }

    private IEnumerator ResetVaultCooldown()
    {
        canVault = false;
        yield return new WaitForSeconds(vaultCooldown);
        canVault = true;
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        isGrounded = Physics.Raycast(ray, 0.6f);

        Debug.DrawRay(ray.origin, ray.direction * 0.2f, isGrounded ? Color.green : Color.red); //debug ray
    }

    private void UpdateCameraRotation()
    {
        yaw += lookInput.x;
        pitch -= lookInput.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -cameraDistance);
        Vector3 targetPosition = transform.position + Vector3.up * cameraHeight;

        cameraTransform.position = targetPosition + offset;
        cameraTransform.LookAt(targetPosition);
    }
}

