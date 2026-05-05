using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    [Header("Prefab base do player (compartilhado)")]
    public GameObject playerPrefab;

    [Header("Personagens disponíveis")]
    public CharacterData[] characters;

    public CharacterData GetCharacter(int index)
    {
        if (index < 0 || index >= characters.Length)
        {
            Debug.LogError($"Índice de personagem inválido: {index}");
            return null;
        }
        return characters[index];
    }
}