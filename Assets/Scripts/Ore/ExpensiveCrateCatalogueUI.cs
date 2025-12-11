using UnityEngine;
using TMPro;

public class ExpensiveCrateCatalogueUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI crateTypeText;
    public TextMeshProUGUI cratePriceText;

    [Header("Dependencies")]
    public PlayerGold playerGold;
    public PlayerInventory playerInventory;
    public ItemData oreItem;

    [Header("Crate Settings")]
    public string crateName = "Expensive Ore Crate";
    public int cratePrice = 150;
    public int oreAmount = 30;

    private void Awake()
    {
        if (playerGold == null)
            playerGold = FindFirstObjectByType<PlayerGold>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (oreItem == null)
            oreItem = ResolveDefaultOreItem();
    }

    private void Start()
    {
        if (crateTypeText != null)
            crateTypeText.text = crateName;

        if (cratePriceText != null)
            cratePriceText.text = $"Price: {cratePrice}";

        if (oreItem == null)
        {
            oreItem = ResolveDefaultOreItem();

            if (oreItem == null)
            {
            Debug.LogWarning("ExpensiveCrateCatalogueUI: oreItem is not assigned and no default was found. Drag an ItemData asset into the Ore Item field.", this);
            }
        }
    }

    public void BuyExpensiveCrate()
    {
        if (!CanPurchase())
            return;

        playerGold.RemoveGold(cratePrice);
        playerInventory.AddItem(oreItem, oreAmount);
    }

    private bool CanPurchase()
    {
        if (playerGold == null || playerInventory == null)
        {
            Debug.LogError("ExpensiveCrateCatalogueUI: Missing PlayerGold or PlayerInventory reference.", this);
            return false;
        }

        if (oreItem == null)
        {
            oreItem = ResolveDefaultOreItem();
            if (oreItem == null)
            {
                Debug.LogError("ExpensiveCrateCatalogueUI: oreItem is not assigned and no default was found.", this);
                return false;
            }
        }

        if (playerGold.CurrentGold < cratePrice)
        {
            Debug.Log("ExpensiveCrateCatalogueUI: Not enough gold to purchase this crate.");
            return false;
        }

        return true;
    }
    private ItemData ResolveDefaultOreItem()
    {
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && gameManager.ironOreItem != null)
        {
            Debug.Log("ExpensiveCrateCatalogueUI: Auto-linked ore item from GameManager.", this);
            return gameManager.ironOreItem;
        }

        return null;
    }
}
