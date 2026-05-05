using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Cursor (opcional - se quiser cursor diferente no hover)")]
    public Texture2D hoverCursor;
    public Vector2 cursorHotspot = new Vector2(8, 4);

    [Header("Sons")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    [Range(0f, 1f)] public float volume = 0.7f;

    private static AudioSource sharedAudioSource;

    void Awake()
    {
        if (sharedAudioSource == null)
        {
            GameObject audioGO = new GameObject("UIAudioSource");
            sharedAudioSource = audioGO.AddComponent<AudioSource>();
            sharedAudioSource.playOnAwake = false;
            DontDestroyOnLoad(audioGO);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCursor != null)
            Cursor.SetCursor(hoverCursor, cursorHotspot, CursorMode.Auto);

        if (hoverSound != null && sharedAudioSource != null)
            sharedAudioSource.PlayOneShot(hoverSound, volume);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Volta pro cursor padrão do jogo
        if (GlobalCursor.Instance != null)
            GlobalCursor.Instance.ApplyDefaultCursor();
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && sharedAudioSource != null)
            sharedAudioSource.PlayOneShot(clickSound, volume);
    }
}