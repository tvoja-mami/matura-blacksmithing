using UnityEngine;
using System.Collections.Generic;
using System; // Required for Action

public class PlayerInventory : MonoBehaviour
{
    // Use a Dictionary to store items and their quantities
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    // This is a C# Event. Other scripts can "subscribe" to it.
    // When the inventory changes, we'll invoke this event.
    public static event Action OnInventoryChanged;

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
        // Announce that the inventory has changed!
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemData item, int quantity)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= quantity;
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
            // Announce that the inventory has changed!
            OnInventoryChanged?.Invoke();
        }
    }
}