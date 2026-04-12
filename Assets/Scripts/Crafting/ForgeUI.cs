using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ForgeUI : MonoBehaviour
{
    [Header("Recipe List")]
    public List<RecipeData> allRecipes;
    public GameObject recipeButtonPrefab;
    public Transform recipeButtonContainer;

    [Header("Required Items Display")]
    public GameObject requiredItemsPrefab;
    public Transform requiredItemsContainer;

    [Header("UI")]
    [SerializeField] private GameObject forgePanel;
    [SerializeField] private TextMeshProUGUI selectedRecipeText;

    [Header("Item Detail Panel")]
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image itemDetailIcon;
    [SerializeField] private TextMeshProUGUI itemDetailName;
    [SerializeField] private TextMeshProUGUI itemDetailDescription;

    [Header("Craft / Unlock Button")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftButtonText;   // drag the TMP child of the Craft button here
    [SerializeField] private TextMeshProUGUI unlockInfoText;    // TMP text shown when a locked recipe is selected

    [Header("Crafting Mini-game")]
    [SerializeField] private CraftingMinigame craftingMinigame;

    [Header("Progression")]
    [SerializeField] private PlayerLevel playerLevel;

    private bool isForgeOpen;
    private RecipeData selectedRecipe;
    private PlayerInventory playerInventory;

    private void Awake()
    {
        if (playerLevel == null)
            playerLevel = FindFirstObjectByType<PlayerLevel>();
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    private void OnEnable()
    {
        PlayerLevel.OnLevelUp                += HandleLevelUp;
        RecipeUnlockManager.OnUnlocksChanged += HandleUnlocksChanged;
        PlayerInventory.OnInventoryChanged   += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        PlayerLevel.OnLevelUp                -= HandleLevelUp;
        RecipeUnlockManager.OnUnlocksChanged -= HandleUnlocksChanged;
        PlayerInventory.OnInventoryChanged   -= HandleInventoryChanged;
    }

    private void HandleLevelUp(int _)       { if (isForgeOpen) PopulateRecipeList(); }
    private void HandleUnlocksChanged()     { if (isForgeOpen) PopulateRecipeList(); }
    private void HandleInventoryChanged()   { if (isForgeOpen && selectedRecipe != null) ShowRequiredItems(selectedRecipe); }

    // ── Open / Close ───────────────────────────────────────────────────────────

    public void OpenForge()
    {
        isForgeOpen = true;
        forgePanel.SetActive(true);
        PlayerMovement.ActiveMenuCount++;
        PopulateRecipeList();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIOpen();
    }

    public void CloseForge()
    {
        isForgeOpen = false;
        forgePanel.SetActive(false);
        PlayerMovement.ActiveMenuCount--;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIClose();
    }

    public void ToggleForge()
    {
        if (isForgeOpen) CloseForge();
        else OpenForge();
    }

    private void Update()
    {
        if (isForgeOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseForge();
    }

    // ── Recipe List ────────────────────────────────────────────────────────────

    private void PopulateRecipeList()
    {
        ClearChildren(recipeButtonContainer);
        ClearSelectedRecipe();

        var unlocked = new List<RecipeData>();
        var locked   = new List<RecipeData>();

        foreach (RecipeData recipe in allRecipes)
        {
            bool isUnlocked = RecipeUnlockManager.Instance == null
                              || RecipeUnlockManager.Instance.IsUnlocked(recipe);
            if (isUnlocked) unlocked.Add(recipe);
            else            locked.Add(recipe);
        }

        unlocked.Sort((a, b) => a.requiredLevel.CompareTo(b.requiredLevel));
        locked.Sort((a, b) => a.requiredLevel.CompareTo(b.requiredLevel));

        foreach (RecipeData recipe in unlocked) SpawnRecipeButton(recipe, false);
        foreach (RecipeData recipe in locked)   SpawnRecipeButton(recipe, true);
    }

    private void SpawnRecipeButton(RecipeData recipe, bool isLocked)
    {
        GameObject obj = Instantiate(recipeButtonPrefab, recipeButtonContainer);
        RecipeUI ui = obj.GetComponent<RecipeUI>();
        if (ui != null)
            ui.Initialize(recipe, this, isLocked,
                itemDetailPanel, itemDetailIcon,
                itemDetailName, itemDetailDescription);
    }

    // ── Selection ──────────────────────────────────────────────────────────────

    public void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;

        bool isLocked = RecipeUnlockManager.Instance != null
                        && !RecipeUnlockManager.Instance.IsUnlocked(recipe);

        if (selectedRecipeText != null)
            selectedRecipeText.text = recipe.recipeName;

        if (craftButton != null)
            craftButton.interactable = true;

        if (craftButtonText != null)
            craftButtonText.text = isLocked ? "Unlock" : "Craft";

        if (unlockInfoText != null)
        {
            if (isLocked)
            {
                int level = playerLevel != null ? playerLevel.CurrentLevel : 1;
                string info = recipe.requiredLevel > level
                    ? $"Requires Level {recipe.requiredLevel}\n"
                    : "";
                info += $"Cost: {recipe.unlockCost}g";
                unlockInfoText.text = info;
                unlockInfoText.gameObject.SetActive(true);
            }
            else
            {
                unlockInfoText.gameObject.SetActive(false);
            }
        }

        ShowRequiredItems(recipe);
        ShowItemDetail(recipe);
    }

    // ── Craft / Unlock Button ──────────────────────────────────────────────────

    public void TryCraft()
    {
        if (selectedRecipe == null) return;

        // Day is over — don't allow new crafts
        if (GameManager.Instance != null && GameManager.Instance.IsWaitingForNextDay)
        {
            CloseForge();
            return;
        }

        bool isLocked = RecipeUnlockManager.Instance != null
                        && !RecipeUnlockManager.Instance.IsUnlocked(selectedRecipe);

        if (isLocked)
        {
            RecipeUnlockManager.Instance?.TryUnlock(selectedRecipe);
            return;
        }

        if (craftingMinigame == null)
        {
            Debug.LogError("ForgeUI: CraftingMinigame reference not assigned!");
            return;
        }
        craftingMinigame.StartCraft(selectedRecipe);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private void ClearSelectedRecipe()
    {
        selectedRecipe = null;
        ClearChildren(requiredItemsContainer);
        if (selectedRecipeText != null) selectedRecipeText.text = "";
        if (craftButton != null)        craftButton.interactable = false;
        if (craftButtonText != null)    craftButtonText.text = "Craft";
        if (unlockInfoText != null)     unlockInfoText.gameObject.SetActive(false);
        if (itemDetailPanel != null)    itemDetailPanel.SetActive(false);
    }

    private void ShowItemDetail(RecipeData recipe)
    {
        ItemData output = recipe.outputItem;
        if (output == null || itemDetailPanel == null) return;

        itemDetailPanel.SetActive(true);
        if (itemDetailIcon != null)
        {
            itemDetailIcon.sprite  = output.icon;
            itemDetailIcon.enabled = output.icon != null;
        }
        if (itemDetailName != null)        itemDetailName.text = output.itemName;
        if (itemDetailDescription != null) itemDetailDescription.text = output.description;
    }

    private void ShowRequiredItems(RecipeData recipe)
    {
        ClearChildren(requiredItemsContainer);
        if (recipe.requiredMaterials == null) return;
        if (requiredItemsPrefab == null) { Debug.LogError("requiredItemsPrefab is null!"); return; }

        foreach (var req in recipe.requiredMaterials)
        {
            if (req.item == null) continue;
            GameObject entry = Instantiate(requiredItemsPrefab, requiredItemsContainer);
            TextMeshProUGUI tmp = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                int owned = 0;
                if (playerInventory != null && playerInventory.items.ContainsKey(req.item))
                    owned = playerInventory.items[req.item];

                tmp.text = $"{req.item.itemName}: {owned}/{req.amount}";
                tmp.color = owned < req.amount ? Color.red : Color.white;
            }
        }
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (child == recipeButtonPrefab || child == requiredItemsPrefab) continue;
            Destroy(child);
        }
    }
}
