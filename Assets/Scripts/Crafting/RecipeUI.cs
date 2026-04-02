using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    [Header("Item Detail Panel")]
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image itemDetailIcon;
    [SerializeField] private TextMeshProUGUI itemDetailName;
    [SerializeField] private TextMeshProUGUI itemDetailDescription;

    private RecipeData recipeData;
    private ForgeUI forgeUI;

    private static readonly Color LockedColor = new Color(0.55f, 0.55f, 0.55f, 1f);

    private void Awake()
    {
        button      = GetComponentInChildren<Button>(true);
        buttonText  = button.GetComponentInChildren<TextMeshProUGUI>(true);
        buttonImage = button.GetComponent<Image>();
    }

    public void Initialize(RecipeData data, ForgeUI forge, bool isLocked,
        GameObject detailPanel, Image detailIcon,
        TextMeshProUGUI detailName, TextMeshProUGUI detailDescription)
    {
        recipeData = data;
        forgeUI    = forge;

        itemDetailPanel       = detailPanel;
        itemDetailIcon        = detailIcon;
        itemDetailName        = detailName;
        itemDetailDescription = detailDescription;

        buttonText.text = data.recipeName;

        if (isLocked && buttonImage != null)
            buttonImage.color = LockedColor;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnRecipeClicked);
    }

    public void OnRecipeClicked()
    {
        forgeUI.SelectRecipe(recipeData);
    }

    public void HideItemDetail()
    {
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);
    }
}
