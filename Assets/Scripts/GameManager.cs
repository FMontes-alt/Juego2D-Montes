using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int maxLives = 3;

    // Estado del juego
    private int currentLives;
    private int score;
    private float levelTimer;
    private bool isGameActive;
    private Vector3 respawnPosition;

    // Propiedades públicas
    public int CurrentLives => currentLives;
    public int Score => score;
    public float LevelTimer => levelTimer;
    public bool IsGameActive => isGameActive;

    // Eventos
    public System.Action<int> OnLivesChanged;
    public System.Action<int> OnScoreChanged;
    public System.Action OnGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameLevel")
        {
            StartLevel();
        }
    }

    private void Update()
    {
        if (isGameActive)
        {
            levelTimer += Time.deltaTime;
        }
    }

    public void StartLevel()
    {
        currentLives = maxLives;
        score = 0;
        levelTimer = 0f;
        isGameActive = true;

        // Buscar punto de spawn del jugador
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        if (spawnPoint != null)
        {
            respawnPosition = spawnPoint.transform.position;
        }

        OnLivesChanged?.Invoke(currentLives);
        OnScoreChanged?.Invoke(score);
    }

    public void AddScore(int points)
    {
        if (!isGameActive) return;
        score += points;
        OnScoreChanged?.Invoke(score);
    }

    public void LoseLife()
    {
        if (!isGameActive) return;

        currentLives--;
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnPlayer();
        }
    }

    public void SetRespawnPosition(Vector3 position)
    {
        respawnPosition = position;
    }

    private void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = respawnPosition;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Activar invencibilidad para evitar morir al reaparecer
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ActivateInvincibility();
            }
        }
    }

    private void GameOver()
    {
        isGameActive = false;
        OnGameOver?.Invoke();
        // Pequeño delay antes de cargar la escena de Game Over
        Invoke(nameof(LoadGameOverScene), 1.5f);
    }

    private void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LevelComplete()
    {
        isGameActive = false;
        SceneManager.LoadScene("WinScreen");
    }

    public void LoadMainMenu()
    {
        isGameActive = false;
        score = 0;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("GameLevel");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(levelTimer / 60f);
        int seconds = Mathf.FloorToInt(levelTimer % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
