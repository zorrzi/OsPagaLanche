using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Configura��o da fase atual")]
    [Tooltip("Cena pra carregar quando completar essa fase. Deixa vazio se for a �ltima.")]
    public string nextSceneName = "";

    [Tooltip("Se marcado, ao completar essa fase vai pra cena de Vit�ria")]
    public bool isLastLevel = false;

    [Tooltip("Nome da cena de vit�ria (ex: 'Victory' ou 'Leaderboard')")]
    public string victorySceneName = "GameOverScene";

    [Header("Delay")]
    public float transitionDelay = 1.5f;

    [Header("Leaderboard")]
    [Tooltip("Se marcado, envia a run para o leaderboard ao completar a fase.")]
    public bool submitRunOnComplete = true;

    void Awake()
    {
        Instance = this;
    }

    public void CompleteLevel()
    {
        Debug.Log("Fase completada!");

        // Salva o estado do player (vidas + chaves) antes de trocar de cena
        SavePlayerState();

        // Toca som de vit�ria
        SFXManager.Instance?.Play("level_complete");

        // Para o cron�metro
        if (LevelTimer.Instance != null)
            LevelTimer.Instance.StopTimer();

        SubmitLeaderboardRun();

        StartCoroutine(TransitionAfterDelay());
    }

    void SavePlayerState()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        int lives = health != null ? health.currentLives : 3;
        int keys = inventory != null ? inventory.keys : 0;
        float currentTime = LevelTimer.Instance != null ? LevelTimer.Instance.CurrentTime : 0f;

        if (GameData.Instance != null)
            GameData.Instance.SaveStateBeforeLevelChange(lives, keys, currentTime);
    }

    IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(transitionDelay);

        string targetScene = isLastLevel ? victorySceneName : nextSceneName;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("Nenhuma pr�xima cena configurada!");
            yield break;
        }

        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade(targetScene);
        else
            SceneManager.LoadScene(targetScene);
    }

    void SubmitLeaderboardRun()
    {
        if (!submitRunOnComplete) return;
        if (GameData.Instance == null) return;

        string username = GameData.Instance.playerName;
        if (string.IsNullOrWhiteSpace(username)) return;

        int durationSeconds = 0;
        if (LevelTimer.Instance != null)
            durationSeconds = Mathf.RoundToInt(LevelTimer.Instance.CurrentTime);

        LeaderboardApiClient client = LeaderboardApiClient.EnsureInstance();
        client.SubmitRun(username, durationSeconds, 0, (ok, error) =>
        {
            if (!ok && !string.IsNullOrEmpty(error))
                Debug.LogWarning($"Falha ao enviar leaderboard: {error}");
        });
    }
}