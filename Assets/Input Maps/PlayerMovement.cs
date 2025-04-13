using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign the main camera here

    private PlayerControls controls;
    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    private float rotationVelocity;

    [Header("Camera Settings")]
    public float cameraDistance = 6f;         // How far the camera is from the player
    public float cameraHeight = 0.5f;         // Vertical offset of the camera from target position
    public float lookSensitivity = 1f;      // Mouse sensitivity
    public float minPitch = -40f;             // Min vertical angle
    public float maxPitch = 80f;              // Max vertical angle

    private float yaw = 0f;                   // Horizontal angle (around Y)
    private float pitch = 0f;                 // Vertical angle (up/down)

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();

        // Prevent player from tipping over
        rb.freezeRotation = true;

        // Lock cursor for better camera control (optional)
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
    }

    private void OnDisable()
    {
        controls.Land.Disable();
    }

    private void Update()
    {
        UpdateCameraRotation();
        UpdateCameraPosition();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveInput.sqrMagnitude < 0.01f) return;

        // Get movement direction relative to camera yaw
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        moveDirection.Normalize();

        // Rotate player toward movement direction
        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        // Apply velocity to Rigidbody
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void UpdateCameraRotation()
    {
        // Accumulate yaw/pitch with sensitivity
        yaw += lookInput.x;
        pitch -= lookInput.y;

        // Clamp vertical look (optional to prevent flipping)
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void UpdateCameraPosition()
    {
        // Calculate camera offset using pitch and yaw
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -cameraDistance);

        // Final camera position and look
        Vector3 targetPosition = transform.position + Vector3.up * cameraHeight;
        cameraTransform.position = targetPosition + offset;
        cameraTransform.LookAt(targetPosition);
    }
}

