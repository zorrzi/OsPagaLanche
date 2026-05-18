using UnityEngine;

public class HamburgerProjectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 10f;
    public Vector2 direction = Vector2.right;

    [Header("Dano")]
    public int damage = 1;
    
    [Header("Timeout")]
    // Lifetime in seconds before projectile is automatically destroyed
    public float lifetime = 0.5f;

    void Start()
    {
        // Destroy after lifetime seconds to avoid lingering projectiles
        if (lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
        // Espelha o sprite na dire��o certa
        if (direction.x < 0)
        {
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(-Mathf.Abs(s.x), s.y, s.z);
        }
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Acertou um inimigo
        if (col.CompareTag("Enemy"))
        {
            EnemyHealth enemy = col.GetComponent<EnemyHealth>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

        // Acertou o ch�o/parede
        if (col.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}