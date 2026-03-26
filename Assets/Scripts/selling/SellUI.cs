using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerGold playerGold;

    [Header("UI")]
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private Transform itemListContainer;
    [SerializeField] private GameObject sellSlotPrefab;
    [SerializeField] private TextMeshProUGUI goldText;

    private bool isOpen;

    private void Start()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (playerGold == null)
            playerGold = FindFirstObjectByType<PlayerGold>();
    }

    // ────────── Open / Close (same pattern as ForgeUI) ──────────

    public void OpenShop()
    {
        isOpen = true;
        sellPanel.SetActive(true);
        PlayerMovement.ActiveMenuCount++;
        UpdateGoldDisplay();
        RefreshItemList();
    }

    public void CloseShop()
    {
        isOpen = false;
        sellPanel.SetActive(false);
        PlayerMovement.ActiveMenuCount--;
    }

    public void ToggleShop()
    {
        if (isOpen) CloseShop();
        else OpenShop();
    }

    // ────────── Build the list of sellable items ──────────

    private void RefreshItemList()
    {
        ClearChildren(itemListContainer);

        foreach (CraftedItem crafted in playerInventory.craftedItems)
        {
            if (crafted?.item == null) continue;

            GameObject slot = Instantiate(sellSlotPrefab, itemListContainer);

            // Expect the prefab to have child TextMeshPro objects:
            //   "ItemName_Text"  — for the item name + rarity
            //   "ItemCount_Text" — for the sell price
            TextMeshProUGUI nameText = null;
            TextMeshProUGUI priceText = null;

            Transform nameT = slot.transform.Find("ItemName_Text");
            if (nameT != null) nameText = nameT.GetComponent<TextMeshProUGUI>();

            Transform priceT = slot.transform.Find("ItemCount_Text");
            if (priceT != null) priceText = priceT.GetComponent<TextMeshProUGUI>();

            // Icon
            Transform iconT = slot.transform.Find("Icon");
            if (iconT != null)
            {
                Image icon = iconT.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = crafted.item.icon;
                    icon.enabled = crafted.item.icon != null;
                }
            }

            // Name with rarity colour
            if (nameText != null)
            {
                nameText.text = $"{RarityHelper.GetName(crafted.rarity)}\n{crafted.item.itemName}";
                nameText.color = RarityHelper.GetColor(crafted.rarity);
            }

            // Sell value
            if (priceText != null)
                priceText.text = $"{crafted.GetSellValue()}g";

            // Rarity background tint
            Image bg = slot.GetComponent<Image>();
            if (bg != null)
                bg.color = RarityHelper.GetBackgroundColor(crafted.rarity);

            // Hook up sell button
            Button sellBtn = slot.GetComponentInChildren<Button>();
            if (sellBtn != null)
            {
                CraftedItem captured = crafted;
                sellBtn.onClick.AddListener(() => SellItem(captured));
            }
        }
    }

    // ────────── Sell ──────────

    private void SellItem(CraftedItem crafted)
    {
        int value = crafted.GetSellValue();
        playerGold.AddGold(value);
        playerInventory.RemoveCraftedItem(crafted);
        UpdateGoldDisplay();
        RefreshItemList();
    }

    private void UpdateGoldDisplay()
    {
        if (goldText != null && playerGold != null)
            goldText.text = $"Gold: {playerGold.CurrentGold}g";
    }

    // ────────── Util ──────────

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
}
