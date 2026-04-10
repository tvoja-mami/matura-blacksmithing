using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot Prefabs")]
    [Tooltip("Prefab for material slots (left side)")]
    public GameObject leftSlotPrefab;
    [Tooltip("Prefab for crafted item slots (right side)")]
    public GameObject rightSlotPrefab;

    [Header("Materials (left side)")]
    [Tooltip("Content transform inside the materials ScrollRect")]
    public Transform materialsContentParent;

    [Header("Crafted Items (right side)")]
    [Tooltip("Content transform inside the crafted items ScrollRect")]
    public Transform craftedContentParent;

    [Header("Debug")]
    [SerializeField]
    private PlayerInventory playerInventory;

    public bool IsConfigured =>
        leftSlotPrefab != null && rightSlotPrefab != null && materialsContentParent != null && craftedContentParent != null;

    /// <summary>Find the first InventoryUI in the scene that has its references assigned.</summary>
    public static InventoryUI FindConfiguredInstance()
    {
        foreach (var ui in FindObjectsByType<InventoryUI>(FindObjectsSortMode.None))
        {
            if (ui.IsConfigured) return ui;
        }
        return null;
    }

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
            Debug.LogWarning($"InventoryUI on '{gameObject.name}': Missing references (leftSlotPrefab/rightSlotPrefab/materialsContentParent/craftedContentParent). Skipping.");
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
            return;

        playerInventory = inventory;

        ClearChildren(materialsContentParent);
        ClearChildren(craftedContentParent);

        // ── Materials → left side ──
        foreach (var itemEntry in inventory.items)
        {
            ItemData item = itemEntry.Key;
            int quantity = itemEntry.Value;

            if (quantity <= 0 || item == null)
                continue;

            GameObject newSlot = Instantiate(leftSlotPrefab, materialsContentParent);
            InventoryItem itemUI = GetOrCreateInventoryItem(newSlot);

            if (itemUI != null)
            {
                itemUI.item = item;
                itemUI.RefreshUI();
            }
            else
            {
                Debug.LogError($"InventoryUI: Failed to create InventoryItem for {item.name}");
                Destroy(newSlot);
            }
        }

        // ── Crafted items → right side ──
        foreach (CraftedItem craftedItem in inventory.craftedItems)
        {
            if (craftedItem?.item == null) continue;

            GameObject newSlot = Instantiate(rightSlotPrefab, craftedContentParent);
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

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
}
