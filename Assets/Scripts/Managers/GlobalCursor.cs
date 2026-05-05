using UnityEngine;

public class GlobalCursor : MonoBehaviour
{
    public static GlobalCursor Instance { get; private set; }

    [Header("Cursores")]
    public Texture2D defaultCursor;     // mão apontando
    public Texture2D clickCursor;       // mão fechada
    public Vector2 hotspot = new Vector2(8, 4);

    [Header("Sons (opcional)")]
    public AudioClip clickSound;
    [Range(0f, 1f)] public float volume = 0.7f;

    private static AudioSource sharedAudioSource;
    private bool isOverButton = false; // se está em cima de um botão

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sharedAudioSource == null)
        {
            GameObject audioGO = new GameObject("GlobalCursorAudio");
            sharedAudioSource = audioGO.AddComponent<AudioSource>();
            sharedAudioSource.playOnAwake = false;
            DontDestroyOnLoad(audioGO);
        }
    }

    void Start()
    {
        ApplyDefaultCursor();
    }

    void Update()
    {
        // Botão esquerdo do mouse pressionado
        if (Input.GetMouseButtonDown(0))
        {
            if (clickCursor != null)
                Cursor.SetCursor(clickCursor, hotspot, CursorMode.Auto);

            if (clickSound != null && sharedAudioSource != null)
                sharedAudioSource.PlayOneShot(clickSound, volume);
        }

        // Botão esquerdo solto
        if (Input.GetMouseButtonUp(0))
        {
            ApplyDefaultCursor();
        }
    }

    public void ApplyDefaultCursor()
    {
        if (defaultCursor != null)
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    public void SetButtonHover(bool isHovering)
    {
        isOverButton = isHovering;
    }
}