using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 1;
    public Vector2 forcedDirection = Vector2.zero;
    public bool homing = false;
    private Vector2 direction;
    private Transform playerRef;

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;

        if (forcedDirection != Vector2.zero)
            direction = forcedDirection.normalized;
        else
            direction = (playerRef.position - transform.position).normalized;

        Vector3 s = transform.localScale;
        float sign = direction.x < 0 ? -1f : 1f;
        transform.localScale = new Vector3(sign * Mathf.Abs(s.x), s.y, s.z);

        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (homing && playerRef != null)
            direction = Vector2.Lerp(direction, (playerRef.position - transform.position).normalized, 5f * Time.deltaTime);

        transform.Translate(direction * speed * Time.deltaTime);
    }   

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerHealth hp = col.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (col.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}