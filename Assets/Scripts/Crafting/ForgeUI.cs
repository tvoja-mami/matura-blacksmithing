using System.Collections.Generic;
using UnityEngine;

public class ForgeUI : MonoBehaviour
{
    public List<RecipeData> unlockedRecipes;
    public GameObject recipeButtonPrefab;
    public Transform recipeButtonContainer;

    [Header("UI")]
    [SerializeField] private GameObject forgePanel;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool isForgeOpen;

    private void Start()
    {
        if (forgePanel == null)
        {
            // Fallback: if this component is on the panel itself.
            forgePanel = gameObject;
        }

        EnsureCanvasGroup();
        SetVisible(false);
    }

    public void OpenForge()
    {
        if (forgePanel == null)
        {
            forgePanel = gameObject;
        }

        EnsureCanvasGroup();

        SetVisible(true);

        if (recipeButtonPrefab == null || recipeButtonContainer == null)
        {
            Debug.LogWarning("ForgeUI: recipeButtonPrefab or recipeButtonContainer is missing.");
            return;
        }

        foreach (Transform child in recipeButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in unlockedRecipes)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeButtonContainer);
            RecipeUI recipeUI = buttonObj.GetComponent<RecipeUI>();
            if (recipeUI != null)
            {
                recipeUI.Initialize(recipe, this);
            }
        }
    }

    public void CloseForge()
    {
        if (forgePanel == null)
        {
            forgePanel = gameObject;
        }

        EnsureCanvasGroup();

        SetVisible(false);
    }

    public void ToggleForge()
    {
        if (isForgeOpen)
        {
            CloseForge();
        }
        else
        {
            OpenForge();
        }
    }

    private void EnsureCanvasGroup()
    {
        if (canvasGroup == null)
        {
            canvasGroup = forgePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = forgePanel.AddComponent<CanvasGroup>();
            }
        }
    }

    private void SetVisible(bool visible)
    {
        isForgeOpen = visible;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            forgePanel.SetActive(true);
        }
        else
        {
            forgePanel.SetActive(visible);
        }
    }
}
