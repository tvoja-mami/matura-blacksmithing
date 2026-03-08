using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttontText;
    [SerializeField] private Button button;

    private RecipeData recipeData;
    private ForgeUI forgeUI;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (buttontText == null)
        {
            if (button != null)
            {
                buttontText = button.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (buttontText == null)
            {
                buttontText = GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }
    }

    public void Initialize(RecipeData data, ForgeUI forgeUI)
    {
        this.recipeData = data;
        this.forgeUI = forgeUI;

        if (buttontText != null)
        {
            buttontText.text = data.recipeName;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnRecipeClicked);
        }
    }

    public void OnRecipeClicked()
    {
        if (recipeData == null || forgeUI == null) return;

        forgeUI.SelectRecipe(recipeData);
    }
}
