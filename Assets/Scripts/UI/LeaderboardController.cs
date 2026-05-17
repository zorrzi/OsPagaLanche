using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform listParent;
    [SerializeField] private LeaderboardRowView rowPrefab;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Config")]
    [SerializeField] private int limit = 50;
    [SerializeField] private bool sortByDuration = true;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        SetStatus("Carregando...");
        ClearList();

        LeaderboardApiClient client = LeaderboardApiClient.EnsureInstance();
        client.GetRuns(limit, OnRunsLoaded);
    }

    private void OnRunsLoaded(RunRead[] runs, string error)
    {
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

        for (int i = 0; i < ordered.Length; i++)
        {
            LeaderboardRowView row = Instantiate(rowPrefab, listParent);
            row.SetData(i + 1, ordered[i]);
        }

        SetStatus(string.Empty);
    }

    private void ClearList()
    {
        if (listParent == null) return;
        for (int i = listParent.childCount - 1; i >= 0; i--)
        {
            Destroy(listParent.GetChild(i).gameObject);
        }
    }

    private void SetStatus(string message)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }
}

