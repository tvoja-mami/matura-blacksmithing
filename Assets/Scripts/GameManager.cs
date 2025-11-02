using System;
using UnityEngine;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public ItemData ironOreItem;
    public ItemData ironSwordItem;
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
        for (int i = 0; i < 5; i++)
        {
            playerInventory.items.Add(ironOreItem);
        }
        Debug.Log("The player has "+playerInventory.items.Count+"Iron ores");
    }
    public void CraftSword_DEBUG()
    {
        int ironOreCount = playerInventory.items.Count(item => item == ironOreItem);
        if (ironOreCount >= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                playerInventory.items.Remove(ironOreItem);
            }

            playerInventory.items.Add(ironSwordItem);
            int ironSwordCount = playerInventory.items.Count(item => item == ironSwordItem);
            Debug.Log("The player has " + ironSwordCount + " Swords");
        }
    }
    public void SellSword_DEBUG()
    {
        if (playerInventory.items.Contains(ironSwordItem))
        {
            playerInventory.items.Remove(ironSwordItem);
            int remainingSwords = playerInventory.items.Count(item => item == ironSwordItem);
            Debug.Log("The player has " + remainingSwords + " Swords");
        }
        else
        {
            Debug.Log("You dont have an iron sword");
        }
    }
        
}