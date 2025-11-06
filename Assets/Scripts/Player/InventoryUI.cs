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
        // Validate required references
        if (slotPrefab == null)
        {
            Debug.LogError("InventoryUI: slotPrefab reference is missing!");
            enabled = false;
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("InventoryUI: contentParent reference is missing!");
            enabled = false;
            return;
        }

        // Initial UI update if we have an inventory reference
        if (playerInventory != null)
        {
            UpdateInventoryUI(playerInventory);
        }
    }

    // Called whenever the inventory changes (through the event system)
    private void HandleInventoryChanged()
    {
        if (playerInventory != null)
        {
            UpdateInventoryUI(playerInventory);
        }
    }

    /// <summary>
    /// Updates the entire inventory UI display with the current inventory contents
    /// </summary>
    /// <param name="inventory">The PlayerInventory to display</param>
    public void UpdateInventoryUI(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogWarning("InventoryUI: Attempted to update with null inventory!");
            return;
        }

        // Cache the inventory reference for event handling
        playerInventory = inventory;

        // Clear existing slots
        ClearInventorySlots(contentParent);

        // Create slots for all items with quantity > 0
        foreach (var itemEntry in inventory.items)
        {
            ItemData item = itemEntry.Key;
            int quantity = itemEntry.Value;

            if (quantity <= 0) continue;

            // Instantiate a new slot
            GameObject newSlot = Instantiate(slotPrefab, contentParent);
            
            // Try to get the InventoryItem component from the instantiated slot
            InventoryItem itemUI = newSlot.GetComponent<InventoryItem>();

            if (itemUI == null)
            {
                // Fallback: if the prefab doesn't have the InventoryItem script,
                // create one on the instantiated object and try to wire basic UI children.
                itemUI = newSlot.AddComponent<InventoryItem>();

                // Try to auto-find an Image and a TextMeshProUGUI in children
                var image = newSlot.GetComponentInChildren<Image>();
                var tmp = newSlot.GetComponentInChildren<TextMeshProUGUI>();

                if (image != null)
                {
                    itemUI.iconImage = image;
                }
                if (tmp != null)
                {
                    itemUI.quantityText = tmp;
                }
                // nameText is optional; try to find a second TMP (if present)
                var allTmps = newSlot.GetComponentsInChildren<TextMeshProUGUI>();
                if (allTmps != null && allTmps.Length > 1)
                {
                    itemUI.nameText = allTmps.FirstOrDefault(t => t != itemUI.quantityText);
                }
            }

            if (itemUI != null)
            {
                // Update the slot with item data
                itemUI.item = item;

                // Force an immediate refresh of the slot's UI
                itemUI.RefreshUI();
            }
            else
            {
                Debug.LogError($"InventoryUI: Failed to create or find InventoryItem component for Item: {item.name}");
                Destroy(newSlot);
            }
        }
        Debug.Log("InventoryUI: UI updated with current inventory contents.");
    }

    /// <summary>
    /// Safely destroys all child slot objects in the content parent
    /// </summary>
    private void ClearInventorySlots(Transform gridContent)
    {
        if (gridContent == null) return;

        // Cache children count as it can change during destruction
        int childCount = gridContent.childCount;
        
        // Destroy from last to first to avoid reindexing issues
        for (int i = childCount - 1; i >= 0; i--)
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
