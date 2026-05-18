using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Transform headerContainer;

    [Header("Config")]
    [SerializeField] private int limit = 50;
    [SerializeField] private bool sortByDuration = true;

    private int refreshCallCount = 0;
    private int onRunsLoadedCallCount = 0;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        refreshCallCount++;
        Debug.Log($"[Leaderboard] Refresh() called (count: {refreshCallCount})");
        
        SetStatus("Carregando...");
        ClearList();

        LeaderboardApiClient client = LeaderboardApiClient.EnsureInstance();
        client.GetRuns(limit, OnRunsLoaded);
    }

    private void OnRunsLoaded(RunRead[] runs, string error)
    {
        onRunsLoadedCallCount++;
        Debug.Log($"[Leaderboard] OnRunsLoaded() called (count: {onRunsLoadedCallCount}), runs: {(runs != null ? runs.Length : 0)}, error: {error}");
        
        if (!string.IsNullOrEmpty(error))
        {
            SetStatus("Falha ao carregar leaderboard");
            Debug.LogWarning($"Leaderboard error: {error}");
            return;
        }

        if (runs == null || runs.Length == 0)
        {
            SetStatus("Sem registros");
            return;
        }

        RunRead[] ordered = runs;
        if (sortByDuration)
        {
            ordered = runs.OrderBy(r => r.duration).ThenBy(r => r.created_at).ToArray();
        }

        if (listParent == null)
        {
            Debug.LogError("[Leaderboard] listParent is null! Cannot instantiate rows.");
            SetStatus("Erro ao carregar leaderboard");
            return;
        }

        if (rowPrefab == null)
        {
            Debug.LogError("[Leaderboard] rowPrefab is null! Cannot instantiate rows.");
            SetStatus("Erro ao carregar leaderboard");
            return;
        }

        for (int i = 0; i < ordered.Length; i++)
        {
            GameObject rowInstance = Instantiate(rowPrefab, listParent, false);
            LeaderboardRowView row = rowInstance.GetComponent<LeaderboardRowView>();
            
            if (row == null)
            {
                Debug.LogError($"[Leaderboard] LeaderboardRowView component not found on prefab instance at index {i}");
                Destroy(rowInstance);
                continue;
            }
            
            row.SetData(i + 1, ordered[i]);
        }

        // Force rebuild of content layout
        if (listParent != null)
        {
            RectTransform contentRect = listParent as RectTransform;
            if (contentRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
                Debug.Log($"[Leaderboard] Layout rebuilt for content. Child count: {listParent.childCount}");
            }
            else
            {
                Debug.LogWarning("[Leaderboard] listParent is not a RectTransform!");
            }
        }

        SetStatus(string.Empty);
    }


    private void ClearList()
    {
        if (listParent == null)
        {
            Debug.LogWarning("[Leaderboard] listParent is null! Cannot clear list.");
            return;
        }
        
        for (int i = listParent.childCount - 1; i >= 0; i--)
        {
            Destroy(listParent.GetChild(i).gameObject);
        }
        
        Debug.Log("[Leaderboard] List cleared. All child rows destroyed.");
    }

    private void SetStatus(string message)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }
}

