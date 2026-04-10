using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    /// <summary>Luck stops increasing after this level. Prevents rarity weights going negative.</summary>
    public const int LuckCap = 20;

    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;

    /// <summary>Fires when XP changes: (currentXP, xpForNextLevel, currentLevel)</summary>
    public static event System.Action<int, int, int> OnXPChanged;

    /// <summary>Fires when the player levels up: (newLevel)</summary>
    public static event System.Action<int> OnLevelUp;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;

    public void SetLevelAndXP(int level, int xp)
    {
        currentLevel = Mathf.Max(1, level);
        currentXP = Mathf.Max(0, xp);
        OnXPChanged?.Invoke(currentXP, XPNeededForNextLevel, currentLevel);
    }

    /// <summary>Luck bonus passed to RarityHelper. Caps at LuckCap.</summary>
    public int Luck => Mathf.Min(currentLevel - 1, LuckCap);

    /// <summary>XP required to go from level n to level n+1.</summary>
    public static int XPToNextLevel(int level) =>
        Mathf.RoundToInt(50f * Mathf.Pow(1.4f, level - 1));

    /// <summary>Total cumulative XP needed to reach a given level from level 1.</summary>
    public static int CumulativeXPForLevel(int level)
    {
        int total = 0;
        for (int i = 1; i < level; i++)
            total += XPToNextLevel(i);
        return total;
    }

    /// <summary>XP earned within the current level (resets each level).</summary>
    public int XPInCurrentLevel => currentXP - CumulativeXPForLevel(currentLevel);

    /// <summary>XP needed to complete the current level.</summary>
    public int XPNeededForNextLevel => XPToNextLevel(currentLevel);

    /// <summary>0-1 progress through the current level, for a progress bar.</summary>
    public float LevelProgress =>
        Mathf.Clamp01((float)XPInCurrentLevel / XPNeededForNextLevel);

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= CumulativeXPForLevel(currentLevel + 1))
        {
            currentLevel++;
            Debug.Log($"PlayerLevel: Level up! Now level {currentLevel}");
            OnLevelUp?.Invoke(currentLevel);
        }

        OnXPChanged?.Invoke(currentXP, XPNeededForNextLevel, currentLevel);
    }
}
