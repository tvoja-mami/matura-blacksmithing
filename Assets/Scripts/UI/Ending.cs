using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Full-screen victory panel shown when the player buys the shop.
/// Attach to a Canvas or child panel that starts disabled.
/// Wire the "Main Menu" button to OnMainMenuPressed() in the Inspector.
/// </summary>
public class Ending : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        // Player now owns the shop — no more rent
        if (RentManager.Instance != null)
            RentManager.Instance.Rent = false;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Freeze game and block other inputs
        Time.timeScale = 0f;
        PlayerMovement.ActiveMenuCount++;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIOpen();
    }

    /// <summary>Dismiss the victory panel and let the player keep playing rent-free.</summary>
    public void OnContinuePressed()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        Time.timeScale = 1f;
        PlayerMovement.ActiveMenuCount--;
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        PlayerMovement.ActiveMenuCount = 0;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
