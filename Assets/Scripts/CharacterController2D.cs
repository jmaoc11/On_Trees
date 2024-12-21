using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float skinWidth = 0.02f;        // Distance for corner checks
    [SerializeField] private int horizontalRayCount = 4;     // Number of rays for better corner detection

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Configure physics settings for better corner handling
        Physics2D.queriesStartInColliders = false;
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().sharedMaterial = new PhysicsMaterial2D 
            { 
                friction = 0,          // Reduce friction for smoother corner movement
                bounciness = 0
            };
        }
    }

    void Update()
    {
        rb.gravityScale = gravityScale;
        Move();
        Jump();
        CheckGround();
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        
        // Flip the character based on movement direction
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1f, 1f);
        }

        // Set the isWalking parameter based on movement
        animator.SetBool("isWalking", moveInput != 0);
    }

    private void Jump()
    {
        if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void CheckGround()
    {
        // Use a circle overlap check instead of raycasts for more reliable ground detection
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
