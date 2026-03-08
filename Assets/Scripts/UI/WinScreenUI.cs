using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreenUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text winText;
    public Text scoreText;
    public Text timeText;
    public Button menuButton;
    public Button replayButton;

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

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);

        if (replayButton != null)
            replayButton.onClick.AddListener(OnReplayClicked);

        // Mostrar estadísticas finales
        if (GameManager.Instance != null)
        {
            if (scoreText != null)
                scoreText.text = "Puntuación: " + GameManager.Instance.Score;

            if (timeText != null)
                timeText.text = "Tiempo: " + GameManager.Instance.GetFormattedTime();
        }
    }

    private void Update()
    {
        // Fade in
        if (canvasGroup != null && canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeInDuration;
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

    private void OnReplayClicked()
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
}
