using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;
    private bool playerNearby = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            TryOpenChest();
        }
    }

    void TryOpenChest()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.UseKey())
        {
            isOpen = true;
            animator.SetBool("IsOpen", true);
            Debug.Log("Baú aberto com a chave!");

            // Som de abrir baú
            SFXManager.Instance?.Play("chest_open");
        }
        else
        {
            Debug.Log("Você precisa de uma chave!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}