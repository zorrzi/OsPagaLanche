using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI ammoText;

    [Header("Formato")]
    [Tooltip("Formato do texto. {0} é substituído pela quantidade.")]
    public string format = "x {0}";

    private PlayerInventory playerInventory;

    void Start()
    {
        // Busca o player na cena
        StartCoroutine(FindPlayerInventory());
    }

    System.Collections.IEnumerator FindPlayerInventory()
    {
        // Espera 1 frame pro player ser spawnado pelo PlayerSpawner
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("AmmoUI: Player não encontrado!");
            yield break;
        }

        playerInventory = player.GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogWarning("AmmoUI: PlayerInventory não encontrado!");
            yield break;
        }

        // Escuta mudanças de munição
        playerInventory.OnAmmoChanged += UpdateAmmoText;

        // Atualiza com o valor inicial
        UpdateAmmoText(playerInventory.ammo);
    }

    void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnAmmoChanged -= UpdateAmmoText;
    }

    void UpdateAmmoText(int amount)
    {
        if (ammoText != null)
            ammoText.text = string.Format(format, amount);
    }
}