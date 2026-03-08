using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            // Actualizar punto de respawn
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetRespawnPosition(transform.position);
            }

            // Feedback visual - cambiar color
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.green;
            }

            // Sonido
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayCoin();
        }
    }
}
