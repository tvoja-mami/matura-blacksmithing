using UnityEngine;
using System.Collections.Generic;
using System; // Required for Action

public class PlayerInventory : MonoBehaviour
{
    /// <summary>Stackable materials (ore, etc.).</summary>
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    /// <summary>Individual crafted items, each with its own rarity.</summary>
    public List<CraftedItem> craftedItems = new List<CraftedItem>();

    public static event Action OnInventoryChanged;
    //dodaj item
    public void AddItem(ItemData item, int quantity)
    {
        if (items.ContainsKey(item))
        {
            items[item] += quantity;
        }
        else
        {
            items.Add(item, quantity);
        }
        OnInventoryChanged?.Invoke();
    }
    /// <summary>Add a crafted item as a separate instance with its own rarity.</summary>
    public void AddItemWithRarity(ItemData item, int quantity, ItemRarity rarity)
    {
        for (int i = 0; i < quantity; i++)
            craftedItems.Add(new CraftedItem(item, rarity));

        OnInventoryChanged?.Invoke();
    }

    /// <summary>Remove a specific crafted item instance.</summary>
    public void RemoveCraftedItem(CraftedItem craftedItem)
    {
        if (craftedItems.Remove(craftedItem))
            OnInventoryChanged?.Invoke();
    }

    //odstrani item
    public void RemoveItem(ItemData item, int quantity)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= quantity;
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
            OnInventoryChanged?.Invoke();
        }
    }

}