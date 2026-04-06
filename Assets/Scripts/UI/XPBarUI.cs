using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [SerializeField] private RectTransform xpFillRect;
    [SerializeField] private RectTransform xpBarBackground;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;

    private PlayerLevel playerLevel;

    private void Start()
    {
        playerLevel = FindFirstObjectByType<PlayerLevel>();
        RefreshAll();
    }

    private void OnEnable()
    {
        PlayerLevel.OnXPChanged += HandleXPChanged;
        PlayerLevel.OnLevelUp  += HandleLevelUp;
    }

    private void OnDisable()
    {
        PlayerLevel.OnXPChanged -= HandleXPChanged;
        PlayerLevel.OnLevelUp  -= HandleLevelUp;
    }

    private void HandleXPChanged(int currentXP, int xpForNext, int level)
    {
        if (playerLevel == null) return;
        UpdateBar();
    }

    private void HandleLevelUp(int newLevel)
    {
        if (playerLevel == null) return;
        UpdateBar();
    }

    private void RefreshAll()
    {
        if (playerLevel == null) return;
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (xpFillRect != null && xpBarBackground != null)
        {
            float maxWidth = xpBarBackground.rect.width;
            xpFillRect.sizeDelta = new Vector2(maxWidth * playerLevel.LevelProgress, xpFillRect.sizeDelta.y);
        }

        if (levelText != null)
            levelText.text = $"Level {playerLevel.CurrentLevel}";

        if (xpText != null)
            xpText.text = $"{playerLevel.XPInCurrentLevel} / {playerLevel.XPNeededForNextLevel} XP";
    }
}
