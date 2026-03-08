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
    [SerializeField] private Image itemDetailIcon;
    [SerializeField] private TextMeshProUGUI itemDetailText;

    private bool isForgeOpen;

    public void OpenForge()
    {
        isForgeOpen = true;
        forgePanel.SetActive(true);
        PlayerMovement.ActiveMenuCount++;
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
        if (selectedRecipeText != null)
            selectedRecipeText.text = recipe != null ? recipe.recipeName : "";

        if (recipe != null && recipe.outputItem != null)
        {
            if (itemDetailIcon != null) itemDetailIcon.sprite = recipe.outputItem.icon;
            if (itemDetailText != null) itemDetailText.text = recipe.outputItem.description;
        }

        ShowRequiredItems(recipe);
    }

    private void PopulateRecipeList()
    {
        ClearChildren(recipeButtonContainer);

        foreach (RecipeData recipe in unlockedRecipes)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeButtonContainer);
            RecipeUI recipeUI = buttonObj.GetComponent<RecipeUI>();
            if (recipeUI != null)
                recipeUI.Initialize(recipe, this);
        }
    }

    private void ClearSelectedRecipe()
    {
        ClearChildren(requiredItemsContainer);
        if (selectedRecipeText != null) selectedRecipeText.text = "";
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

