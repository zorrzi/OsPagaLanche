using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Configuração")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private AudioClip currentClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = musicVolume;
    }

    /// <summary>
    /// Toca uma música, com fade da anterior para a nova.
    /// Se já estiver tocando essa música, não faz nada.
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Já está tocando essa música — não troca
        if (currentClip == clip && audioSource.isPlaying) return;

        StopAllCoroutines();
        StartCoroutine(FadeAndPlay(clip));
    }

    /// <summary>
    /// Para a música atual com fade out.
    /// </summary>
    public void StopMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeAndPlay(AudioClip newClip)
    {
        // Fade out da música atual
        if (audioSource.isPlaying)
            yield return StartCoroutine(FadeOut());

        // Troca para a nova música
        audioSource.clip = newClip;
        currentClip = newClip;
        audioSource.volume = 0f;
        audioSource.Play();

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = musicVolume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = musicVolume;
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null && audioSource.isPlaying)
            audioSource.volume = musicVolume;
    }
}