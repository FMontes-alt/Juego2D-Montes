using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;
    public bool useLocalPositions = true;

    private Vector3 worldPointA;
    private Vector3 worldPointB;
    private Vector3 targetPoint;

    private void Start()
    {
        if (useLocalPositions)
        {
            worldPointA = transform.position + pointA;
            worldPointB = transform.position + pointB;
        }
        else
        {
            worldPointA = pointA;
            worldPointB = pointB;
        }
        targetPoint = worldPointB;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            targetPoint = targetPoint == worldPointA ? worldPointB : worldPointA;
        }
    }

    // El jugador se mueve con la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = Application.isPlaying ? worldPointA : transform.position + pointA;
        Vector3 b = Application.isPlaying ? worldPointB : transform.position + pointB;
        Gizmos.DrawSphere(a, 0.2f);
        Gizmos.DrawSphere(b, 0.2f);
        Gizmos.DrawLine(a, b);
    }
}
