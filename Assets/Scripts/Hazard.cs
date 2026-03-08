using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Damage Settings")]
    public bool instantKill = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible())
            {
                playerHealth.TakeDamage();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvincible())
            {
                playerHealth.TakeDamage();
            }
        }
    }
}
