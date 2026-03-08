using UnityEngine;

/// <summary>
/// Controla las animaciones del enemigo Mushroom cambiando sprites según su estado.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAnimator : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] idleSprites;
    public Sprite[] runSprites;
    public Sprite[] hitSprites;
    public Sprite[] dieSprites;

    [Header("Velocidad de animación")]
    public float idleFPS = 6f;
    public float runFPS = 10f;
    public float hitFPS = 8f;
    public float dieFPS = 10f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private Sprite[] currentAnim;
    private float currentFPS;
    private float frameTimer;
    private int currentFrame;
    private bool animLocked;
    private float lockTimer;

    private enum AnimState { Idle, Run, Hit, Die }
    private AnimState currentState = AnimState.Idle;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (runSprites != null && runSprites.Length > 0)
            SetAnimation(runSprites, runFPS);
        else if (idleSprites != null && idleSprites.Length > 0)
            SetAnimation(idleSprites, idleFPS);
    }

    private void Update()
    {
        if (animLocked)
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0)
            {
                animLocked = false;
                if (currentState == AnimState.Die)
                {
                    // Quedarse en el último frame de muerte
                    if (dieSprites != null && dieSprites.Length > 0)
                        sr.sprite = dieSprites[dieSprites.Length - 1];
                    return;
                }
            }
            else
            {
                UpdateFrame();
                return;
            }
        }

        // Determinar estado
        AnimState newState = DetermineState();

        if (newState != currentState)
        {
            currentState = newState;
            switch (currentState)
            {
                case AnimState.Idle:
                    SetAnimation(idleSprites, idleFPS);
                    break;
                case AnimState.Run:
                    SetAnimation(runSprites, runFPS);
                    break;
            }
        }

        UpdateFrame();
    }

    private AnimState DetermineState()
    {
        if (rb == null) return AnimState.Idle;
        float velX = Mathf.Abs(rb.linearVelocity.x);
        return velX > 0.1f ? AnimState.Run : AnimState.Idle;
    }

    private void SetAnimation(Sprite[] sprites, float fps)
    {
        if (sprites == null || sprites.Length == 0) return;
        currentAnim = sprites;
        currentFPS = fps;
        currentFrame = 0;
        frameTimer = 0;
        sr.sprite = currentAnim[0];
    }

    private void UpdateFrame()
    {
        if (currentAnim == null || currentAnim.Length <= 1) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / currentFPS)
        {
            frameTimer -= 1f / currentFPS;
            currentFrame = (currentFrame + 1) % currentAnim.Length;
            sr.sprite = currentAnim[currentFrame];
        }
    }

    public void PlayHitAnimation()
    {
        if (hitSprites != null && hitSprites.Length > 0)
        {
            currentState = AnimState.Hit;
            SetAnimation(hitSprites, hitFPS);
            animLocked = true;
            lockTimer = hitSprites.Length / hitFPS;
        }
    }

    public void PlayDieAnimation()
    {
        if (dieSprites != null && dieSprites.Length > 0)
        {
            currentState = AnimState.Die;
            SetAnimation(dieSprites, dieFPS);
            animLocked = true;
            lockTimer = dieSprites.Length / dieFPS;
        }
    }
}
