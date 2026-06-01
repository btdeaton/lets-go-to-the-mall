using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

[RequireComponent(typeof(CharacterController))]
public class SourceMovementController : MonoBehaviour
{
    [Header("Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 0.5f; // Lowered for New Input System pixel deltas
    private float cameraPitch = 0f;

    [Header("Movement Specs")]
    public float maxSpeed = 7f;
    public float jumpForce = 5f;
    public float gravity = 20f;

    [Header("Source Engine Physics")]
    public float groundFriction = 6f;
    public float groundAcceleration = 14f;
    public float airAcceleration = 2f;
    public float maxAirSpeed = 2f;

    private CharacterController controller;
    private Vector3 playerVelocity = Vector3.zero;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        if (Mouse.current == null) return;

        // New Input System reads raw pixel delta
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        // Poll Keyboard for WASD
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveY += 1f;
            if (Keyboard.current.sKey.isPressed) moveY -= 1f;
            if (Keyboard.current.dKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        }

        Vector2 input = new Vector2(moveX, moveY).normalized;
        Vector3 wishDir = transform.right * input.x + transform.forward * input.y;

        if (isGrounded)
        {
            GroundMove(wishDir);

            // Jump via Spacebar
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            {
                playerVelocity.y = jumpForce;
                isGrounded = false;
            }
        }
        else
        {
            AirMove(wishDir);
        }

        if (!isGrounded)
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void GroundMove(Vector3 wishDir)
    {
        ApplyFriction();

        if (playerVelocity.y < 0)
            playerVelocity.y = -2f;

        Accelerate(wishDir, maxSpeed, groundAcceleration);
    }

    private void AirMove(Vector3 wishDir)
    {
        Accelerate(wishDir, maxAirSpeed, airAcceleration);
    }

    private void ApplyFriction()
    {
        Vector3 flatVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        float speed = flatVelocity.magnitude;

        if (speed != 0)
        {
            float drop = speed * groundFriction * Time.deltaTime;
            float newSpeed = Mathf.Max(speed - drop, 0);
            newSpeed /= speed;

            playerVelocity.x *= newSpeed;
            playerVelocity.z *= newSpeed;
        }
    }

    private void Accelerate(Vector3 wishDir, float targetSpeed, float accel)
    {
        float currentSpeed = Vector3.Dot(playerVelocity, wishDir);
        float addSpeed = targetSpeed - currentSpeed;

        if (addSpeed <= 0) return;

        float accelSpeed = accel * Time.deltaTime * targetSpeed;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        playerVelocity.x += accelSpeed * wishDir.x;
        playerVelocity.z += accelSpeed * wishDir.z;
    }
}