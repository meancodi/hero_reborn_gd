using UnityEngine;
using TMPro;

public class SpeedrunTimerUI : MonoBehaviour
{
    public static SpeedrunTimerUI instance;

    [SerializeField] private TMP_Text timerText;

    private void Awake()
    {
        instance = this;
    }

    public void ShowFinalTime(float time)
    {
        timerText.text = FormatTime(time);
    }

    private string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        int milliseconds = Mathf.FloorToInt((t * 1000f) % 1000f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
