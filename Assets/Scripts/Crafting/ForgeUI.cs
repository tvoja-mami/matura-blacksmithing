using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForgeUI : MonoBehaviour
{
    public List<RecipeData> unlockedRecipes;
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

    public GameObject ItemDetailPanel => itemDetailPanel;
    public Image ItemDetailIcon => itemDetailIcon;
    public TextMeshProUGUI ItemDetailName => itemDetailName;
    public TextMeshProUGUI ItemDetailDescription => itemDetailDescription;

    [Header("Crafting Mini-game")]
    [SerializeField] private CraftingMinigame craftingMinigame;
    [SerializeField] private Button craftButton;

    private bool isForgeOpen;
    private RecipeData selectedRecipe;

    public void OpenForge()
    {
        isForgeOpen = true;
        forgePanel.SetActive(true);
        PlayerMovement.ActiveMenuCount++;
        selectedRecipe = null;
        if (craftButton != null) craftButton.interactable = false;
        ClearSelectedRecipe();
        PopulateRecipeList();
    }

    public void CloseForge()
    {
        isForgeOpen = false;
        forgePanel.SetActive(false);
        PlayerMovement.ActiveMenuCount--;
    }

    public void ToggleForge()
    {
        if (isForgeOpen) CloseForge();
        else OpenForge();
    }

    public void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;

        if (selectedRecipeText != null)
            selectedRecipeText.text = recipe != null ? recipe.recipeName : "";

        if (craftButton != null)
            craftButton.interactable = recipe != null;

        ShowRequiredItems(recipe);
    }

    /// <summary>Called by the CraftButton onClick event in the Inspector.</summary>
    public void TryCraft()
    {
        if (craftingMinigame == null)
        {
            Debug.LogError("ForgeUI: CraftingMinigame reference not assigned!");
            return;
        }

        craftingMinigame.StartCraft(selectedRecipe);
    }

    private void PopulateRecipeList()
    {
        ClearChildren(recipeButtonContainer);

        foreach (RecipeData recipe in unlockedRecipes)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeButtonContainer);
            RecipeUI recipeUI = buttonObj.GetComponent<RecipeUI>();
            if (recipeUI != null)
                recipeUI.Initialize(recipe, this,
                    itemDetailPanel, itemDetailIcon,
                    itemDetailName, itemDetailDescription);
        }
    }

    private void ClearSelectedRecipe()
    {
        ClearChildren(requiredItemsContainer);
        if (selectedRecipeText != null) selectedRecipeText.text = "";
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);
    }

    private void ShowRequiredItems(RecipeData recipe)
    {
        ClearChildren(requiredItemsContainer);
        if (recipe == null || recipe.requiredMaterials == null) return;
        if (requiredItemsPrefab == null) { Debug.LogError("requiredItemsPrefab is null or destroyed!"); return; }

        foreach (var req in recipe.requiredMaterials)
        {
            if (req.item == null) continue;

            GameObject entry = Instantiate(requiredItemsPrefab, requiredItemsContainer);
            TextMeshProUGUI tmp = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = $"{req.item.itemName}: {req.amount}";
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

