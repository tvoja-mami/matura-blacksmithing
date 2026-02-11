using UnityEngine;
using TMPro; // Remove if using legacy UI Text

public class TimeDayCycle : MonoBehaviour
{
    [Tooltip("Seconds for a full 24h in-game day.")]
    public float dayLengthSeconds = 300f; // 5 minutes

    [Tooltip("Start time in hours (0-24).")]
    [Range(0f, 24f)] public float startHour = 8f;

    [Tooltip("UI text to display the clock (HH:MM).")]
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dayText;

    private float currentTime; // hours 0-24
    private int dayNumber = 5;

    private void Start()
    {
        currentTime = startHour;
        switch (dayNumber)
        {
            case 1:
                dayText.text = "Monday";
                break;
            case 2:
                dayText.text = "Tuesday";
                break;
            case 3:
                dayText.text = "Wednesday";
                break;
            case 4:
                dayText.text = "Thursday";
                break;
            case 5:
                dayText.text = "Friday";
                break;
        }
    }

    private void Update()
    {
        if (dayLengthSeconds <= 0f)
            return;

        float hoursPerSecond = 24f / dayLengthSeconds;
        currentTime = (currentTime + hoursPerSecond * Time.deltaTime) % 24f;

        if (clockText != null && currentTime <= 20f && currentTime >= 8f)
        {
            int h = Mathf.FloorToInt(currentTime);
            int m = Mathf.FloorToInt((currentTime - h) * 60f);
            clockText.text = $"{h:00}:{m:00}";

            if (h == 19 && m == 59)
            {
                clockText.text = "20:00";
            }
        }
    }
    public void Sleep()
    {
        Debug.Log("Player is sleeping...");
        currentTime = 8f;
        if (clockText != null)
        {
            clockText.text = "08:00";
        }
        dayNumber++;
        switch (dayNumber)
        {
            case 1:
                dayText.text = "Monday";
                break;
            case 2:
                dayText.text = "Tuesday";
                break;
            case 3:
                dayText.text = "Wednesday";
                break;
            case 4:
                dayText.text = "Thursday";
                break;
            case 5:
                dayText.text = "Friday";
                break;
            case 6:
                dayText.text = "Monday";
                dayNumber = 1;
                break;
        }
    }
}
