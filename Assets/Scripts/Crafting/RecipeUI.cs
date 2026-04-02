using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;

    [Header("Item Detail Panel")]
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image itemDetailIcon;
    [SerializeField] private TextMeshProUGUI itemDetailName;
    [SerializeField] private TextMeshProUGUI itemDetailDescription;

    private RecipeData recipeData;
    private ForgeUI forgeUI;

    private void Awake()
    {
        button = GetComponentInChildren<Button>(true);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Initialize(RecipeData data, ForgeUI forgeUI,
        GameObject detailPanel, Image detailIcon,
        TextMeshProUGUI detailName, TextMeshProUGUI detailDescription)
    {
        this.recipeData = data;
        this.forgeUI = forgeUI;

        this.itemDetailPanel = detailPanel;
        this.itemDetailIcon = detailIcon;
        this.itemDetailName = detailName;
        this.itemDetailDescription = detailDescription;

        if (detailPanel == null) Debug.LogError("detailPanel is null — assign Item Detail Panel on ForgeUI!");
        if (detailIcon == null) Debug.LogError("detailIcon is null — assign Item Detail Icon on ForgeUI!");
        if (detailName == null) Debug.LogError("detailName is null — assign Item Detail Name on ForgeUI!");
        if (detailDescription == null) Debug.LogError("detailDescription is null — assign Item Detail Description on ForgeUI!");

        buttonText.text = data.recipeName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnRecipeClicked);
    }

    public void OnRecipeClicked()
    {
        ShowItemDetail();
        forgeUI.SelectRecipe(recipeData);
    }

    private void ShowItemDetail()
    {
        ItemData output = recipeData.outputItem;
        if (output == null)
        {
            Debug.LogWarning($"Recipe '{recipeData.recipeName}' has no output item assigned!");
            return;
        }

        itemDetailPanel.SetActive(true);
        itemDetailIcon.sprite = output.icon;
        itemDetailIcon.enabled = output.icon != null;
        itemDetailDescription.text = output.description;
        itemDetailName.text = output.itemName;
    }

    public void HideItemDetail()
    {
        itemDetailPanel.SetActive(false);
    }
}
