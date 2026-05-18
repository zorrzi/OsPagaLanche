using UnityEngine;

[CreateAssetMenu(menuName = "Config/Leaderboard API", fileName = "LeaderboardApiConfig")]
public class LeaderboardApiConfig : ScriptableObject
{
    [Header("Connection")]
    public string baseUrl = "https://paga-lanche-api-production.up.railway.app";
    public string apiKey = "marinheiro-paga-tudo";

    [Header("Defaults")]
    public int defaultLimit = 50;

    public const string DefaultBaseUrl = "https://paga-lanche-api-production.up.railway.app";
    public const string DefaultApiKey = "marinheiro-paga-tudo";
    public const int DefaultLimit = 50;
}
