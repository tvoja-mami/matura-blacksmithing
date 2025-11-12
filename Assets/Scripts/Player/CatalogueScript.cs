using UnityEngine;

public class CatalogueScript : MonoBehaviour
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
    private bool isCatalogueOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
