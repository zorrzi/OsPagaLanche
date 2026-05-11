using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.7f;
    }

    [Header("Lista de sons")]
    public List<SoundEntry> sounds = new List<SoundEntry>();

    private Dictionary<string, SoundEntry> soundLookup;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        soundLookup = new Dictionary<string, SoundEntry>();
        foreach (var sound in sounds)
        {
            if (!string.IsNullOrEmpty(sound.name) && !soundLookup.ContainsKey(sound.name))
                soundLookup.Add(sound.name, sound);
        }
    }

    public void Play(string soundName)
    {
        if (soundLookup == null || !soundLookup.ContainsKey(soundName))
        {
            Debug.LogWarning($"SFX '{soundName}' não encontrado!");
            return;
        }

        SoundEntry entry = soundLookup[soundName];
        if (entry.clip != null)
            audioSource.PlayOneShot(entry.clip, entry.volume);
    }

    public void Play(string soundName, float customVolume)
    {
        if (soundLookup == null || !soundLookup.ContainsKey(soundName)) return;

        SoundEntry entry = soundLookup[soundName];
        if (entry.clip != null)
            audioSource.PlayOneShot(entry.clip, customVolume);
    }
}