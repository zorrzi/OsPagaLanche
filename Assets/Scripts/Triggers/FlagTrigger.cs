using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlagTrigger : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Se marcado, exige que o boss tenha sido derrotado pra ativar.")]
    public bool requireBossDefeated = true;

    private bool triggered = false;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"FlagTrigger '{name}': marcando IsTrigger automaticamente.");
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;

        // Verifica se o boss foi derrotado
        if (requireBossDefeated)
        {
            if (BossTracker.Instance == null || !BossTracker.Instance.BossDefeated)
            {
                Debug.Log("Você precisa derrotar o boss antes de prosseguir!");
                return;
            }
        }

        triggered = true;
        Debug.Log("Bandeira tocada! Completando fase...");

        if (LevelManager.Instance != null)
            LevelManager.Instance.CompleteLevel();
        else
            Debug.LogWarning("LevelManager não encontrado na cena!");
    }
}