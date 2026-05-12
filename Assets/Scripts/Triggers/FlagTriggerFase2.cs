using UnityEngine;
using UnityEngine.SceneManagement;

namespace Triggers
{
    public class FlagTriggerFase2 : MonoBehaviour
    {
        [Header("Configuração")]
        [Tooltip("Se marcado, exige que o boss tenha sido derrotado para ativar.")]
        public bool requireBossDefeated = true;

        private bool triggered = false;

        void Start()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && !col.isTrigger)
            {
                Debug.LogWarning($"FlagTriggerFase2 '{name}': marcando IsTrigger automaticamente.");
                col.isTrigger = true;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggered) return;

            // Verifica se o boss foi derrotado, se necessário
            if (requireBossDefeated)
            {
                if (BossTracker.Instance == null || !BossTracker.Instance.BossDefeated)
                {
                    Debug.Log("Você precisa derrotar o boss antes de prosseguir!");
                    return;
                }
            }

            triggered = true;
            Debug.Log("Bandeira tocada! Indo para a fase 3...");

            // Congela o player
            FreezePlayer(other.gameObject);

            // Carrega a próxima cena (Fase 3)
            SceneManager.LoadScene("Fase3");
        }

        /// <summary>
        /// Para o player de se mover e impede que caia até a próxima fase carregar.
        /// </summary>
        void FreezePlayer(GameObject player)
        {
            // Desabilita o PlayerMovement (controles e movimento)
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;

            // Para a física (zera velocidade e congela)
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic; // sem gravidade nem colisões dinâmicas
            }

            // Para a animação no IdleSide (se quiser deixar parado, não em loop de corrida)
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
}