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

    // ---------------- MOBILE ----------------
    // Input de TOUCH (escrito pelos botoes da tela via TouchControlsBridge).
    // Separado do teclado para nao haver sobrescrita.
    private float touchH = 0f;
    private float touchV = 0f;

    // Input "final" que o resto do codigo le. Combinacao de teclado + touch.
    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    // Botoes de acao (event-based, padrao OR-like, nao sao sobrescritos).
    private bool jumpButtonDown = false;
    private bool meleeButtonDown = false;
    private bool rangedButtonDown = false;
    // ----------------------------------------

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

    // ---------------- API publica chamada pelos botoes touch via Bridge ----------------
    public void SetHorizontal(float value) => touchH = value;
    public void SetVertical(float value) => touchV = value;
    public void PressJump() => jumpButtonDown = true;
    public void PressMelee() => meleeButtonDown = true;
    public void PressRanged() => rangedButtonDown = true;
    // -----------------------------------------------------------------------------------

    void Update()
    {
        // ---------- Combinar teclado (Editor/PC) + touch (Mobile) ----------
        float kbH = 0f, kbV = 0f;
#if UNITY_EDITOR || UNITY_STANDALONE
        kbH = Input.GetAxisRaw("Horizontal");
        kbV = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) jumpButtonDown = true;
        if (Input.GetKeyDown(meleeKey)) meleeButtonDown = true;
        if (Input.GetKeyDown(rangedKey)) rangedButtonDown = true;
#endif
        // Teclado tem prioridade quando pressionado, senao usa o touch.
        // Assim os botoes da tela NAO sao sobrescritos pelo teclado parado.
        horizontalInput = Mathf.Abs(kbH) > 0.01f ? kbH : touchH;
        verticalInput = Mathf.Abs(kbV) > 0.01f ? kbV : touchV;
        // -------------------------------------------------------------------

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
        float moveInput = horizontalInput;
        movementVel.x = moveInput * speed;

        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

        anim.SetFloat("Speed", isOnLadder ? 0f : Mathf.Abs(moveInput));
        anim.SetBool("IsJumping", !isGrounded && !isOnLadder);
        anim.SetBool("IsClimbing", isOnLadder);

        // PULO
        if (jumpButtonDown && (coyoteTimeCounter > 0f || isOnLadder))
        {
            SFXManager.Instance?.Play("jump");
            jumpRequested = true;
            coyoteTimeCounter = 0f;
            isOnLadder = false;
        }

        // ESCADA
        if (isOnLadder)
        {
            float climbInput = verticalInput;
            movementVel.y = climbInput * climbSpeed;
        }
        else
        {
            movementVel.y = rb.linearVelocity.y;
        }

        // ATAQUE MELEE
        if (meleeButtonDown && meleeCooldownTimer <= 0f)
        {
            SFXManager.Instance?.Play("attack_melee");
            anim.SetTrigger("AttackMelee");
            meleeCooldownTimer = meleeCooldown;
        }

        // ATAQUE RANGED - so se tiver municao
        if (rangedButtonDown && rangedCooldownTimer <= 0f)
        {
            if (inventory != null && inventory.HasAmmo())
            {
                SFXManager.Instance?.Play("attack_ranged");
                anim.SetTrigger("AttackRanged");
                rangedCooldownTimer = rangedCooldown;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Sem lanches! Abra baus pra conseguir municao.");
#endif
            }
        }

        // Consome os "botoes apertados" (1 frame, igual GetKeyDown)
        jumpButtonDown = false;
        meleeButtonDown = false;
        rangedButtonDown = false;
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

        if (inventory != null)
        {
            if (!inventory.UseAmmo()) return;
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