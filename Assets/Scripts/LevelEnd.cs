using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [Header("Visual")]
    public float pulseSpeed = 2f;
    public float pulseMinScale = 0.9f;
    public float pulseMaxScale = 1.1f;

    private void Update()
    {
        // Animación de pulso para hacerlo visible
        float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Sonido de completar nivel
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLevelComplete();

            // Completar nivel
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LevelComplete();
            }
        }
    }
}
