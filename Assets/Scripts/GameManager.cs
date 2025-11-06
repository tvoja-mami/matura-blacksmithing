using System;
using UnityEngine;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public ItemData ironOreItem;
    public ItemData ironSwordItem;
	public InventoryUI inventoryUi;
    public int ironOreAmount = 0;
    public int goldAmount = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Try to find references if not assigned
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("GameManager: Could not find PlayerInventory in scene!");
                enabled = false;
                return;
            }
        }

        if (inventoryUi == null)
        {
            inventoryUi = FindObjectOfType<InventoryUI>();
            if (inventoryUi == null)
            {
                Debug.LogError("GameManager: Could not find InventoryUI in scene!");
                enabled = false;
                return;
            }
        }

        // Make sure we have the required ItemData references
        if (ironOreItem == null || ironSwordItem == null)
        {
            Debug.LogError("GameManager: ItemData references (ironOreItem or ironSwordItem) are missing!");
            enabled = false;
            return;
        }

        // Initialize UI with current inventory
        inventoryUi.UpdateInventoryUI(playerInventory);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void GetOre_DEBUG()
	{
		// Add 5 iron ore to the player's inventory using the inventory API
		playerInventory.AddItem(ironOreItem, 5);
		if (playerInventory.items.TryGetValue(ironOreItem, out int newCount))
		{
			Debug.Log($"The player now has {newCount} Iron ores");
		}
		else
		{
			Debug.Log("The player has no Iron ores");
		}
        inventoryUi.UpdateInventoryUI(playerInventory);
	}
    public void CraftSword_DEBUG()
    {
		if (playerInventory.items.TryGetValue(ironOreItem, out int currentCount))
		{
			int removeQuantity = 3;
			if (currentCount >= removeQuantity)
			{
				// remove ores and add crafted sword using PlayerInventory API
				playerInventory.RemoveItem(ironOreItem, removeQuantity);
				playerInventory.AddItem(ironSwordItem, 1);
				int remainingOre = playerInventory.items.ContainsKey(ironOreItem) ? playerInventory.items[ironOreItem] : 0;
				int swordCount = playerInventory.items.ContainsKey(ironSwordItem) ? playerInventory.items[ironSwordItem] : 0;
				Debug.Log($"One iron sword crafted. Remaining: {remainingOre} iron. You have {swordCount} iron swords");
                inventoryUi.UpdateInventoryUI(playerInventory);
			}
			else
			{
				Debug.Log($"Not enough materials. You have {currentCount} materials");
			}
		}
    }
    public void SellSword_DEBUG()
    {
		if (playerInventory.items.TryGetValue(ironSwordItem, out int currentCount))
		{
			if (currentCount >= 1)
			{
				playerInventory.RemoveItem(ironSwordItem, 1);
				int remaining = playerInventory.items.ContainsKey(ironSwordItem) ? playerInventory.items[ironSwordItem] : 0;
				Debug.Log("Removed 1 iron sword. You have " + remaining + " left");
                inventoryUi.UpdateInventoryUI(playerInventory);
			}
		}
		else
		{
			Debug.Log("You don't have an iron sword");
		}
		
    }
	public void GetGold_DEBUG(int amount)
	{
		goldAmount += amount;
		Debug.Log("You have " + goldAmount + " gold.");
	}
}