using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CharacterSelectController : MonoBehaviour
{
    [Header("Database")]
    public CharacterDatabase characterDatabase;

    [Header("UI - Input do nome")]
    public TMP_InputField nameInput;

    [Header("UI - Slots dos personagens (na ordem do CharacterDatabase)")]
    public CharacterSlotUI[] characterSlots;

    [Header("UI - Botões")]
    public Button startButton;
    public Button backButton;

    [Header("Validação juicy")]
    public float shakeIntensity = 8f;
    public float shakeDuration = 0.4f;

    [Header("Som de erro")]
    public AudioClip errorSound;
    [Range(0f, 1f)] public float errorVolume = 0.7f;

    [Header("Som de sucesso (click válido)")]
    public AudioClip clickSound;
    [Range(0f, 1f)] public float clickVolume = 0.6f;

    [Header("Voz dos personagens (ao selecionar)")]
    [Range(0f, 1f)] public float voiceVolume = 0.8f;

    private AudioSource audioSource;
    private AudioSource voiceAudioSource; // separado pra voz não cortar com sons UI
    private int selectedCharacterIndex = -1;
    private int lastVoiceIndex = -1; // pra não tocar a mesma voz 2x seguidas

    void Start()
    {
        if (characterDatabase == null)
        {
            Debug.LogError("CharacterDatabase não atribuído!");
            return;
        }

        // Cria 2 AudioSources: um pros sons UI, outro pra voz
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        voiceAudioSource = gameObject.AddComponent<AudioSource>();
        voiceAudioSource.playOnAwake = false;

        SetupSlots();

        // Conecta os botões
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButton);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);

        // Carrega nome anterior, se existir
        if (GameData.Instance != null && !string.IsNullOrEmpty(GameData.Instance.playerName))
        {
            nameInput.text = GameData.Instance.playerName;
        }
    }

    void SetupSlots()
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            int index = i; // capture local para o lambda
            CharacterData data = characterDatabase.GetCharacter(i);

            if (data == null || characterSlots[i] == null) continue;

            // Configura o slot com os dados
            characterSlots[i].SetData(data, data.portrait);

            // Adiciona listener de clique apenas se está disponível
            if (data.isAvailable)
            {
                characterSlots[i].button.onClick.AddListener(() => SelectCharacter(index));
            }
        }
    }

    void SelectCharacter(int index)
    {
        // Não faz nada se já tá selecionado
        if (selectedCharacterIndex == index) return;

        selectedCharacterIndex = index;
        CharacterData data = characterDatabase.GetCharacter(index);
        Debug.Log($"Personagem selecionado: {data.characterName}");

        // Atualiza visual de todos os slots
        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (characterSlots[i] != null)
                characterSlots[i].SetSelected(i == selectedCharacterIndex);
        }

        // Toca uma voz aleatória do personagem selecionado
        PlayRandomVoice(data);
    }

    void PlayRandomVoice(CharacterData data)
    {
        if (data.selectionVoices == null || data.selectionVoices.Length == 0) return;
        if (voiceAudioSource == null) return;

        // Para a voz anterior se ainda estiver tocando
        if (voiceAudioSource.isPlaying)
            voiceAudioSource.Stop();

        // Escolhe uma voz aleatória, evitando repetir a última
        int randomIndex;
        if (data.selectionVoices.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            do
            {
                randomIndex = Random.Range(0, data.selectionVoices.Length);
            } while (randomIndex == lastVoiceIndex);
        }

        lastVoiceIndex = randomIndex;
        AudioClip voice = data.selectionVoices[randomIndex];

        if (voice != null)
            voiceAudioSource.PlayOneShot(voice, voiceVolume);
    }

    void OnStartButton()
    {
        string playerName = nameInput.text.Trim();

        // Validação 1: nome
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Nome vazio!");
            StartCoroutine(ShakeRect(nameInput.GetComponent<RectTransform>()));
            PlayErrorSound();
            return;
        }

        // Validação 2: personagem
        if (selectedCharacterIndex < 0)
        {
            Debug.Log("Nenhum personagem escolhido!");
            foreach (var slot in characterSlots)
            {
                if (slot != null && slot.gameObject.activeInHierarchy)
                    StartCoroutine(ShakeRect(slot.GetComponent<RectTransform>()));
            }
            PlayErrorSound();
            return;
        }

        // ? Validações passaram — toca som de sucesso
        PlayClickSound();

        // Salva no GameData e vai pra cena Game
        if (GameData.Instance != null)
        {
            GameData.Instance.SetPlayerName(playerName);
            GameData.Instance.SetCharacter(selectedCharacterIndex);
        }

        Debug.Log($"Iniciando jogo: {playerName} jogando com {characterDatabase.GetCharacter(selectedCharacterIndex).characterName}");

        // Carrega a cena Fase1 com fade
        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade("Fase1");
        else
            SceneManager.LoadScene("Fase1");
    }

    void OnBackButton()
    {
        // Volta pro MainMenu com fade
        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }

    void PlayErrorSound()
    {
        if (errorSound != null && audioSource != null)
            audioSource.PlayOneShot(errorSound, errorVolume);
    }

    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound, clickVolume);
    }

    IEnumerator ShakeRect(RectTransform rt)
    {
        if (rt == null) yield break;

        Vector2 originalPos = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            rt.anchoredPosition = originalPos + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }
}