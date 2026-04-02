using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The prefab for inventory slots. Must have InventoryItem component")]
    public GameObject slotPrefab;
    
    [Tooltip("The parent transform where inventory items will be instantiated")]
    public Transform contentParent;

    [Header("Debug")]
    [SerializeField]
    private PlayerInventory playerInventory;

    private bool IsConfigured => slotPrefab != null && contentParent != null;

    private void OnEnable()
    {
        PlayerInventory.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= HandleInventoryChanged;
    }

    private void Start()
    {
        if (!IsConfigured)
        {
            Debug.LogWarning($"InventoryUI on '{gameObject.name}': Missing references (slotPrefab/contentParent). Skipping.");
            return;
        }

        playerInventory ??= FindFirstObjectByType<PlayerInventory>();
        if (playerInventory != null)
            UpdateInventoryUI(playerInventory);
    }

    private void HandleInventoryChanged()
    {
        playerInventory ??= FindFirstObjectByType<PlayerInventory>();

        if (playerInventory != null)
            UpdateInventoryUI(playerInventory);
    }

    public void UpdateInventoryUI(PlayerInventory inventory)
    {
        if (inventory == null || !IsConfigured)
        {
            return;
        }

        playerInventory = inventory;
        ClearInventorySlots(contentParent);

        // Stackable materials
        foreach (var itemEntry in inventory.items)
        {
            ItemData item = itemEntry.Key;
            int quantity = itemEntry.Value;

            if (quantity <= 0 || item == null)
                continue;

            GameObject newSlot = Instantiate(slotPrefab, contentParent);
            InventoryItem itemUI = GetOrCreateInventoryItem(newSlot);

            if (itemUI != null)
            {
                itemUI.item = item;
                itemUI.RefreshUI();
            }
            else
            {
                Debug.LogError($"InventoryUI: Failed to create or find InventoryItem component for Item: {item.name}");
                Destroy(newSlot);
            }
        }

        // Individual crafted items — each shown separately with rarity
        foreach (CraftedItem craftedItem in inventory.craftedItems)
        {
            if (craftedItem?.item == null) continue;

            GameObject newSlot = Instantiate(slotPrefab, contentParent);
            InventoryItem itemUI = GetOrCreateInventoryItem(newSlot);

            if (itemUI != null)
                itemUI.RefreshAsCraftedItem(craftedItem);
            else
                Destroy(newSlot);
        }
    }

    private InventoryItem GetOrCreateInventoryItem(GameObject slot)
    {
        InventoryItem itemUI = slot.GetComponent<InventoryItem>() ?? slot.AddComponent<InventoryItem>();

        itemUI.iconImage ??= slot.GetComponentInChildren<Image>();
        itemUI.quantityText ??= slot.GetComponentInChildren<TextMeshProUGUI>();

        if (itemUI.nameText == null)
        {
            var allTmps = slot.GetComponentsInChildren<TextMeshProUGUI>();
            if (allTmps != null && allTmps.Length > 1)
                itemUI.nameText = allTmps.FirstOrDefault(t => t != itemUI.quantityText);
        }

        return itemUI;
    }

    private void ClearInventorySlots(Transform gridContent)
    {
        if (gridContent == null) return;

        for (int i = gridContent.childCount - 1; i >= 0; i--)
        {
            Transform child = gridContent.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
