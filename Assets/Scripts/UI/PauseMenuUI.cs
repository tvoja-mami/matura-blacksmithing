using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        isPaused = false;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        PlayerMovement.ActiveMenuCount++;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        PlayerMovement.ActiveMenuCount--;
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        PlayerMovement.ActiveMenuCount = 0;

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
