using UnityEngine;

/// <summary>A single crafted item instance with its own rarity.</summary>
[System.Serializable]
public class CraftedItem
{
    public ItemData item;
    public ItemRarity rarity;

    public CraftedItem(ItemData item, ItemRarity rarity)
    {
        this.item   = item;
        this.rarity = rarity;
    }

    public int GetSellValue() =>
        Mathf.RoundToInt(item.baseValue * RarityHelper.GetValueMultiplier(rarity));
}
