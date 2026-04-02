using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData item;

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI nameText;

    // Fallback for projects that use the legacy UI Text instead of TMPro
    private UnityEngine.UI.Text legacyQuantityText;

    private PlayerInventory playerInventory;

    private void Awake()
    {
        // Get references if not set
        if (iconImage == null)
        {
            // Look for child named "Icon" - NOT the root Image
            Transform iconTransform = transform.Find("Icon");
            if (iconTransform != null)
            {
                iconImage = iconTransform.GetComponent<Image>();
            }
            
            // If still null, don't use GetComponent on self - that would get the root's Image
            if (iconImage == null)
            {
                Debug.LogWarning($"InventoryItem: Could not find 'icon' child on {gameObject.name}");
            }
        }

        if (playerInventory == null) playerInventory = FindFirstObjectByType<PlayerInventory>();

        // Try to find TextMeshProUGUI children by name
        if (quantityText == null)
        {
            Transform quantityTransform = transform.Find("ItemCount_Text");
            if (quantityTransform != null)
                quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
            
            // Fallback to legacy Text if TMP not found
            if (quantityText == null && quantityTransform != null)
                legacyQuantityText = quantityTransform.GetComponent<UnityEngine.UI.Text>();
        }

        if (nameText == null)
        {
            Transform nameTransform = transform.Find("ItemName_Text");
            if (nameTransform != null)
                nameText = nameTransform.GetComponent<TextMeshProUGUI>();
        }
    }
    /// <summary>Display a crafted item instance with rarity colour on the name.</summary>
    public void RefreshAsCraftedItem(CraftedItem craftedItem)
    {
        item = craftedItem.item;

        if (iconImage != null)
        {
            iconImage.sprite  = item.icon;
            iconImage.enabled = item.icon != null;
        }

        if (nameText != null)
        {
            nameText.text     = $"{RarityHelper.GetName(craftedItem.rarity)}\n{item.itemName}";
            nameText.color    = RarityHelper.GetColor(craftedItem.rarity);
            nameText.fontSize = nameText.fontSize * 0.75f;
            nameText.enableAutoSizing = false;
        }

        if (quantityText != null)
            quantityText.text = $"{craftedItem.GetSellValue()}g";

        // Tint slot background to match rarity
        Image bg = GetComponent<Image>();
        if (bg != null)
            bg.color = RarityHelper.GetBackgroundColor(craftedItem.rarity);
    }

    public void RefreshUI()
    {
        if (item != null && playerInventory != null)
        {
            // Get current quantity from player inventory
            int quantity = 0;
            playerInventory.items.TryGetValue(item, out quantity);

            // Update icon - with null check
            if (iconImage != null)
            {
                iconImage.sprite = item.icon;
                iconImage.enabled = true;
            }

            // Update texts (support both TMPro and legacy Text)
            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
            }
            else if (legacyQuantityText != null)
            {
                legacyQuantityText.text = quantity.ToString();
            }

            if (nameText != null) nameText.text = item.itemName;
        }
        else
        {
            // Clear the slot
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (quantityText != null)
            {
                quantityText.text = "";
            }
            else if (legacyQuantityText != null)
            {
                legacyQuantityText.text = "";
            }

            if (nameText != null) nameText.text = "";
        }
    }
}
