using System.Collections.Generic;
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
    [SerializeField] private Button sellButton;

    [Header("Selection Colours")]
    public Color selectedColour = new Color(0.85f, 0.65f, 0.25f, 1f);
    public Color normalColour   = new Color(0.40f, 0.40f, 0.40f, 1f);

    private bool isOpen;
    private int selectedIndex = -1;
    private readonly List<SellSlotUI> slots = new List<SellSlotUI>();

    private void Start()
    {
        playerInventory ??= FindFirstObjectByType<PlayerInventory>();
        playerGold      ??= FindFirstObjectByType<PlayerGold>();

        if (sellButton != null)
            sellButton.onClick.AddListener(SellSelectedItem);
    }

    // ────────── Open / Close ──────────

    public void OpenShop()
    {
        isOpen = true;
        sellPanel.SetActive(true);
        PlayerMovement.ActiveMenuCount++;
        RefreshItemList();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIOpen();
    }

    public void CloseShop()
    {
        isOpen = false;
        sellPanel.SetActive(false);
        PlayerMovement.ActiveMenuCount--;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIClose();
    }

    public void ToggleShop()
    {
        if (isOpen) CloseShop();
        else OpenShop();
    }

    // ────────── Selection (called by SellSlotUI) ──────────

    public void SelectItem(int index)
    {
        if (index < 0 || index >= playerInventory.craftedItems.Count) return;

        selectedIndex = index;

        if (sellButton != null)
            sellButton.interactable = true;

        UpdateButtonColours();
        UpdateGoldDisplay();
    }

    private void ClearSelection()
    {
        selectedIndex = -1;
        if (sellButton != null)
            sellButton.interactable = false;
        UpdateButtonColours();
    }

    // ────────── Button Colours (same as CrateCatalogueUI) ──────────

    private void UpdateButtonColours()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Button btn = slots[i].GetButton();
            if (btn == null) continue;

            Color tint = i == selectedIndex ? selectedColour : normalColour;

            var img = btn.GetComponent<Image>();
            if (img != null) img.color = Color.white;

            var colours = btn.colors;
            colours.normalColor      = tint;
            colours.highlightedColor = tint;
            colours.selectedColor    = tint;
            colours.colorMultiplier  = 1f;
            btn.colors = colours;
        }
    }

    // ────────── Item List ──────────

    private void RefreshItemList()
    {
        for (int i = itemListContainer.childCount - 1; i >= 0; i--)
            Destroy(itemListContainer.GetChild(i).gameObject);

        slots.Clear();
        ClearSelection();

        for (int i = 0; i < playerInventory.craftedItems.Count; i++)
        {
            CraftedItem crafted = playerInventory.craftedItems[i];
            if (crafted?.item == null) continue;

            GameObject obj = Instantiate(sellSlotPrefab, itemListContainer);
            SellSlotUI slot = obj.GetComponent<SellSlotUI>();
            if (slot == null) slot = obj.AddComponent<SellSlotUI>();

            slot.Initialize(crafted, this, i);
            slots.Add(slot);
        }

        // Auto-select first item
        if (slots.Count > 0)
            SelectItem(0);

        UpdateGoldDisplay();
    }

    // ────────── Sell ──────────

    public void SellSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= playerInventory.craftedItems.Count) return;

        int nextIndex = selectedIndex;

        CraftedItem item = playerInventory.craftedItems[selectedIndex];
        playerGold.AddGold(item.GetSellValue());
        playerInventory.RemoveCraftedItem(item);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayCoinSound();

        RefreshItemList();

        // Auto-select next item (or last if we sold the last one)
        if (slots.Count > 0)
            SelectItem(Mathf.Min(nextIndex, slots.Count - 1));
    }

    private void UpdateGoldDisplay()
    {
        if (goldText != null && playerGold != null)
            goldText.text = $"Gold: {playerGold.CurrentGold}g";
    }
}
