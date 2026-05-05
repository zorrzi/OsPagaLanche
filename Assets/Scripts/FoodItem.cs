using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public enum FoodType { HotDog, BatateFrita, Refrigerante, Pizza }
    public FoodType foodType;
    public float duration = 6f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
            Destroy(gameObject);
        }
    }

    void ApplyEffect(GameObject player)
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();

        switch (foodType)
        {
            case FoodType.HotDog:
                Debug.Log("Hot Dog! Velocidade aumentada.");
                // pm.speed *= 1.5f — implementar depois
                break;
            case FoodType.BatateFrita:
                Debug.Log("Batata Frita! Invencibilidade.");
                break;
            case FoodType.Refrigerante:
                Debug.Log("Refrigerante! Especial cheio.");
                break;
            case FoodType.Pizza:
                Debug.Log("Pizza! +1 vida.");
                break;
        }
    }
}