using UnityEngine;

public class BossTracker : MonoBehaviour
{
    public static BossTracker Instance { get; private set; }

    [Header("Boss da fase")]
    [Tooltip("Arrasta o GameObject do boss aqui (o que tem EnemyHealth com isBoss marcado)")]
    public EnemyHealth bossHealth;

    public bool BossDefeated { get; set; } = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (bossHealth != null)
        {
            bossHealth.OnDeath += OnBossDefeated;
        }
        else
        {
            Debug.LogWarning("BossTracker n�o tem boss atribu�do!");
        }
    }

    void OnDestroy()
    {
        if (bossHealth != null)
            bossHealth.OnDeath -= OnBossDefeated;
    }

    void OnBossDefeated()
    {
        BossDefeated = true;
        Debug.Log("Boss derrotado! Bandeira liberada.");
    }
}