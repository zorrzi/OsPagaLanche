using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Dados do Jogador")]
    public string playerName = "";
    public int selectedCharacterIndex = 0;

    [Header("Tempo da Fase")]
    public float lastLevelTime = 0f;

    void Awake()
    {
        // Singleton persistente entre cenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        Debug.Log($" Nome do jogador: {name}");
    }

    public void SetCharacter(int index)
    {
        selectedCharacterIndex = index;
        Debug.Log($"Personagem selecionado: índice {index}");
    }

    public void SetLevelTime(float time)
    {
        lastLevelTime = time;
    }
}