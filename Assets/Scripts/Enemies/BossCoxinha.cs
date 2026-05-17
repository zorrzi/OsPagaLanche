using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class BossCoxinha : MonoBehaviour
{

    [Header("Movimento")]
    public float speed = 2.5f;
    public float speedPhase2 = 4f;

    [Header("Ranges")]
    public float attackRange = 4f;
    public float detectionRange = 12f;

    [Header("Intervalo entre ataques")]
    public float attackDelay = 2f;

    [Header("Attack 1 – Tomate")]
    public GameObject tomatoPrefab;
    public Transform tomatoFirePoint;
    public float attack1Cooldown = 2f;

    [Header("Attack 2 – Fumaça")]
    public GameObject smokePrefab;
    public Transform smokeFirePoint;
    public float attack2Cooldown = 6f;

    [Header("Fase 2 – Cooldowns reduzidos")]
    public float attack1CooldownPhase2 = 1.2f;
    public float attack2CooldownPhase2 = 3.5f;

    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth healthSystem;
    private Transform player;

    private bool isPhase2 = false;
    private float attack1Timer = 0f;
    private float attack2Timer = 0f;
    private float attackDelayTimer = 0f;
    private float currentSpeed;
    private Vector3 baseScale;
    private float facingDir = 1f;

    private static readonly int HASH_WALK = Animator.StringToHash("isWalking");
    private static readonly int HASH_ATK1 = Animator.StringToHash("isAttack1");
    private static readonly int HASH_ATK2 = Animator.StringToHash("isAttack2");
    private static readonly int HASH_HIT = Animator.StringToHash("isHit");
    private static readonly int HASH_DEAD = Animator.StringToHash("isDead");
    private static readonly int HASH_PHASE2 = Animator.StringToHash("isPhase2");

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

        if (player == null)
        {
            var obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
            else return;
        }

        if (Input.GetKeyDown(KeyCode.J)) healthSystem.TakeDamage(1);

        float dist = Vector2.Distance(transform.position, player.position);
        attack1Timer -= Time.deltaTime;
        attack2Timer -= Time.deltaTime;
        attackDelayTimer -= Time.deltaTime;

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

    void TryAttack()
    {
        if (attackDelayTimer > 0f) return;

        if (attack1Timer <= 0f && attack2Timer <= 0f)
        {
            // 20% fumaça, 80% tomate
            if (Random.value < 0.2f)
                DoAttack2();
            else
                DoAttack1();
        }
        else if (attack1Timer <= 0f)
        {
            DoAttack1();
        }
        else if (attack2Timer <= 0f)
        {
            DoAttack2();
        }
    }

    void DoAttack1()
    {
        anim.SetTrigger(HASH_ATK1);
        attack1Timer = attack1Cooldown;
        attackDelayTimer = attackDelay;
        Invoke(nameof(SpawnTomato), 0.35f);
    }

    void DoAttack2()
    {
        anim.SetTrigger(HASH_ATK2);
        attack2Timer = attack2Cooldown;
        attackDelayTimer = attackDelay;
        Invoke(nameof(SpawnSmoke), 0.6f);
    }

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
        if (healthSystem.IsDead || player == null) return;
        if (smokePrefab == null || smokeFirePoint == null) return;

        GameObject proj = Instantiate(smokePrefab, smokeFirePoint.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.forcedDirection = (player.position - smokeFirePoint.position).normalized;
    }

    private void OnBossDamaged(int currentHP, int maxHP)
    {
        // Se o boss já morreu (este dano foi o fatal), não dispara o Hit
        // para não interromper a animação de Death
        if (healthSystem.IsDead) return;

        anim.SetTrigger(HASH_HIT);

        if (!isPhase2 && currentHP <= maxHP / 2)
            EnterPhase2();
    }

    private void OnBossDeath()
    {
        // Reseta o trigger de Hit para evitar que ele dispare logo após o Death
        anim.ResetTrigger(HASH_HIT);
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
        Debug.Log("[BossCoxinha] Fase 2 ativada!");
    }

    public void AnimEvent_SpawnTomato() => SpawnTomato();
    public void AnimEvent_SpawnSmoke() => SpawnSmoke();
}