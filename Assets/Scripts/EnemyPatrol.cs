using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float speed = 2f;
    public float patrolDistance = 4f;
    public bool startMovingRight = true;

    [Header("Combat")]
    public float stompBounce = 10f;

    private Vector3 startPosition;
    private bool movingRight;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    private void Start()
    {
        startPosition = transform.position;
        movingRight = startMovingRight;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        // Movimiento con Rigidbody (mucho más estable que Transform.Translate)
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Comprobar distancia de patrulla
        float distanceMoved = transform.position.x - startPosition.x;
        if (distanceMoved >= patrolDistance && movingRight)
        {
            movingRight = false;
            FlipSprite();
        }
        else if (distanceMoved <= -patrolDistance && !movingRight)
        {
            movingRight = true;
            FlipSprite();
        }

        // Detectar borde del suelo (sin necesitar groundDetector hijo ni layer mask)
        if (col != null)
        {
            Bounds bounds = col.bounds;
            float checkX = movingRight ? bounds.max.x + 0.2f : bounds.min.x - 0.2f;
            float checkY = bounds.min.y;

            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(checkX, checkY),
                Vector2.down, 1.5f
            );

            // Si no hay suelo delante, girar
            if (hit.collider == null || hit.collider.gameObject == gameObject)
            {
                movingRight = !movingRight;
                FlipSprite();
            }
        }
    }

    private void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !movingRight;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth == null) return;

            // Comprobar si el jugador está cayendo encima (stomp)
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y < -0.5f)
            {
                // El jugador nos pisó - morir
                Die();

                // Bounce del jugador
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounce);
                }
            }
            else
            {
                // El jugador nos tocó por el lado - hacer daño
                if (!playerHealth.IsInvincible())
                {
                    playerHealth.TakeDamage();
                }
            }
        }
    }

    // También girar al chocar con paredes
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player")) return;

        // Si chocamos con algo por el lado, girar
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.7f)
            {
                movingRight = contact.normal.x > 0;
                FlipSprite();
                break;
            }
        }
    }

    private void Die()
    {
        isDead = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyDeath();

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(100);

        // Animación de muerte
        EnemyAnimator anim = GetComponent<EnemyAnimator>();
        if (anim != null)
        {
            anim.PlayDieAnimation();
        }
        else
        {
            // Fallback: aplastar visualmente
            transform.localScale = new Vector3(transform.localScale.x, 0.2f, 1f);
        }

        // Parar movimiento
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        // Desactivar colisión
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1.2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawLine(
            new Vector3(pos.x - patrolDistance, pos.y, pos.z),
            new Vector3(pos.x + patrolDistance, pos.y, pos.z)
        );
    }
}
