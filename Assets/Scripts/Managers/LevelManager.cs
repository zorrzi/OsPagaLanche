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
}