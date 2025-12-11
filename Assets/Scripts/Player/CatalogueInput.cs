using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

public class CatalogueInput : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The catalogue UI panel to show/hide")]
    public GameObject cataloguePanel;

    [Tooltip("Optional: Use CanvasGroup for smooth fade. If not set, will use SetActive")]
    public CanvasGroup canvasGroup;

    [Tooltip("Optional: Refresh inventory UI when opening (if your catalogue shares the inventory list)")]
    public InventoryUI inventoryUI;

    [Tooltip("Optional: PlayerInventory used to refresh UI")] 
    public PlayerInventory playerInventory;

    private PlayerControls controls;
    private bool isCatalogueOpen = false;
    private bool inputReady = false;

    private void Awake()
    {
        controls = new PlayerControls();
        Debug.Log("CatalogueInput: Controls created in Awake");
    }

    private void Start()
    {
        // Auto-find references if not assigned
        if (inventoryUI == null)
            inventoryUI = FindFirstObjectByType<InventoryUI>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (cataloguePanel != null)
        {
            // Ensure we have a CanvasGroup so we can hide without disabling the GameObject
            if (canvasGroup == null)
            {
                canvasGroup = cataloguePanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = cataloguePanel.AddComponent<CanvasGroup>();
                }
            }

            // Keep panel active so any listeners stay subscribed; hide via CanvasGroup
            cataloguePanel.SetActive(true);
            HideCatalogue();
            Debug.Log("CatalogueInput: Catalogue panel found and hidden via CanvasGroup");
        }
        else
        {
            Debug.LogError("CatalogueInput: cataloguePanel is NOT assigned in Inspector!");
        }

        inputReady = true;
    }

    private void OnEnable()
    {
        if (controls != null)
        {
            controls.Gameplay.Enable();
            controls.Gameplay.OpenCatalogue.performed += OnOpenCatalogue;
            Debug.Log("CatalogueInput: Controls enabled and subscribed to OpenCatalogue");
        }
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Gameplay.OpenCatalogue.performed -= OnOpenCatalogue;
            controls.Gameplay.Disable();
        }
    }

    private void OnOpenCatalogue(InputAction.CallbackContext context)
    {
        if (!inputReady)
            return;

        Debug.Log("OnOpenCatalogue called!");

        if (cataloguePanel == null)
        {
            Debug.LogWarning("CatalogueInput: cataloguePanel is not assigned!");
            return;
        }

        // Toggle catalogue visibility
        isCatalogueOpen = !isCatalogueOpen;

        if (isCatalogueOpen)
        {
            ShowCatalogue();
        }
        else
        {
            HideCatalogue();
        }

        Debug.Log($"Catalogue is now {(isCatalogueOpen ? "open" : "closed")}");
    }

    private void ShowCatalogue()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            cataloguePanel.SetActive(true);
        }

        // Optional refresh if your catalogue uses the same inventory view
        if (inventoryUI != null && playerInventory != null)
        {
            inventoryUI.UpdateInventoryUI(playerInventory);
            Debug.Log("CatalogueInput: Refreshed inventory UI");
        }

        LogInventoryContents();
    }

    private void HideCatalogue()
    {
        isCatalogueOpen = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            cataloguePanel.SetActive(false);
        }
    }

    public void CloseCatalogue()
    {
        if (cataloguePanel != null)
        {
            HideCatalogue();
        }
    }

    public void OpenCatalogue()
    {
        if (cataloguePanel != null)
        {
            isCatalogueOpen = true;
            ShowCatalogue();
        }
    }

    private void LogInventoryContents()
    {
        if (playerInventory == null)
            return;

        if (playerInventory.items == null || playerInventory.items.Count == 0)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("CatalogueInput: Inventory contents when opening catalogue:");
        foreach (var entry in playerInventory.items)
        {
            string itemName = entry.Key != null ? entry.Key.itemName : "<null ItemData>";
            sb.AppendLine($" - {itemName}: {entry.Value}");
        }

        Debug.Log(sb.ToString());
    }
}
