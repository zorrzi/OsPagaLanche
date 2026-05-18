using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 3f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public float coyoteTime = 0.15f;

    [Header("Ataque Ranged")]
    public GameObject hamburgerPrefab;
    public Transform firePoint;
    public KeyCode rangedKey = KeyCode.K;
    public float rangedCooldown = 0.6f;

    [Header("Ataque Melee")]
    public Transform meleePoint;
    public Vector2 meleeBoxSize = new Vector2(1.5f, 1f);
    public int meleeDamage = 1;
    public float meleeCooldown = 0.5f;
    public LayerMask enemyLayer;
    public KeyCode meleeKey = KeyCode.J;

    private float coyoteTimeCounter = 0f;
    private float rangedCooldownTimer = 0f;
    private float meleeCooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInventory inventory;
    private bool isGrounded = false;
    private bool wasGrounded = false;
    private bool isOnLadder = false;
    private bool jumpRequested = false;
    private Vector2 movementVel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        // GROUND CHECK
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        Vector2 origin = (Vector2)transform.position + col.offset + Vector2.down * (col.size.y / 2);
        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            new Vector2(col.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        wasGrounded = isGrounded;
        isGrounded = hit.collider != null && hit.collider.CompareTag("Ground");

        if (isGrounded && !wasGrounded)
            SFXManager.Instance?.Play("land");

        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        if (rangedCooldownTimer > 0f) rangedCooldownTimer -= Time.deltaTime;
        if (meleeCooldownTimer > 0f) meleeCooldownTimer -= Time.deltaTime;

        // INPUT MOVIMENTO
        float moveInput = Input.GetAxisRaw("Horizontal");
        movementVel.x = moveInput * speed;

        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

        anim.SetFloat("Speed", isOnLadder ? 0f : Mathf.Abs(moveInput));
        anim.SetBool("IsJumping", !isGrounded && !isOnLadder);
        anim.SetBool("IsClimbing", isOnLadder);

        // PULO
        if (Input.GetKeyDown(KeyCode.Space) && (coyoteTimeCounter > 0f || isOnLadder))
        {
            SFXManager.Instance?.Play("jump");
            jumpRequested = true;
            coyoteTimeCounter = 0f;
            isOnLadder = false;
        }

        // ESCADA
        if (isOnLadder)
        {
            float climbInput = Input.GetAxisRaw("Vertical");
            movementVel.y = climbInput * climbSpeed;
        }
        else
        {
            movementVel.y = rb.linearVelocity.y;
        }

        // ATAQUE MELEE
        if (Input.GetKeyDown(meleeKey) && meleeCooldownTimer <= 0f)
        {
            SFXManager.Instance?.Play("attack_melee");
            anim.SetTrigger("AttackMelee");
            meleeCooldownTimer = meleeCooldown;
        }

        // ATAQUE RANGED — só se tiver munição
        if (Input.GetKeyDown(rangedKey) && rangedCooldownTimer <= 0f)
        {
            if (inventory != null && inventory.HasAmmo())
            {
                SFXManager.Instance?.Play("attack_ranged");
                anim.SetTrigger("AttackRanged");
                rangedCooldownTimer = rangedCooldown;
            }
            else
            {
                Debug.Log("Sem lanches! Abra baús pra conseguir munição.");
                // Opcional: som de "click vazio" pra feedback
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementVel;
        rb.gravityScale = isOnLadder ? 0f : 3f;

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    public void Shoot()
    {
        if (hamburgerPrefab == null || firePoint == null) return;

        // Consome munição
        if (inventory != null)
        {
            if (!inventory.UseAmmo()) return; // segurança extra
        }

        GameObject proj = Instantiate(hamburgerPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        HamburgerProjectile p = proj.GetComponent<HamburgerProjectile>();
        if (p != null) p.direction = dir;
    }

    public void MeleeAttack()
    {
        if (meleePoint == null) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(meleePoint.position, meleeBoxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.TakeDamage(meleeDamage);
                    SFXManager.Instance?.Play("hit_enemy");
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) isOnLadder = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) isOnLadder = false;
    }

    void OnDrawGizmosSelected()
    {
        if (meleePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(meleePoint.position, meleeBoxSize);
    }
}