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
        button = GetComponent<Button>();
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

        itemDetailPanel.SetActive(true);
        itemDetailIcon.sprite = output.icon;
        itemDetailIcon.enabled = output.icon != null;
        itemDetailName.text = output.itemName;
        itemDetailDescription.text = output.description;
    }

    public void HideItemDetail()
    {
        itemDetailPanel.SetActive(false);
    }
}
