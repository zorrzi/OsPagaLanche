using UnityEngine;

public class HamburgerProjectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 10f;
    public Vector2 direction = Vector2.right;

    [Header("Dano")]
    public int damage = 1;

    void Start()
    {
        // Espelha o sprite na direção certa
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

        // Acertou o chão/parede
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