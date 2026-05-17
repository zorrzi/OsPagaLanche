using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

[RequireComponent(typeof(Collider2D))]
public class FlagTrigger3 : MonoBehaviour
{
    [Header("Configura��o")]
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
                Debug.Log("Voc� precisa derrotar o boss antes de prosseguir!");
                return;
            }
        }

        triggered = true;
        Debug.Log("Bandeira tocada! Completando fase...");

        // Congela o player
        FreezePlayer(other.gameObject);

        if (LevelManager.Instance != null)
            LevelManager.Instance.CompleteLevel();
        else
            Debug.LogWarning("LevelManager n�o encontrado na cena!");
    }

    /// <summary>
    /// Para o player de se mover e impede que caia at� a pr�xima fase carregar.
    /// </summary>
    void FreezePlayer(GameObject player)
    {
        // Desabilita o PlayerMovement (controles e movimento)
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        // Para a f�sica (zera velocidade e congela)
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // sem gravidade nem colis�es din�micas
        }

        // Para a anima��o no IdleSide (se quiser deixar parado, n�o em loop de corrida)
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsClimbing", false);
        }

        Debug.Log("Player congelado.");
    }
}