using UnityEngine;

[CreateAssetMenu(menuName = "Config/Leaderboard API", fileName = "LeaderboardApiConfig")]
public class LeaderboardApiConfig : ScriptableObject
{
    [Header("Connection")]
    public string baseUrl = "https://paga-lanche-api-production.up.railway.app";
    public string apiKey = "";

    [Header("Defaults")]
    public int defaultLimit = 50;
}
