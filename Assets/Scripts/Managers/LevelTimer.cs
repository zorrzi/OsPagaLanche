using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [Header("Configuração")]
    [SerializeField] private bool startOnAwake = true;

    private float currentTime = 0f;
    private bool isRunning = false;

    // Eventos para outros scripts reagirem
    public System.Action<float> OnTimerStopped;

    // Propriedades públicas
    public float CurrentTime => currentTime;
    public bool IsRunning => isRunning;

    void Awake()
    {
        // Singleton — garante que só existe um LevelTimer na cena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Se tem tempo acumulado no GameData, continua de onde parou
        if (GameData.Instance != null && GameData.Instance.accumulatedTime > 0f)
        {
            currentTime = GameData.Instance.accumulatedTime;
            Debug.Log($"Cronômetro continuando de {GetFormattedTime()}");
        }
        else
        {
            currentTime = 0f;
            Debug.Log("Cronômetro iniciado do zero");
        }

        if (startOnAwake)
            StartTimer();
    }

    void Update()
    {
        if (isRunning)
            currentTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        isRunning = true;
        Debug.Log("Cronômetro iniciado");
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Cronômetro parado em: {GetFormattedTime()}");
        OnTimerStopped?.Invoke(currentTime);
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        isRunning = false;
    }

    // Retorna o tempo formatado como MM:SS.cc
    public string GetFormattedTime()
    {
        return FormatTime(currentTime);
    }

    // Método estático útil para formatar qualquer tempo (usado pelo ranking depois)
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60f);
        int seconds = (int)(timeInSeconds % 60f);
        int centiseconds = (int)((timeInSeconds * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, centiseconds);
    }
}