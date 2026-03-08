using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && !health.IsInvincible())
            {
                PlayerAnimator anim = other.GetComponent<PlayerAnimator>();
                if (anim != null) anim.PlayDieAnimation();

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayDeath();

                if (GameManager.Instance != null)
                    GameManager.Instance.LoseLife();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
