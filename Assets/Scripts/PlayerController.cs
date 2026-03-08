using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.15f;
    public LayerMask groundLayer;

    [Header("Animations")]
    public Animator animator;

    private Rigidbody2D rb;
    private Collider2D col;
    private float horizontalInput;
    private bool isGrounded;
    private bool facingRight = true;
    private bool jumpRequested;
    private bool jumpHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive) return;

        // Detectar suelo
        CheckGround();

        // Input
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Movimiento horizontal
        horizontalInput = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontalInput = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontalInput = 1f;

        // Salto
        if (kb.spaceKey.wasPressedThisFrame && isGrounded)
        {
            jumpRequested = true;
        }

        jumpHeld = kb.spaceKey.isPressed;

        // Flip
        if (horizontalInput > 0 && !facingRight) Flip();
        else if (horizontalInput < 0 && facingRight) Flip();

        UpdateAnimations();
    }

    private void CheckGround()
    {
        // Usar el collider del propio jugador para lanzar un BoxCast hacia abajo
        // Esto es MUCHO más fiable que OverlapCircle con un GroundCheck hijo
        Bounds bounds = col.bounds;

        // Lanzar un pequeño boxcast desde la parte inferior del collider
        RaycastHit2D hit = Physics2D.BoxCast(
            bounds.center,                           // origen: centro del collider
            new Vector2(bounds.size.x * 0.9f, 0.1f), // tamaño del box (un poco más estrecho)
            0f,                                       // ángulo
            Vector2.down,                             // dirección: abajo
            bounds.extents.y + groundCheckDistance,    // distancia: hasta justo debajo del collider
            ~LayerMask.GetMask("Player")              // ignorar al propio jugador
        );

        isGrounded = hit.collider != null;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive) return;

        // Movimiento horizontal
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Salto
        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayJump();
        }

        // Mejor sensación de salto
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 boxCenter = bounds.center + Vector3.down * (bounds.extents.y + groundCheckDistance / 2f);
        Vector3 boxSize = new Vector3(bounds.size.x * 0.9f, groundCheckDistance, 0);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
