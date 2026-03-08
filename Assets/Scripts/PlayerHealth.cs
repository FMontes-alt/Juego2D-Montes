using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;
    public float flashInterval = 0.15f;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimator playerAnimator;
    private float invincibilityTimer;
    private float flashTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            flashTimer -= Time.deltaTime;

            // Parpadeo visual
            if (flashTimer <= 0)
            {
                if (spriteRenderer != null)
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                flashTimer = flashInterval;
            }

            // Fin de invencibilidad
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                    spriteRenderer.enabled = true;
            }
        }
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        // Sonido de daño
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDamage();

        // Activar invencibilidad
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        flashTimer = flashInterval;

        // Animación de daño
        if (playerAnimator != null)
            playerAnimator.PlayHitAnimation();

        // Pequeño salto hacia atrás al recibir daño
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 knockback = new Vector2(
                -transform.localScale.x * 5f,
                7f
            );
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }

        // Notificar al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife();
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    public void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        flashTimer = flashInterval;
    }

    // Muerte por caer al vacío — ajusta killY según tu nivel
    [Header("Void Death")]
    public float killY = -20f;

    private void LateUpdate()
    {
        if (transform.position.y < killY && !isInvincible)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayDeath();

            if (playerAnimator != null)
                playerAnimator.PlayDieAnimation();

            if (GameManager.Instance != null)
                GameManager.Instance.LoseLife();
        }
    }
}
