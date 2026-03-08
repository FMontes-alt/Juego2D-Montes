using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text gameOverText;
    public Text scoreText;
    public Button retryButton;
    public Button menuButton;

    [Header("Animation")]
    public float fadeInDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);

        // Mostrar puntuación final
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = "Puntuación: " + GameManager.Instance.Score;
        }
    }

    private void Update()
    {
        // Fade in simple
        if (canvasGroup != null && canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeInDuration;
        }
    }

    private void OnRetryClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
        else
        {
            SceneManager.LoadScene("GameLevel");
        }
    }

    private void OnMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
