using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sits on each spawned sell-slot prefab (same pattern as RecipeUI).
/// Finds its own Button, fills in item data, and calls back to SellUI on click.
/// </summary>
public class SellSlotUI : MonoBehaviour
{
    private Button button;
    private Image buttonImage;
    private SellUI sellUI;
    private int slotIndex;

    private void Awake()
    {
        button      = GetComponentInChildren<Button>(true);
        buttonImage = button != null ? button.GetComponent<Image>() : null;
    }

    public void Initialize(CraftedItem crafted, SellUI parent, int index)
    {
        sellUI    = parent;
        slotIndex = index;

        Transform btnT = button != null ? button.transform : transform;

        // Icon
        Transform iconT = btnT.Find("Icon");
        if (iconT != null)
        {
            Image icon = iconT.GetComponent<Image>();
            if (icon != null)
            {
                icon.sprite  = crafted.item.icon;
                icon.enabled = crafted.item.icon != null;
            }
        }

        // Name
        Transform nameT = btnT.Find("ItemName_Text");
        if (nameT != null)
        {
            var t = nameT.GetComponent<TextMeshProUGUI>();
            if (t != null)
            {
                t.text  = $"{RarityHelper.GetName(crafted.rarity)}\n{crafted.item.itemName}";
                t.color = RarityHelper.GetColor(crafted.rarity);
            }
        }

        // Price
        Transform priceT = btnT.Find("ItemCount_Text");
        if (priceT != null)
        {
            var t = priceT.GetComponent<TextMeshProUGUI>();
            if (t != null) t.text = $"{crafted.GetSellValue()}g";
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        sellUI.SelectItem(slotIndex);
    }

    public Button GetButton() => button;
}
