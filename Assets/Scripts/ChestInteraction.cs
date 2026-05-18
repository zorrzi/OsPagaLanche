using UnityEngine;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    [Header("Drop")]
    [Tooltip("Quantas munições o lanche desse baú vai dar quando coletado")]
    public int ammoValuePerPickup = 10;

    [Tooltip("Prefab do lanche coletável que vai pular do baú")]
    public GameObject hamburgerPickupPrefab;

    [Tooltip("Força do pulo do lanche")]
    public Vector2 jumpForceMin = new Vector2(-1f, 5f);
    public Vector2 jumpForceMax = new Vector2(1f, 7f);

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
            Debug.Log($"Baú aberto! Lanche vale {ammoValuePerPickup} munições");

            SFXManager.Instance?.Play("chest_open");

            // Spawna um único lanche
            StartCoroutine(SpawnHamburgerDelayed());
        }
        else
        {
            Debug.Log("Você precisa de uma chave!");
        }
    }

    IEnumerator SpawnHamburgerDelayed()
    {
        // Espera a animação do baú começar
        yield return new WaitForSeconds(0.2f);

        SpawnHamburger();
    }

    void SpawnHamburger()
    {
        if (hamburgerPickupPrefab == null)
        {
            Debug.LogWarning("hamburgerPickupPrefab não atribuído no baú!");
            return;
        }

        Vector3 spawnPos = transform.position + new Vector3(0, 0.5f, 0);
        GameObject pickup = Instantiate(hamburgerPickupPrefab, spawnPos, Quaternion.identity);

        // Configura quanto vale esse lanche
        HamburgerPickup pickupScript = pickup.GetComponent<HamburgerPickup>();
        if (pickupScript != null)
            pickupScript.ammoValue = ammoValuePerPickup;

        // Aplica força aleatória pra "pular" do baú
        Rigidbody2D rb = pickup.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float forceX = Random.Range(jumpForceMin.x, jumpForceMax.x);
            float forceY = Random.Range(jumpForceMin.y, jumpForceMax.y);
            rb.linearVelocity = new Vector2(forceX, forceY);
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