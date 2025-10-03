using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Input System")]
    InputAction moveAction;
    InputAction jumpAction;

    [Header("Collision Detection")]
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;
    private float playerHalfHeight;

    [Header("Character Run")]
    [SerializeField] private float moveSpeed = 5f;
    private float moveInput;

    [Header("Character Jump")]
    [SerializeField] float jumpForce = 7f;

    [Header("Utilities")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Initial Run")]
    [SerializeField] private float startX = -10f;
    [SerializeField] private float targetX = -7f;
    private bool isInitialRunComplete = false;

    [Header("Character Attack")]
    [SerializeField] private bool isAttacking = false;
    [SerializeField] GameObject damege;


    private bool isGrounded;

    void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer not assigned!");
        if (animator == null) Debug.LogError("Animator not assigned!");
        damege.SetActive(false);

        playerHalfHeight = spriteRenderer.bounds.extents.y;

        // Set initial position
        Vector3 startPosition = transform.position;
        startPosition.x = startX;
        transform.position = startPosition;
    }

    void Update()
    {
        isGrounded = IsGroundedCheck();

        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        if (!isInitialRunComplete)
        {
            // Handle initial automatic run
            HandleInitialRun();
        }
        else
        {
            // Handle normal player input
            moveInput = moveAction.ReadValue<Vector2>().x;
            Run(moveInput);

            // Handle jumping
            if (jumpAction.IsPressed() && isGrounded)
            {
                Jump();
            }
        }

        Debug.DrawRay(transform.position, Vector2.down * (playerHalfHeight + 0.1f), Color.red);
    }

    private void HandleInitialRun()
    {
        if (transform.position.x < targetX)
        {
            moveInput = 1f; // Run right
            Run(moveInput);
        }
        else
        {
            moveInput = 0f;
            Run(moveInput);
            isInitialRunComplete = true;
        }
    }

    void FixedUpdate()
    {
        // Apply horizontal movement using physics
        float targetVelocityX = moveInput * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private void Run(float direction)
    {
        // Handle running animation
        animator.SetBool("isRunning", direction != 0 && isGrounded);

        // Handle sprite flipping
        if (direction != 0)
        {
            spriteRenderer.flipX = direction < 0;
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        // Wait for the attack animation duration
        yield return new WaitForSeconds(0.5f); // Change 0.5f to match your animation length

        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void Jump()
    {
        // Apply jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // Set jump animation
        animator.SetBool("isJumping", true);
    }

    private bool IsGroundedCheck()
    {
        bool grounded = Physics2D.Raycast(transform.position, Vector2.down, playerHalfHeight + 0.5f, groundLayer);

        // Update jumping animation based on grounded state
        if (grounded)
        {
            animator.SetBool("isJumping", false);
        }

        return grounded;
    }
}