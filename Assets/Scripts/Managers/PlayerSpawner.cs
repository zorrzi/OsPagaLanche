using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Referências")]
    public CharacterDatabase characterDatabase;
    public Transform spawnPoint;

    void Start()
    {
        Debug.Log("PlayerSpawner.Start() executou!");
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (characterDatabase == null)
        {
            Debug.LogError("CharacterDatabase não atribuído!");
            return;
        }

        int charIndex = 0;
        if (GameData.Instance != null)
        {
            charIndex = GameData.Instance.selectedCharacterIndex;
        }

        CharacterData charData = characterDatabase.GetCharacter(charIndex);
        if (charData == null)
        {
            Debug.LogError($"Personagem {charIndex} não encontrado!");
            return;
        }

        // Instancia o prefab do player
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject playerInstance = Instantiate(characterDatabase.playerPrefab, spawnPos, Quaternion.identity);
        playerInstance.name = "Player";

        // Aplica o Animator Controller do personagem escolhido
        Animator anim = playerInstance.GetComponent<Animator>();
        if (anim != null && charData.animatorController != null)
        {
            anim.runtimeAnimatorController = charData.animatorController;
        }

        // Avisa a câmera quem é o novo target
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            Rigidbody2D playerRb = playerInstance.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                cam.SetTarget(playerRb);
            }
        }

        Debug.Log($"Spawnou: {charData.characterName}");

        // Restaura estado (vidas, chaves, munição) do GameData
        StartCoroutine(RestoreStateNextFrame(playerInstance));
    }

    IEnumerator RestoreStateNextFrame(GameObject playerInstance)
    {
        yield return null;

        Debug.Log($"GameData.Instance é null? {GameData.Instance == null}");

        if (GameData.Instance == null)
        {
            Debug.LogWarning("Saindo: GameData.Instance é null");
            yield break;
        }

        Debug.Log($"hasGameStarted? {GameData.Instance.hasGameStarted}");
        Debug.Log($"currentLives? {GameData.Instance.currentLives}");
        Debug.Log($"currentKeys? {GameData.Instance.currentKeys}");
        Debug.Log($"currentAmmo? {GameData.Instance.currentAmmo}");


        PlayerHealth health = playerInstance.GetComponent<PlayerHealth>();
        PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();

        Debug.Log($"health é null? {health == null} | inventory é null? {inventory == null}");

        // Restaura vidas
        if (health != null && GameData.Instance.currentLives > 0)
        {
            health.currentLives = GameData.Instance.currentLives;
            health.UpdateHeartsUI();
            Debug.Log($"Vidas restauradas: {GameData.Instance.currentLives}");
        }

        // Restaura chaves
        if (inventory != null && GameData.Instance.currentKeys > 0)
        {
            for (int i = 0; i < GameData.Instance.currentKeys; i++)
            {
                inventory.AddKey();
            }
            Debug.Log($"Chaves restauradas: {GameData.Instance.currentKeys}");
        }

        // Restaura munição
        if (inventory != null && GameData.Instance.currentAmmo > 0)
        {
            inventory.SetAmmoSilent(GameData.Instance.currentAmmo);
            Debug.Log($"Munição restaurada: {GameData.Instance.currentAmmo}");
        }
    }
}