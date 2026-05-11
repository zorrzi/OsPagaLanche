using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class BurguerEnemy : MonoBehaviour
{
    [Header("Configura��es")]
    public float speed = 2f;
    public float attackRange = 3f;
    public float detectionRange = 6f;

    [Header("Proj�til")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Animator anim;
    private Transform player;
    private EnemyHealth healthSystem;
    private Rigidbody2D rb;
    private Vector3 baseScale;
    private float facingDirection = 1f;
    private float attackTimer = 0f;
    private float attackCooldown = 2f;

    void Start()
    {
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (healthSystem.IsDead) return;

        // Busca o player se ainda n�o foi encontrado (lazy lookup)
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                return; // sem player ainda, espera o pr�ximo frame
        }

        float dist = Vector2.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        if (dist <= attackRange)
        {
            anim.SetBool("isWalking", false);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (attackTimer <= 0f)
            {
                anim.SetTrigger("isAttacking");
                Invoke("SpawnProjectile", 0.4f);
                attackTimer = attackCooldown;
            }
        }
        else if (dist <= detectionRange)
        {
            anim.SetBool("isWalking", true);
            float direction = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            facingDirection = direction;
            transform.localScale = new Vector3(
                facingDirection * Mathf.Abs(baseScale.x),
                baseScale.y,
                baseScale.z
            );
        }
        else
        {
            anim.SetBool("isWalking", false);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void SpawnProjectile()
    {
        if (healthSystem.IsDead) return;
        if (projectilePrefab != null && firePoint != null)
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }
}