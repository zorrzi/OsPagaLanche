using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardApiClient : MonoBehaviour
{
    public static LeaderboardApiClient Instance { get; private set; }

    [SerializeField] private LeaderboardApiConfig config;

    // Proteção contra submissões duplicadas por sessão
    private string currentSessionUsername = null;
    private bool hasSubmittedRunThisSession = false;

    public static LeaderboardApiClient EnsureInstance()
    {
        if (Instance != null) return Instance;

        GameObject go = new GameObject("LeaderboardApiClient");
        Instance = go.AddComponent<LeaderboardApiClient>();
        DontDestroyOnLoad(go);
        return Instance;
    }

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

    void Start()
    {
        // No asset load: we rely on inline defaults when config is not assigned.
        Debug.Log("[LeaderboardApiClient] Initialized with base URL: " + LeaderboardApiConfig.DefaultBaseUrl);
    }

    /// <summary>
    /// Submete uma run com validações e proteção contra duplicação.
    /// Regra: Chamar apenas uma vez por partida, no fim do jogo.
    /// </summary>
    public void SubmitRun(string username, int durationSeconds, int score, Action<bool, string> callback = null)
    {
        // Validação 1: Username não vazio
        if (string.IsNullOrWhiteSpace(username))
        {
            string errMsg = "[LeaderboardApiClient] SubmitRun bloqueado: username vazio.";
            Debug.LogError(errMsg);
            callback?.Invoke(false, "Username vazio");
            return;
        }

        // Validação 2: Username com máximo 80 caracteres (regra da API)
        if (username.Length > 80)
        {
            string errMsg = $"[LeaderboardApiClient] SubmitRun bloqueado: username '{username}' excede 80 caracteres ({username.Length}).";
            Debug.LogError(errMsg);
            callback?.Invoke(false, "Username muito longo (máx 80 caracteres)");
            return;
        }

        // Validação 3: Proteção contra submissão duplicada na mesma sessão
        if (hasSubmittedRunThisSession && currentSessionUsername == username)
        {
            string warnMsg = $"[LeaderboardApiClient] SubmitRun bloqueado para '{username}': já foi submetida uma run nesta sessão. Prevenção de duplicação ativa.";
            Debug.LogWarning(warnMsg);
            callback?.Invoke(false, "Run já foi submetida nesta sessão");
            return;
        }

        currentSessionUsername = username;
        
        Debug.Log($"[LeaderboardApiClient] Tentando submeter run para '{username}' ({durationSeconds}s, score={score})...");
        StartCoroutine(SubmitRunRoutine(username, durationSeconds, score, callback));
    }

    public void GetRuns(int limit, Action<RunRead[], string> callback)
    {
        StartCoroutine(GetRunsRoutine(limit, callback));
    }

    private IEnumerator SubmitRunRoutine(string username, int durationSeconds, int score, Action<bool, string> callback)
    {
        // Passo 1: Verificar/criar usuário
        Debug.Log($"[LeaderboardApiClient] Verificando se usuário '{username}' existe...");
        yield return StartCoroutine(CreateUserRoutine(username));

        // Passo 2: Preparar payload
        RunCreate payload = new RunCreate
        {
            username = username,
            duration = Mathf.Max(0, durationSeconds),
            score = Mathf.Max(0, score)
        };

        string json = JsonUtility.ToJson(payload);
        string url = BuildUrl("/runs");

        Debug.Log($"[LeaderboardApiClient] Enviando run para {url} com payload: {json}");

        // Passo 3: Enviar run
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            ApplyHeaders(request, true);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMsg = $"[LeaderboardApiClient] Erro ao enviar run: HTTP {request.responseCode} - {request.error}";
                Debug.LogError(errorMsg);
                callback?.Invoke(false, request.error);
                yield break;
            }

            // Sucesso: marcar como submetido nesta sessão
            hasSubmittedRunThisSession = true;
            Debug.Log($"[LeaderboardApiClient] Run submetida com sucesso para '{username}'! Response: {request.downloadHandler.text}");
            callback?.Invoke(true, request.downloadHandler.text);
        }
    }

    private IEnumerator CreateUserRoutine(string username)
    {
        UserCreate payload = new UserCreate { username = username };
        string json = JsonUtility.ToJson(payload);
        string url = BuildUrl("/users");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            ApplyHeaders(request, true);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[LeaderboardApiClient] Usuário '{username}' criado com sucesso.");
            }
            else if (request.responseCode == 409)
            {
                // 409 Conflict: usuário já existe (esperado)
                Debug.Log($"[LeaderboardApiClient] Usuário '{username}' já existe (HTTP 409). Continuando...");
            }
            else
            {
                // Outro erro, mas continua mesmo assim (conforme regra original)
                Debug.LogWarning($"[LeaderboardApiClient] Aviso ao criar/verificar usuário '{username}': HTTP {request.responseCode} - {request.error}. Continuando com submissão de run...");
            }
        }
    }

    private IEnumerator GetRunsRoutine(int limit, Action<RunRead[], string> callback)
    {
        int safeLimit = limit > 0 ? limit : GetDefaultLimit();
        string url = BuildUrl($"/runs?limit={safeLimit}");

        Debug.Log($"[LeaderboardApiClient] Carregando leaderboard ({safeLimit} runs)...");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            ApplyHeaders(request, false);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMsg = $"[LeaderboardApiClient] Erro ao carregar leaderboard: HTTP {request.responseCode} - {request.error}";
                Debug.LogError(errorMsg);
                callback?.Invoke(null, request.error);
                yield break;
            }

            RunRead[] runs = ParseRunArray(request.downloadHandler.text);
            Debug.Log($"[LeaderboardApiClient] Leaderboard carregado: {runs.Length} runs.");
            callback?.Invoke(runs, null);
        }
    }

    private RunRead[] ParseRunArray(string json)
    {
        if (string.IsNullOrEmpty(json)) return new RunRead[0];

        string wrapped = "{\"items\":" + json + "}";
        RunReadList list = JsonUtility.FromJson<RunReadList>(wrapped);
        return list != null && list.items != null ? list.items : new RunRead[0];
    }

    private void ApplyHeaders(UnityWebRequest request, bool hasBody)
    {
        if (hasBody)
        {
            request.SetRequestHeader("Content-Type", "application/json");
        }

        string apiKey = GetApiKey();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.SetRequestHeader("X-API-Key", apiKey);
        }
    }

    private string BuildUrl(string path)
    {
        string baseUrl = GetBaseUrl();
        if (string.IsNullOrWhiteSpace(baseUrl)) return path;

        if (baseUrl.EndsWith("/")) baseUrl = baseUrl.TrimEnd('/');
        return baseUrl + path;
    }

    private string GetBaseUrl()
    {
        if (config == null) return LeaderboardApiConfig.DefaultBaseUrl;
        return config.baseUrl != null ? config.baseUrl.Trim() : string.Empty;
    }

    private string GetApiKey()
    {
        if (config == null) return LeaderboardApiConfig.DefaultApiKey;
        return config.apiKey != null ? config.apiKey.Trim() : string.Empty;
    }

    private int GetDefaultLimit()
    {
        if (config == null || config.defaultLimit <= 0) return LeaderboardApiConfig.DefaultLimit;
        return config.defaultLimit;
    }
}

