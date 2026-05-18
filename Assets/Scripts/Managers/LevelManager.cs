using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Configuração da fase atual")]
    [Tooltip("Cena pra carregar quando completar essa fase. Deixa vazio se for a última.")]
    public string nextSceneName = "";

    [Tooltip("Se marcado, ao completar essa fase vai pra cena de Vitória")]
    public bool isLastLevel = false;

    [Tooltip("Nome da cena de vitória (ex: 'Victory' ou 'Leaderboard')")]
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

        // Salva o estado do player (vidas + chaves + munição + tempo) antes de trocar de cena
        SavePlayerState();

        // Toca som de vitória
        SFXManager.Instance?.Play("level_complete");

        // Para o cronômetro
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
        int ammo = inventory != null ? inventory.ammo : 0; // ← NOVO: salva munição
        float currentTime = LevelTimer.Instance != null ? LevelTimer.Instance.CurrentTime : 0f;

        if (GameData.Instance != null)
            GameData.Instance.SaveStateBeforeLevelChange(lives, keys, currentTime, ammo); // ← passa ammo
    }

    IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(transitionDelay);

        string targetScene = isLastLevel ? victorySceneName : nextSceneName;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("Nenhuma próxima cena configurada!");
            yield break;
        }

        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade(targetScene);
        else
            SceneManager.LoadScene(targetScene);
    }

    void SubmitLeaderboardRun()
    {
        // IMPORTANTE: Run deve ser enviada APENAS no final de TUDO (após completar a última fase)
        // Nunca enviar em fases intermediárias
        if (!isLastLevel)
        {
            Debug.Log($"[LevelManager] Fase {SceneManager.GetActiveScene().name} completada, mas não é a última. Não enviando run ainda.");
            return;
        }

        if (!submitRunOnComplete)
        {
            Debug.Log("[LevelManager] Submissão de leaderboard desabilitada para esta fase.");
            return;
        }

        if (GameData.Instance == null)
        {
            Debug.LogError("[LevelManager] GameData.Instance é nulo! Não é possível submeter run.");
            return;
        }

        string username = GameData.Instance.playerName;
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("[LevelManager] Nome do jogador vazio! Não é possível submeter run.");
            return;
        }

        int durationSeconds = 0;
        if (LevelTimer.Instance != null)
            durationSeconds = Mathf.RoundToInt(LevelTimer.Instance.CurrentTime);

        Debug.Log($"JOGO FINALIZADO! Enviando run final: jogador='{username}', tempo total={durationSeconds}s");

        LeaderboardApiClient client = LeaderboardApiClient.EnsureInstance();
        client.SubmitRun(username, durationSeconds, 0, (ok, error) =>
        {
            if (ok)
            {
                Debug.Log($"[LevelManager] Run FINAL submetida com sucesso para '{username}'!");
            }
            else
            {
                Debug.LogWarning($"[LevelManager] ERRO ao submeter run final: {error}");
            }
        });
    }
}