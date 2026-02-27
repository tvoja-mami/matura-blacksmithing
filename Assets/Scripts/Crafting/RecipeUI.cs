using TMPro;
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
            buttontText.text = recipeData != null ? recipeData.recipeName : "Missing Recipe";
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
