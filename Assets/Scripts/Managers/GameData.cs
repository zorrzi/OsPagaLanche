using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Dados do Jogador")]
    public string playerName = "";
    public int selectedCharacterIndex = 0;

    [Header("Tempo")]
    public float lastLevelTime = 0f;
    public float totalGameTime = 0f; // soma de todas as fases

    [Header("Estado entre fases")]
    public int currentLives = 3;
    public int currentKeys = 0;
    public bool hasGameStarted = false; // pra saber se acabou de começar (vidas cheias)

    void Awake()
    {
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
        Debug.Log($"Nome do jogador: {name}");
    }

    public void SetCharacter(int index)
    {
        selectedCharacterIndex = index;
        Debug.Log($"Personagem selecionado: índice {index}");
    }

    /// <summary>
    /// Reseta tudo pra começar um novo jogo (chamado no início, na MainMenu/CharacterSelect)
    /// </summary>
    public void ResetGameProgress()
    {
        currentLives = 3;
        currentKeys = 0;
        totalGameTime = 0f;
        lastLevelTime = 0f;
        hasGameStarted = true;
        Debug.Log("Progresso do jogo resetado");
    }

    /// <summary>
    /// Salva estado atual antes de mudar de fase
    /// </summary>
    public void SaveStateBeforeLevelChange(int lives, int keys)
    {
        currentLives = lives;
        currentKeys = keys;
        Debug.Log($"Estado salvo: {lives} vidas, {keys} chaves");
    }
}