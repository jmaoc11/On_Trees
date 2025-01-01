using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float skinWidth = 0.02f;
    [SerializeField] private int horizontalRayCount = 4;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip walkingSound;
    [SerializeField] [Range(0f, 1f)] private float walkVolume = 0.5f;
    [SerializeField] private float stepRate = 0.5f; // Time between footsteps

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private AudioSource audioSource;
    private float stepTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Add AudioSource component if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        
        Physics2D.queriesStartInColliders = false;
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().sharedMaterial = new PhysicsMaterial2D 
            { 
                friction = 0,
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
        
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1f, 1f);
            
            // Handle walking sound
            if (isGrounded && walkingSound != null)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= stepRate)
                {
                    audioSource.PlayOneShot(walkingSound, walkVolume);
                    stepTimer = 0f;
                }
            }
        }
        else
        {
            stepTimer = stepRate; // Reset timer when not moving
        }

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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
