using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Música dessa cena")]
    [Tooltip("Deixe vazio se quiser silêncio nessa cena")]
    public AudioClip music;

    void Start()
    {
        if (MusicManager.Instance == null) return;

        if (music != null)
        {
            // Toca a música dessa cena
            MusicManager.Instance.PlayMusic(music);
        }
        else
        {
            // Sem música — para a música atual com fade
            MusicManager.Instance.StopMusic();
        }
    }
}