using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardApiClient : MonoBehaviour
{
    public static LeaderboardApiClient Instance { get; private set; }

    [SerializeField] private LeaderboardApiConfig config;

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
        if (config == null)
        {
            config = Resources.Load<LeaderboardApiConfig>("LeaderboardApiConfig");
        }
    }

    public void SubmitRun(string username, int durationSeconds, int score, Action<bool, string> callback = null)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            callback?.Invoke(false, "Username vazio");
            return;
        }

        StartCoroutine(SubmitRunRoutine(username, durationSeconds, score, callback));
    }

    public void GetRuns(int limit, Action<RunRead[], string> callback)
    {
        StartCoroutine(GetRunsRoutine(limit, callback));
    }

    private IEnumerator SubmitRunRoutine(string username, int durationSeconds, int score, Action<bool, string> callback)
    {
        yield return StartCoroutine(CreateUserRoutine(username));

        RunCreate payload = new RunCreate
        {
            username = username,
            duration = Mathf.Max(0, durationSeconds),
            score = Mathf.Max(0, score)
        };

        string json = JsonUtility.ToJson(payload);
        string url = BuildUrl("/runs");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            ApplyHeaders(request, true);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(false, request.error);
                yield break;
            }

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

            // Ignore errors (user may already exist).
        }
    }

    private IEnumerator GetRunsRoutine(int limit, Action<RunRead[], string> callback)
    {
        int safeLimit = limit > 0 ? limit : GetDefaultLimit();
        string url = BuildUrl($"/runs?limit={safeLimit}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            ApplyHeaders(request, false);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(null, request.error);
                yield break;
            }

            RunRead[] runs = ParseRunArray(request.downloadHandler.text);
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
        if (config == null) return string.Empty;
        return config.baseUrl != null ? config.baseUrl.Trim() : string.Empty;
    }

    private string GetApiKey()
    {
        if (config == null) return string.Empty;
        return config.apiKey != null ? config.apiKey.Trim() : string.Empty;
    }

    private int GetDefaultLimit()
    {
        if (config == null || config.defaultLimit <= 0) return 50;
        return config.defaultLimit;
    }
}

