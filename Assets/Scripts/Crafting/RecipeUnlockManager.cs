using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks which recipes the player has unlocked.
/// Persists via PlayerPrefs. Tier 1 recipes (unlockCost == 0) are auto-unlocked at start.
/// </summary>
public class RecipeUnlockManager : MonoBehaviour
{
    public static RecipeUnlockManager Instance { get; private set; }

    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private PlayerLevel playerLevel;

    private HashSet<int> unlockedIDs = new HashSet<int>();

    public static event System.Action OnUnlocksChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (playerGold == null)
            playerGold = FindFirstObjectByType<PlayerGold>();
        if (playerLevel == null)
            playerLevel = FindFirstObjectByType<PlayerLevel>();

        LoadUnlocks();
        AutoUnlockFreeRecipes();
        PlayerLevel.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        PlayerLevel.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int _)
    {
        AutoUnlockFreeRecipes();
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    public bool IsUnlocked(RecipeData recipe) => unlockedIDs.Contains(recipe.recipeID);

    public bool TryUnlock(RecipeData recipe)
    {
        if (IsUnlocked(recipe)) return false;

        if (playerLevel != null && playerLevel.CurrentLevel < recipe.requiredLevel)
        {
            Debug.Log($"RecipeUnlockManager: Level too low to unlock {recipe.recipeName}. Need level {recipe.requiredLevel}.");
            return false;
        }

        if (playerGold.CurrentGold < recipe.unlockCost)
        {
            Debug.Log($"RecipeUnlockManager: Not enough gold to unlock {recipe.recipeName}.");
            return false;
        }

        playerGold.RemoveGold(recipe.unlockCost);
        unlockedIDs.Add(recipe.recipeID);
        SaveUnlocks();
        OnUnlocksChanged?.Invoke();
        Debug.Log($"RecipeUnlockManager: Unlocked {recipe.recipeName} for {recipe.unlockCost}g.");
        return true;
    }

    /// <summary>Returns all recipes the player has unlocked.</summary>
    public List<RecipeData> GetUnlockedRecipes()
    {
        List<RecipeData> result = new List<RecipeData>();
        foreach (RecipeData r in allRecipes)
            if (IsUnlocked(r)) result.Add(r);
        return result;
    }

    /// <summary>Returns recipes available to unlock at the given level but not yet unlocked.</summary>
    public List<RecipeData> GetAvailableToUnlock(int playerLevel)
    {
        List<RecipeData> result = new List<RecipeData>();
        foreach (RecipeData r in allRecipes)
            if (!IsUnlocked(r) && r.requiredLevel <= playerLevel) result.Add(r);
        return result;
    }

    // ── Persistence ────────────────────────────────────────────────────────────

    private void SaveUnlocks()
    {
        // Store as comma-separated IDs e.g. "101,102,201"
        PlayerPrefs.SetString("UnlockedRecipes", string.Join(",", unlockedIDs));
        PlayerPrefs.Save();
    }

    private void LoadUnlocks()
    {
        unlockedIDs.Clear();
        string saved = PlayerPrefs.GetString("UnlockedRecipes", "");
        if (string.IsNullOrEmpty(saved)) return;

        foreach (string part in saved.Split(','))
            if (int.TryParse(part, out int id))
                unlockedIDs.Add(id);
    }

    private void AutoUnlockFreeRecipes()
    {
        int level = playerLevel != null ? playerLevel.CurrentLevel : 1;
        bool changed = false;
        foreach (RecipeData r in allRecipes)
        {
            if (r.unlockCost == 0 && r.requiredLevel <= level && !unlockedIDs.Contains(r.recipeID))
            {
                unlockedIDs.Add(r.recipeID);
                changed = true;
            }
        }
        if (changed) { SaveUnlocks(); OnUnlocksChanged?.Invoke(); }
    }
}
