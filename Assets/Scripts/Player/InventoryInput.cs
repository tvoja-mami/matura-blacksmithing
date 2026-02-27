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
    private bool isInventoryOpen;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void Start()
    {
        if (inventoryUI == null)
            inventoryUI = FindFirstObjectByType<InventoryUI>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (inventoryPanel != null)
        {
            if (canvasGroup == null)
            {
                canvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = inventoryPanel.AddComponent<CanvasGroup>();
            }

            inventoryPanel.SetActive(true);
            SetInventoryVisible(false);
        }
        else
        {
            Debug.LogWarning("InventoryInput: inventoryPanel is not assigned.");
        }
    }

    void OnEnable()
    {
        if (controls == null)
            return;

        controls.Gameplay.OpenInventory.performed += OnOpenInventory;
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        if (controls == null)
            return;

        controls.Gameplay.OpenInventory.performed -= OnOpenInventory;
        controls.Gameplay.Disable();
    }

    private void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (inventoryPanel == null)
            return;

        SetInventoryVisible(!isInventoryOpen);
    }

    private void SetInventoryVisible(bool visible)
    {
        isInventoryOpen = visible;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        else
        {
            inventoryPanel.SetActive(visible);
        }

        if (visible && inventoryUI != null && playerInventory != null)
            inventoryUI.UpdateInventoryUI(playerInventory);
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
            SetInventoryVisible(false);
    }

    public void OpenInventory()
    {
        if (inventoryPanel != null)
            SetInventoryVisible(true);
    }
}
