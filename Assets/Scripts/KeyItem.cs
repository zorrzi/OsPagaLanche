using UnityEngine;
using UnityEngine.UI;

public class KeyItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey();
                Destroy(gameObject);
            }
        }
    }
}