using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text titleText;
    public Button playButton;
    public Button quitButton;

    [Header("Animation")]
    public float titleBobSpeed = 1.5f;
    public float titleBobAmount = 10f;

    private Vector3 titleStartPos;

    private void Start()
    {
        // Asegurar que no haya GameManager previo con datos
        // (se reiniciará al cargar GameLevel)

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (titleText != null)
            titleStartPos = titleText.transform.localPosition;

        // Desbloquear cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Animación suave del título
        if (titleText != null)
        {
            float newY = titleStartPos.y + Mathf.Sin(Time.time * titleBobSpeed) * titleBobAmount;
            titleText.transform.localPosition = new Vector3(titleStartPos.x, newY, titleStartPos.z);
        }
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("GameLevel");
    }

    private void OnQuitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
