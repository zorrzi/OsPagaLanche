using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButtonObject;
    [SerializeField] private Image pauseButtonImage;

    [Header("Sprites")]
    [SerializeField] private Sprite hamburgerSprite;
    [SerializeField] private Sprite closeSprite;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        ResumeGame();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (LevelTimer.Instance != null)
            LevelTimer.Instance.PauseTimer();

        if (pauseButtonObject != null)
            pauseButtonObject.SetActive(false);

        if (pauseButtonImage != null && closeSprite != null)
            pauseButtonImage.sprite = closeSprite;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (LevelTimer.Instance != null)
            LevelTimer.Instance.ResumeTimer();

        if (pauseButtonObject != null)
            pauseButtonObject.SetActive(true);

        if (pauseButtonImage != null && hamburgerSprite != null)
            pauseButtonImage.sprite = hamburgerSprite;
    }

    public void RestartLevel()
    {
        if (LevelTimer.Instance != null && GameData.Instance != null)
        {
            GameData.Instance.accumulatedTime = LevelTimer.Instance.CurrentTime;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.accumulatedTime = 0f;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}