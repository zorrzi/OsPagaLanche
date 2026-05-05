using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Referências")]
    public CharacterDatabase characterDatabase;
    public Transform spawnPoint;

    void Start()
    {
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
    }
}