using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4.5f;
    public float runSpeed = 7.5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    public float gravity = -18f;
    public float jumpForce = 5.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform cameraTransform;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 verticalVelocity;
    private CharacterController controller;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGrounded();
        HandleJump();
        ApplyGravity();
        HandleMovement();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer) || controller.isGrounded;
        if (isGrounded && verticalVelocity.y < 0)
            verticalVelocity.y = -2f;
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(inputX, 0, inputZ).normalized;
        Vector3 move = cameraTransform.TransformDirection(inputDir);
        move.y = 0f;
        move.Normalize();

        // Detect sprint
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 targetVelocity = move * targetSpeed;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity,
            (targetVelocity.magnitude > 0.1f ? acceleration : deceleration) * Time.deltaTime);

        Vector3 finalVelocity = currentVelocity + verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}