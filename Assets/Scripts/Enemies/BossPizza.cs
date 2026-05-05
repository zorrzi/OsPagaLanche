using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class BossPizza : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 2f;
    public float speedPhase2 = 3.5f;

    [Header("Ranges")]
    public float attackRange = 4f;
    public float detectionRange = 10f;

    [Header("Projétil")]
    public GameObject pizzaSlicePrefab;
    public Transform firePoint;

    [Header("Cooldowns")]
    public float attack1Cooldown = 2f;
    public float attack2Cooldown = 5f;

    private Animator anim;
    private Transform player;
    private Rigidbody2D rb;
    private EnemyHealth healthSystem;
    private bool isPhase2 = false;
    private float attack1Timer = 0f;
    private float attack2Timer = 0f;
    private float currentSpeed;
    private Vector3 baseScale;
    private float facingDirection = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<EnemyHealth>();
        currentSpeed = speed;
        baseScale = transform.localScale;

        // Escuta quando o boss toma dano para verificar fase 2
        healthSystem.OnDamaged += OnBossDamaged;
    }

    void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnDamaged -= OnBossDamaged;
    }

    private void OnBossDamaged(int currentHP, int maxHP)
    {
        if (!isPhase2 && currentHP <= maxHP / 2)
        {
            isPhase2 = true;
            currentSpeed = speedPhase2;
            attack1Cooldown = 1.2f;
            attack2Cooldown = 3f;
            Debug.Log("Boss entrou na Fase 2!");
        }
    }

    void Update()
    {
        if (healthSystem.IsDead) return;

        // Busca o player se ainda não foi encontrado (lazy lookup)
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                return; // sem player ainda, não faz nada nesse frame
        }

        // TESTE - remover depois
        if (Input.GetKeyDown(KeyCode.J))
        {
            healthSystem.TakeDamage(1);
        }

        float dist = Vector2.Distance(transform.position, player.position);
        attack1Timer -= Time.deltaTime;
        attack2Timer -= Time.deltaTime;

        Vector2 dir = (player.position - transform.position).normalized;
        facingDirection = dir.x < 0 ? -1f : 1f;
        transform.localScale = new Vector3(
            facingDirection * Mathf.Abs(baseScale.x),
            baseScale.y,
            baseScale.z
        );

        if (dist <= attackRange)
        {
            anim.SetBool("isWalking", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (isPhase2 && attack2Timer <= 0f)
            {
                anim.SetTrigger("isAttack2");
                attack2Timer = attack2Cooldown;
            }
            else if (attack1Timer <= 0f)
            {
                anim.SetTrigger("isAttack1");
                Invoke("SpawnSlice", 0.3f);
                attack1Timer = attack1Cooldown;
            }
        }
        else if (dist <= detectionRange)
        {
            anim.SetBool("isWalking", true);
            float direction = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            anim.SetBool("isWalking", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void SpawnSlice()
    {
        if (healthSystem.IsDead || player == null) return;
        if (pizzaSlicePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(pizzaSlicePrefab, firePoint.position, Quaternion.identity);
            Projectile projScript = proj.GetComponent<Projectile>();
            if (projScript != null)
                projScript.forcedDirection = (player.position - firePoint.position).normalized;
        }
    }

    public void SpawnSpinSlices()
    {
        if (healthSystem.IsDead) return;
        if (pizzaSlicePrefab == null || firePoint == null) return;

        int sliceCount = 6;
        for (int i = 0; i < sliceCount; i++)
        {
            float angle = i * (360f / sliceCount);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject proj = Instantiate(pizzaSlicePrefab, firePoint.position, Quaternion.identity);
            Projectile projScript = proj.GetComponent<Projectile>();
            if (projScript != null)
                projScript.forcedDirection = dir;
        }
    }
}