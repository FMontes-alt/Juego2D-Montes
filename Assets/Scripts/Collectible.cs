using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    public int scoreValue = 10;
    public float bobSpeed = 2f;
    public float bobAmount = 0.3f;
    public float rotateSpeed = 90f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Animación de flotación (sube y baja)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotación suave
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // Sonido
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoin();

        // Puntos
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        // Destruir
        Destroy(gameObject);
    }
}
