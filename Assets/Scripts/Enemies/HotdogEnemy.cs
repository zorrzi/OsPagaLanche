using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class HotdogEnemy : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 2.5f;
    public float attackRange = 4f;
    public float detectionRange = 7f;

    [Header("Projétil")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Animator anim;
    private Transform player;
    private EnemyHealth healthSystem;
    private float attackTimer = 0f;
    private float attackCooldown = 1.5f;
    private Vector3 baseScale;
    private float facingDirection = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<EnemyHealth>();
        baseScale = transform.localScale;
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
                return; // sem player ainda, espera o próximo frame
        }

        float dist = Vector2.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        Vector2 dir = (player.position - transform.position).normalized;
        facingDirection = dir.x < 0 ? -1f : 1f;
        transform.localScale = new Vector3(facingDirection * Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);

        if (dist <= attackRange)
        {
            anim.SetBool("isWalking", false);
            if (attackTimer <= 0f)
            {
                anim.SetTrigger("isAttacking");
                Invoke("SpawnProjectile", 0.6f);
                attackTimer = attackCooldown;
            }
        }
        else if (dist <= detectionRange)
        {
            anim.SetBool("isWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void SpawnProjectile()
    {
        if (healthSystem.IsDead) return;
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projScript = proj.GetComponent<Projectile>();
            if (projScript != null)
                projScript.forcedDirection = new Vector2(facingDirection, 0);
        }
    }
}