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
    public int goldAmount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void GetOre_DEBUG()
	{
    	if (playerInventory.items.TryGetValue(ironOreItem, out int currentCount))
    		{
        		playerInventory.items[ironOreItem] = currentCount + 5;
    		}
    	else
    		{
        		playerInventory.items[ironOreItem] = 5;
    		}
    
    	Debug.Log($"The player now has {playerInventory.items[ironOreItem]} Iron ores");
	}
    public void CraftSword_DEBUG()
    {
        if (playerInventory.items.TryGetValue(ironOreItem, out int currentCount))
			{
    			int removeQuantity = 3;
    			if (currentCount > removeQuantity)
    				{
        				if(playerInventory.items.TryGetValue(ironSwordItem, out int currentCountSword))
							{
								playerInventory.items[ironOreItem] = currentCount - removeQuantity;
								Debug.Log($"One iron sword crafted. Remaining: {currentCount-3} iron. You have {currentCountSword} iron swords");
								playerInventory.items[ironSwordItem] += 1;
							}
						else
							{
								playerInventory.items[ironSwordItem] = 1;
								Debug.Log($"You have one iron sword. You have {currentCount-3} iron remaining");
							}
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
    			if (currentCount > 1)
    				{
        				// izbrise en iron sword
        				playerInventory.items[ironSwordItem] = currentCount - 1;
        				Debug.Log("Removed 1 iron sword. You have " + (currentCount - 1) + " left");
    				}
			}
		else
			{
    			Debug.Log("You don't have an iron sword");
			}
		
    }
        
}