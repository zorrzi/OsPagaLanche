using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    void Awake()
    {
        if (timerText == null)
            timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (LevelTimer.Instance != null && timerText != null)
        {
            timerText.text = LevelTimer.Instance.GetFormattedTime();
        }
    }
}