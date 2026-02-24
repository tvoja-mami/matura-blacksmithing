using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dayText;

    private void OnEnable()
    {
        GameManager.OnTimeChanged += UpdateClock;
    }

    private void OnDisable()
    {
        GameManager.OnTimeChanged -= UpdateClock;
    }

    private void UpdateClock(float newTime, int newDay)
    {
        int hours = Mathf.FloorToInt(newTime);
        int minutes = Mathf.FloorToInt((newTime - hours) * 60f);
        clockText.text = $"{hours:00}:{minutes:00}";

        switch (newDay)
        {
            case 1: dayText.text = "Monday"; break;
            case 2: dayText.text = "Tuesday"; break;
            case 3: dayText.text = "Wednesday"; break;
            case 4: dayText.text = "Thursday"; break;
            case 5: dayText.text = "Friday"; break;
            default: dayText.text = "ErrorDay"; break;
        }
    }
}