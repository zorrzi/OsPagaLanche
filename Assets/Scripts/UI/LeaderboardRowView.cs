using TMPro;
using UnityEngine;
using System;

public class LeaderboardRowView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dateText;

    public void SetData(int rank, RunRead run)
    {
        if (rankText != null) rankText.text = rank.ToString();
        if (playerText != null) playerText.text = run.username;
        if (timeText != null) timeText.text = LevelTimer.FormatTime(run.duration);
        if (scoreText != null) scoreText.text = run.score.ToString();
        if (dateText != null) dateText.text = FormatDate(run.created_at);
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
}

