using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                PlayerHealth hp = col.GetComponent<PlayerHealth>();
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}