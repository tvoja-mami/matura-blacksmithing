using UnityEngine;
using System.Collections.Generic;
using System; // Required for Action

public class PlayerInventory : MonoBehaviour
{
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

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