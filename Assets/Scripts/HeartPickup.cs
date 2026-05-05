using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerHealth hp = col.GetComponent<PlayerHealth>();

            if (hp != null && hp.currentLives < hp.maxLives)
            {
                hp.AddLife();
                Destroy(gameObject);
            }
        }
    }
}