using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class BossFoodTruck : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  INSPECTOR
    // ─────────────────────────────────────────

    [Header("Movimento")]
    public float speed = 2f;
    public float speedPhase2 = 3.8f;

    [Header("Ranges")]
    public float attackRange = 5f;
    public float detectionRange = 12f;

    [Header("Attack 1 – Tomate")]
    public GameObject tomatoPrefab;         // projétil de tomate
    public Transform tomatoFirePoint;      // ponto de spawn do tomate
    public float attack1Cooldown = 2.5f;

    [Header("Attack 2 – Fumaça")]
    public GameObject smokePuffPrefab;      // projétil/área de fumaça
    public Transform smokeFirePoint;       // saída do escapamento
    public float attack2Cooldown = 6f;
    public int smokePuffCount = 4;  // quantas bolinhas de fumaça
    public float smokeSpreadAngle = 30f;

    [Header("Fase 2 – Cooldowns reduzidos")]
    public float attack1CooldownPhase2 = 1.4f;
    public float attack2CooldownPhase2 = 3.5f;

    // ─────────────────────────────────────────
    //  PRIVADOS
    // ─────────────────────────────────────────

    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth healthSystem;
    private Transform player;

    private bool isPhase2 = false;
    private float attack1Timer = 0f;
    private float attack2Timer = 0f;
    private float currentSpeed;
    private Vector3 baseScale;
    private float facingDir = 1f;

    // Animator hashes (evita GC de string lookup)
    private static readonly int HASH_WALK = Animator.StringToHash("isWalking");
    private static readonly int HASH_ATK1 = Animator.StringToHash("isAttack1");
    private static readonly int HASH_ATK2 = Animator.StringToHash("isAttack2");
    private static readonly int HASH_HIT = Animator.StringToHash("isHit");
    private static readonly int HASH_DEAD = Animator.StringToHash("isDead");
    private static readonly int HASH_PHASE2 = Animator.StringToHash("isPhase2");

    // ─────────────────────────────────────────
    //  UNITY CALLBACKS
    // ─────────────────────────────────────────

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<EnemyHealth>();

        currentSpeed = speed;
        baseScale = transform.localScale;

        healthSystem.OnDamaged += OnBossDamaged;
        healthSystem.OnDeath += OnBossDeath;
    }

    void OnDestroy()
    {
        if (healthSystem == null) return;
        healthSystem.OnDamaged -= OnBossDamaged;
        healthSystem.OnDeath -= OnBossDeath;
    }

    void Update()
    {
        if (healthSystem.IsDead) return;

        // Lazy player lookup
        if (player == null)
        {
            var obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
            else return;
        }

        // DEBUG – remover em build final
        if (Input.GetKeyDown(KeyCode.H)) healthSystem.TakeDamage(1);

        float dist = Vector2.Distance(transform.position, player.position);
        attack1Timer -= Time.deltaTime;
        attack2Timer -= Time.deltaTime;

        FlipToPlayer();

        if (dist <= attackRange)
        {
            StopMovement();
            TryAttack();
        }
        else if (dist <= detectionRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    // ─────────────────────────────────────────
    //  MOVIMENTO / ESTADO
    // ─────────────────────────────────────────

    void Chase()
    {
        anim.SetBool(HASH_WALK, true);
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * currentSpeed, rb.linearVelocity.y);
    }

    void Idle()
    {
        anim.SetBool(HASH_WALK, false);
        StopMovement();
    }

    void StopMovement()
    {
        anim.SetBool(HASH_WALK, false);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void FlipToPlayer()
    {
        if (player == null) return;
        float dir = player.position.x < transform.position.x ? -1f : 1f;
        if (dir == facingDir) return;
        facingDir = dir;
        transform.localScale = new Vector3(
            facingDir * Mathf.Abs(baseScale.x),
            baseScale.y,
            baseScale.z
        );
    }

    // ─────────────────────────────────────────
    //  LÓGICA DE ATAQUE
    // ─────────────────────────────────────────

    void TryAttack()
    {
        // Fase 2: prioriza ataque 2 (fumaça) se disponível
        if (isPhase2 && attack2Timer <= 0f)
        {
            DoAttack2();
            return;
        }
        if (attack1Timer <= 0f)
        {
            DoAttack1();
        }
    }

    /// <summary>Attack 1 – Arremessa tomate em direção ao player.</summary>
    void DoAttack1()
    {
        anim.SetTrigger(HASH_ATK1);
        attack1Timer = attack1Cooldown;
        // Delay para sincronizar com o frame de arremesso da animação
        Invoke(nameof(SpawnTomato), 0.35f);
    }

    /// <summary>Attack 2 – Cospe fumaça em leque (somente Fase 2).</summary>
    void DoAttack2()
    {
        anim.SetTrigger(HASH_ATK2);
        attack2Timer = attack2Cooldown;
        // Animação tem "carga" mais longa, delay maior
        Invoke(nameof(SpawnSmoke), 0.6f);
    }

    // ─────────────────────────────────────────
    //  SPAWN DE PROJÉTEIS
    // ─────────────────────────────────────────

    void SpawnTomato()
    {
        if (healthSystem.IsDead || player == null) return;
        if (tomatoPrefab == null || tomatoFirePoint == null) return;

        GameObject proj = Instantiate(tomatoPrefab, tomatoFirePoint.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.forcedDirection = (player.position - tomatoFirePoint.position).normalized;
    }

    void SpawnSmoke()
    {
        if (healthSystem.IsDead) return;
        if (smokePuffPrefab == null || smokeFirePoint == null) return;

        float baseAngle = facingDir > 0 ? 0f : 180f;

        for (int i = 0; i < smokePuffCount; i++)
        {
            float t = smokePuffCount == 1 ? 0f : (i / (float)(smokePuffCount - 1)) - 0.5f;
            float angle = baseAngle + t * smokeSpreadAngle;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject proj = Instantiate(smokePuffPrefab, smokeFirePoint.position, Quaternion.identity);
            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
                p.forcedDirection = dir;
        }
    }

    // ─────────────────────────────────────────
    //  EVENTOS DE SAÚDE
    // ─────────────────────────────────────────

    private void OnBossDamaged(int currentHP, int maxHP)
    {
        anim.SetTrigger(HASH_HIT);

        if (!isPhase2 && currentHP <= maxHP / 2)
            EnterPhase2();
    }

    private void OnBossDeath()
    {
        anim.SetTrigger(HASH_DEAD);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        CancelInvoke();
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        currentSpeed = speedPhase2;
        attack1Cooldown = attack1CooldownPhase2;
        attack2Cooldown = attack2CooldownPhase2;
        anim.SetBool(HASH_PHASE2, true);
        Debug.Log("[BossFoodTruck] Fase 2 ativada!");
    }

    // ─────────────────────────────────────────
    //  ANIMATION EVENTS (opcional, mais preciso)
    // ─────────────────────────────────────────

    /// <summary>
    /// Alternativa ao Invoke: adicione um Animation Event no frame exato
    /// do arremesso/cuspida e chame estes métodos diretamente.
    /// </summary>
    public void AnimEvent_SpawnTomato() => SpawnTomato();
    public void AnimEvent_SpawnSmoke() => SpawnSmoke();
}