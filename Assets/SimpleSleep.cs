using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleSleep : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float sleepDuration = 3f;

    [Header("References")]
    [SerializeField] private ClockUI clockUI;
    [SerializeField] private GameManager gameManager;

    private bool isSleeping = false;

    private void Update()
    {
        if (isSleeping)
            return;

        if (Bed.ConsumeTrigger())
        {
            // Only allow sleep at 20:00 or later
            if (clockUI != null && clockUI.CurrentHour >= 20)
            {
                StartCoroutine(SleepRoutine());
            }
            else
            {
                Debug.Log("You can only sleep at 20:00");
            }
        }
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;

        // Fade to black
        yield return StartCoroutine(FadeTo(1f));

        // Advance to next day (sets time to 8:00)
        if (gameManager != null)
        {
            gameManager.AdvanceToNextDay();
        }

        // Stay black for 3 seconds
        yield return new WaitForSeconds(sleepDuration);

        // Fade back to game
        yield return StartCoroutine(FadeTo(0f));

        isSleeping = false;
    }


    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("Fade Image is not assigned!");
            yield break;
        }

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}