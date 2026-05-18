using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Dados do Jogador")]
    public string playerName = "";
    public int selectedCharacterIndex = 0;

    [Header("Tempo")]
    public float lastLevelTime = 0f;
    public float accumulatedTime = 0f;

    [Header("Estado entre fases")]
    public int currentLives = 3;
    public int currentKeys = 0;
    public int currentAmmo = 0; //  NOVO: munição persistente
    public bool hasGameStarted = false;

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
    /// Reseta tudo pra começar um novo jogo (chamado na CharacterSelect)
    /// </summary>
    public void ResetGameProgress()
    {
        currentLives = 3;
        currentKeys = 0;
        currentAmmo = 0; // reseta munição também
        accumulatedTime = 0f;
        lastLevelTime = 0f;
        hasGameStarted = true;
        Debug.Log("Progresso do jogo resetado");
    }

    /// <summary>
    /// Salva estado atual antes de mudar de fase ou de morrer.
    /// </summary>
    public void SaveStateBeforeLevelChange(int lives, int keys, float currentTime, int ammo = 0)
    {
        currentLives = lives;
        currentKeys = keys;
        accumulatedTime = currentTime;
        currentAmmo = ammo;
        Debug.Log($"Estado salvo: {lives} vidas, {keys} chaves, {ammo} lanches, tempo {currentTime:F2}s");
    }
}