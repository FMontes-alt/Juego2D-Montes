using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text livesText;
    public Text scoreText;
    public Text timerText;

    [Header("Lives Display (Hearts)")]
    public Image[] heartImages;
    public Sprite heartFull;
    public Sprite heartEmpty;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged += UpdateLives;
            GameManager.Instance.OnScoreChanged += UpdateScore;

            // Inicializar valores
            UpdateLives(GameManager.Instance.CurrentLives);
            UpdateScore(GameManager.Instance.Score);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged -= UpdateLives;
            GameManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }

    private void Update()
    {
        // Actualizar temporizador
        if (GameManager.Instance != null && timerText != null)
        {
            timerText.text = GameManager.Instance.GetFormattedTime();
        }
    }

    private void UpdateLives(int lives)
    {
        // Actualizar texto
        if (livesText != null)
        {
            livesText.text = "x " + lives;
        }

        // Actualizar corazones (si se usan imágenes)
        if (heartImages != null)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] != null)
                {
                    if (heartFull != null && heartEmpty != null)
                    {
                        heartImages[i].sprite = i < lives ? heartFull : heartEmpty;
                    }
                    else
                    {
                        // Sin sprites asignados, cambiar color
                        heartImages[i].color = i < lives ? Color.red : new Color(0.3f, 0.3f, 0.3f);
                    }
                    heartImages[i].gameObject.SetActive(true);
                }
            }
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
