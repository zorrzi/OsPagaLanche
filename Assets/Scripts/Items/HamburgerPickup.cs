using UnityEngine;

public class HamburgerPickup : MonoBehaviour
{
    [Header("Configuração")]
    public int ammoValue = 10;
    public float lifetime = 30f;

    [Header("Animação visual")]
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.1f;

    [Tooltip("Tempo até desativar a colisão física com o player após spawnar")]
    public float ignorePlayerTime = 0.3f;

    private Vector3 baseScale;
    private bool hasLanded = false;

    void Start()
    {
        baseScale = transform.localScale;

        if (lifetime > 0)
            Destroy(gameObject, lifetime);

        // Ignora colisão física com o player (mantém só o trigger pra coletar)
        IgnorePlayerCollision();
    }

    void IgnorePlayerCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider2D[] myColliders = GetComponents<Collider2D>();
        Collider2D[] playerColliders = player.GetComponents<Collider2D>();

        foreach (Collider2D myCol in myColliders)
        {
            // Só ignora os colliders NÃO-trigger (físicos), mantém o trigger pra coletar
            if (myCol.isTrigger) continue;

            foreach (Collider2D playerCol in playerColliders)
            {
                Physics2D.IgnoreCollision(myCol, playerCol, true);
            }
        }
    }

    void Update()
    {
        if (hasLanded)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = baseScale * pulse;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground") && !hasLanded)
        {
            hasLanded = true;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.5f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.3f, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddAmmo(ammoValue);
            Destroy(gameObject);
        }
    }
}