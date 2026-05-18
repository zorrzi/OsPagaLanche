using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TomatoProjectile : MonoBehaviour
{
    [Header("Dano")]
    public int damage = 1;

    [Header("Trajetória")]
    [Tooltip("Altura do arco em unidades acima do ponto mais alto entre origem e alvo.")]
    public float arcHeight = 3f;

    [Tooltip("Offset vertical no alvo (0 = pés do player).")]
    public float targetYOffset = 0f;

    [Header("Auto-destruição")]
    public float maxLifetime = 5f;

    [Header("Rotação visual")]
    public float spinSpeed = 360f;

    private Rigidbody2D rb;
    private bool launched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }
    public void Launch(Vector2 target)
    {
        launched = true;

        Vector2 origin = transform.position;
        Vector2 finalTarget = target + new Vector2(0f, targetYOffset);

        float g = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;
        if (g <= 0f) g = 9.81f;

        float peakY = Mathf.Max(origin.y, finalTarget.y) + arcHeight;

        float vy = Mathf.Sqrt(2f * g * (peakY - origin.y));

        float tUp = vy / g;
        float tDown = Mathf.Sqrt(2f * (peakY - finalTarget.y) / g);
        float tTotal = tUp + tDown;

        float vx = (finalTarget.x - origin.x) / tTotal;

        rb.linearVelocity = new Vector2(vx, vy);

        Debug.Log($"[Tomato] origin={origin} target={finalTarget} g={g} peakY={peakY} tTotal={tTotal:F3} v=({vx:F2}, {vy:F2})");
    }

    void Update()
    {
        if (!launched) return;

        if (spinSpeed != 0f)
        {
            float dir = rb.linearVelocity.x >= 0f ? -1f : 1f;
            transform.Rotate(0f, 0f, dir * spinSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!launched) return;

        if (col.CompareTag("Player"))
        {
            PlayerHealth hp = col.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (col.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}