using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class BurguerEnemy : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 2f;
    public float attackRange = 3f;
    public float detectionRange = 6f;

    [Header("Projétil")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Animator anim;
    private Transform player;
    private EnemyHealth healthSystem;
    private float attackTimer = 0f;
    private float attackCooldown = 2f;

    void Start()
    {
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<EnemyHealth>();
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

        if (dist <= attackRange)
        {
            anim.SetBool("isWalking", false);
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
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            if (dir.x < 0)
                transform.localScale = new Vector3(-0.5f, 0.5f, 1);
            else
                transform.localScale = new Vector3(0.5f, 0.5f, 1);
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
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }
}