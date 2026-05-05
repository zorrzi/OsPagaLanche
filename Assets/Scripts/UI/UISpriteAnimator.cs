using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UISpriteAnimator : MonoBehaviour
{
    [Header("Frames da animação")]
    public Sprite[] frames;

    [Header("Configuração")]
    public float frameRate = 8f; // frames por segundo
    public bool playOnAwake = true;

    private Image image;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool isPlaying = false;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        if (playOnAwake) Play();
    }

    public void Play()
    {
        if (frames == null || frames.Length == 0) return;

        isPlaying = true;
        currentFrame = 0;
        timer = 0f;
        image.sprite = frames[0];
    }

    public void Stop()
    {
        isPlaying = false;
    }

    void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
        }
    }
}