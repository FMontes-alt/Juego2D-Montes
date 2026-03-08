using UnityEngine;

/// <summary>
/// Controla las animaciones del personaje cambiando sprites según su estado.
/// Se asignan automáticamente los sprites del spritesheet char_blue.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Sprites (se asignan solos si están vacíos)")]
    public Sprite[] idleSprites;     // Idle (parado)
    public Sprite[] runSprites;      // Correr
    public Sprite[] jumpSprites;     // Saltar (subiendo)
    public Sprite[] fallSprites;     // Caer (bajando)
    public Sprite[] attackSprites;   // Atacar
    public Sprite[] hitSprites;      // Recibir daño
    public Sprite[] dieSprites;      // Morir

    [Header("Velocidad de animación")]
    public float idleFPS = 6f;
    public float runFPS = 10f;
    public float jumpFPS = 8f;
    public float attackFPS = 12f;
    public float hitFPS = 8f;
    public float dieFPS = 8f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private PlayerController controller;
    private PlayerHealth health;

    private Sprite[] currentAnim;
    private float currentFPS;
    private float frameTimer;
    private int currentFrame;
    private bool animLocked; // Para animaciones que no se interrumpen (hit, die)
    private float lockTimer;

    private enum AnimState { Idle, Run, Jump, Fall, Attack, Hit, Die }
    private AnimState currentState = AnimState.Idle;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        // Auto-asignar sprites si no están asignados
        if (idleSprites == null || idleSprites.Length == 0)
        {
            LoadSpritesFromSheet();
        }

        // Empezar en idle
        SetAnimation(idleSprites, idleFPS);
    }

    private void Update()
    {
        // Manejar lock de animaciones
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

        // Determinar estado de animación según el estado del jugador
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
                case AnimState.Jump:
                    SetAnimation(jumpSprites, jumpFPS);
                    break;
                case AnimState.Fall:
                    SetAnimation(fallSprites, jumpFPS);
                    break;
            }
        }

        UpdateFrame();
    }

    private AnimState DetermineState()
    {
        if (rb == null) return AnimState.Idle;

        float velY = rb.linearVelocity.y;
        float velX = Mathf.Abs(rb.linearVelocity.x);

        // En el aire
        if (velY > 0.5f) return AnimState.Jump;
        if (velY < -0.5f) return AnimState.Fall;

        // En el suelo
        if (velX > 0.1f) return AnimState.Run;

        return AnimState.Idle;
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

    // Llamar desde PlayerHealth cuando recibe daño
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

    // Llamar desde PlayerHealth cuando muere
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

    // ========================================
    // AUTO-CARGA DE SPRITES
    // ========================================
    private void LoadSpritesFromSheet()
    {
        // Cargar todos los sub-sprites del spritesheet
        Sprite[] allSprites = Resources.LoadAll<Sprite>("char_blue");

        // Si no está en Resources, intentar cargar por nombre
        if (allSprites == null || allSprites.Length == 0)
        {
            // Buscar el sprite del SpriteRenderer y cargar sus hermanos
            if (sr.sprite != null)
            {
                string path = UnityEngine.Application.isEditor ? "" : "";
                Debug.Log("[PlayerAnimator] Asignando sprites manualmente por índice.");
            }
            return;
        }

        AssignSpritesByIndex(allSprites);
    }

    private void AssignSpritesByIndex(Sprite[] all)
    {
        if (all.Length < 48) return;

        // Basado en el layout del spritesheet char_blue:
        // Fila 1 (0-5): Idle
        idleSprites = GetRange(all, 0, 6);
        // Fila 2 (6-11): Attack
        attackSprites = GetRange(all, 6, 6);
        // Fila 3 (12-19): Run
        runSprites = GetRange(all, 12, 8);
        // Fila 4 (20-25): Jump/Fall
        jumpSprites = GetRange(all, 20, 3);
        fallSprites = GetRange(all, 23, 3);
        // Fila 6 (38-47): Hit + Die
        hitSprites = GetRange(all, 38, 3);
        dieSprites = GetRange(all, 41, 7);
    }

    private Sprite[] GetRange(Sprite[] source, int start, int count)
    {
        count = Mathf.Min(count, source.Length - start);
        if (count <= 0) return new Sprite[0];
        Sprite[] result = new Sprite[count];
        System.Array.Copy(source, start, result, 0, count);
        return result;
    }
}
