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

    private bool interactButtonDown = false;
    public void PressInteract() => interactButtonDown = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool pressed = interactButtonDown;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.E)) pressed = true;
#endif

        if (playerNearby && pressed && !isOpen)
        {
            TryOpenChest();
        }

        // Consome o flag (verdadeiro por 1 frame, igual GetKeyDown)
        interactButtonDown = false;
    }

    void TryOpenChest()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.UseKey())
        {
            isOpen = true;
            animator.SetBool("IsOpen", true);
#if UNITY_EDITOR
            Debug.Log($"Bau aberto! Lanche vale {ammoValuePerPickup} municoes");
#endif

            SFXManager.Instance?.Play("chest_open");

            // Spawna um único lanche
            StartCoroutine(SpawnHamburgerDelayed());
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Voce precisa de uma chave!");
#endif
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
            Debug.LogWarning("hamburgerPickupPrefab nao atribuido no bau!");
            return;
        }

        Vector3 spawnPos = transform.position + new Vector3(0, 0.5f, 0);
        GameObject pickup = Instantiate(hamburgerPickupPrefab, spawnPos, Quaternion.identity);

        HamburgerPickup pickupScript = pickup.GetComponent<HamburgerPickup>();
        if (pickupScript != null)
            pickupScript.ammoValue = ammoValuePerPickup;

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