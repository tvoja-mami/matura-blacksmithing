using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

public class InventoryInput : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The inventory UI panel to show/hide")]
    public GameObject inventoryPanel;
    
    [Tooltip("Optional: Use CanvasGroup for smooth fade. If not set, will use SetActive")]
    public CanvasGroup canvasGroup;
    
    [Tooltip("Reference to InventoryUI to force refresh when opening")]
    public InventoryUI inventoryUI;
    
    [Tooltip("Reference to PlayerInventory")]
    public PlayerInventory playerInventory;
    
    private PlayerControls controls;
    private bool isInventoryOpen = false;
    private bool inputReady = false;

    void Awake()
    {
        // Initialize controls in Awake (before OnEnable)
        controls = new PlayerControls();
        Debug.Log("InventoryInput: Controls created in Awake");
    }

    void Start()
    {
        Debug.Log("InventoryInput: Start called");
        
        // Auto-find references if not assigned
        if (inventoryUI == null)
            inventoryUI = FindFirstObjectByType<InventoryUI>();
        
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        
        // Make sure inventory starts hidden
        if (inventoryPanel != null)
        {
            // Ensure we have a CanvasGroup so we can hide without disabling the GameObject
            if (canvasGroup == null)
            {
                canvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = inventoryPanel.AddComponent<CanvasGroup>();
                }
            }

            // Keep panel active so InventoryUI stays subscribed; hide via CanvasGroup
            inventoryPanel.SetActive(true);
            HideInventory();
            Debug.Log("InventoryInput: Inventory panel found and hidden via CanvasGroup");
        }
        else
        {
            Debug.LogError("InventoryInput: inventoryPanel is NOT assigned in Inspector!");
        }

        inputReady = true;
    }

    void OnEnable()
    {
        if (controls != null)
        {
            controls.Gameplay.Enable();
            controls.Gameplay.OpenInventory.performed += OnOpenInventory;
            Debug.Log("InventoryInput: Controls enabled and subscribed to OpenInventory");
        }
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Gameplay.Disable();
            controls.Gameplay.OpenInventory.performed -= OnOpenInventory;
        }
    }

    private void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (!inputReady)
            return;

        Debug.Log("OnOpenInventory called!");
        
        if (inventoryPanel == null)
        {
            Debug.LogWarning("InventoryInput: inventoryPanel is not assigned!");
            return;
        }

        // Toggle inventory visibility
        isInventoryOpen = !isInventoryOpen;
        
        if (isInventoryOpen)
        {
            ShowInventory();
        }
        else
        {
            HideInventory();
        }
        
        Debug.Log($"Inventory is now {(isInventoryOpen ? "open" : "closed")}");
    }

    private void ShowInventory()
    {
        if (canvasGroup != null)
        {
            // Use CanvasGroup for smoother control
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            // Fallback to SetActive
            inventoryPanel.SetActive(true);
        }
        
        // Force refresh the inventory UI when opening
        if (inventoryUI != null && playerInventory != null)
        {
            inventoryUI.UpdateInventoryUI(playerInventory);
            Debug.Log("InventoryInput: Refreshed inventory UI");
        }

        LogInventoryContents();
    }

    private void HideInventory()
    {
        isInventoryOpen = false;
        
        if (canvasGroup != null)
        {
            // Use CanvasGroup - keeps components enabled but invisible
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            // Fallback to SetActive (last resort)
            inventoryPanel.SetActive(false);
        }
    }

    // Public method to close inventory
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            HideInventory();
        }
    }

    // Public method to open inventory
    public void OpenInventory()
    {
        if (inventoryPanel != null)
        {
            isInventoryOpen = true;
            ShowInventory();
        }
    }

    private void LogInventoryContents()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("InventoryInput: PlayerInventory reference missing; cannot log inventory contents.");
            return;
        }

        if (playerInventory.items == null || playerInventory.items.Count == 0)
        {
            Debug.Log("InventoryInput: Inventory dictionary empty when opening inventory.");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("InventoryInput: Inventory contents when opening:");
        foreach (var entry in playerInventory.items)
        {
            string itemName = entry.Key != null ? entry.Key.itemName : "<null ItemData>";
            sb.AppendLine($" - {itemName}: {entry.Value}");
        }

        Debug.Log(sb.ToString());
    }
}
