using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Feedback")]
    [SerializeField] private float hurtDuration = 0.2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hurtColor = Color.red;

    [Header("Morte")]
    [SerializeField] private float destroyDelay = 0.8f;
    [SerializeField] private bool isBoss = false; // Marcar TRUE no boss

    private Animator animator;
    private Color originalColor;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public float HealthPercent => (float)currentHealth / maxHealth;

    public System.Action<int, int> OnDamaged;
    public System.Action OnDeath;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} levou {damage} de dano. HP: {currentHealth}/{maxHealth}");

        OnDamaged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Som de dano no inimigo
            SFXManager.Instance?.Play("enemy_damage");

            if (animator != null)
                animator.SetTrigger("isHurt");
            StartCoroutine(HurtFlash());
        }
    }

    private IEnumerator HurtFlash()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtDuration);
        if (!isDead) spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} morreu!");

        // Som de morte (diferente pra boss vs inimigo comum)
        if (isBoss)
            SFXManager.Instance?.Play("boss_death");
        else
            SFXManager.Instance?.Play("enemy_death");

        if (animator != null)
            animator.SetBool("isDead", true);
        OnDeath?.Invoke();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(gameObject, destroyDelay);
    }
}