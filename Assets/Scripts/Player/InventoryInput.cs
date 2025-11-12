using UnityEngine;
using UnityEngine.InputSystem;

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
            HideInventory();
            Debug.Log("InventoryInput: Inventory panel found and hidden");
        }
        else
        {
            Debug.LogError("InventoryInput: inventoryPanel is NOT assigned in Inspector!");
        }
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
            // Fallback to SetActive
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
}
