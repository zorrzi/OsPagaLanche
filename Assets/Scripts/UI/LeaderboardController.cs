using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private TextMeshProUGUI statusText;

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
            if (leaderboardText != null)
            {
                leaderboardText.text = string.Empty;
            }
            return;
        }

        if (runs == null || runs.Length == 0)
        {
            SetStatus("Sem registros");
            if (leaderboardText != null)
            {
                leaderboardText.text = string.Empty;
            }
            return;
        }

        RunRead[] ordered = runs;
        if (sortByDuration)
        {
            ordered = runs.OrderBy(r => r.duration).ThenBy(r => r.created_at).ToArray();
        }

        StringBuilder sb = new StringBuilder();
        
        // Header
        sb.AppendLine("# | Player | Time | Score | Date");
        sb.AppendLine(new string('-', 50));
        
        // Rows
        for (int i = 0; i < ordered.Length; i++)
        {
            string rank = (i + 1).ToString().PadLeft(3);
            string player = ordered[i].username.PadRight(12);
            string time = LevelTimer.FormatTime(ordered[i].duration).PadRight(10);
            string score = ordered[i].score.ToString().PadLeft(6);
            string date = FormatDate(ordered[i].created_at).PadRight(15);
            
            sb.AppendLine($"{rank} | {player} | {time} | {score} | {date}");
        }

        if (leaderboardText != null)
        {
            leaderboardText.text = sb.ToString();
            leaderboardText.ForceMeshUpdate();
        }

        SetStatus(string.Empty);
    }

    private string FormatDate(string dateTimeStr)
    {
        if (string.IsNullOrEmpty(dateTimeStr)) return "-";
        
        if (System.DateTime.TryParse(dateTimeStr, out System.DateTime dt))
        {
            return dt.ToString("dd/MM HH:mm");
        }
        
        return "-";
    }

    private void ClearList()
    {
        if (leaderboardText != null)
        {
            leaderboardText.text = string.Empty;
        }
    }

    private void SetStatus(string message)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }
}

