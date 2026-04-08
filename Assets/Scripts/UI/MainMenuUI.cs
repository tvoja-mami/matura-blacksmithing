using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private TextMeshProUGUI playButtonText;

    [Header("Panels")]
    [SerializeField] private GameObject creditsPanel;

    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    public static bool ShouldLoadSave { get; private set; }

    private void Start()
    {
        ShouldLoadSave = false;
        playButtonText.text = SaveManager.HasSave() ? "Continue" : "Play";

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void OnPlayPressed()
    {
        ShouldLoadSave = SaveManager.HasSave();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnNewGamePressed()
    {
        SaveManager.DeleteSave();
        ShouldLoadSave = false;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnCreditsPressed()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void OnCreditsBackPressed()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void OnQuitPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
