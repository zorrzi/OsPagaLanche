using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

[RequireComponent(typeof(Collider2D))]
public class FlagTriggerPhase3 : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Se marcado, exige que o boss tenha sido derrotado para ativar.")]
    public bool requireBossDefeated = true;

    private bool triggered = false;

    void Start()
    {
        // Garante que o Collider2D está configurado como Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"FlagTriggerPhase3 '{name}': marcando IsTrigger automaticamente.");
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou no colisor é o jogador
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

        // Marca como ativado e carrega a cena de Game Over
        triggered = true;
        Debug.Log("Flag ativada! Carregando cena de Game Over...");
        SceneManager.LoadScene("GameOverScene"); // Certifique-se de que o nome da cena está correto
    }
}