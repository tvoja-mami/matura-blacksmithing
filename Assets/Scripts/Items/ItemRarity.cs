using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

public static class RarityHelper
{
    /// <summary>Sell price multiplier for each rarity tier.</summary>
    public static float GetValueMultiplier(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => 1.0f,
        ItemRarity.Uncommon  => 1.5f,
        ItemRarity.Rare      => 2.5f,
        ItemRarity.Legendary => 5.0f,
        _                    => 1.0f
    };

    public static Color GetColor(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => Color.white,
        ItemRarity.Uncommon  => Color.green,
        ItemRarity.Rare      => new Color(0.4f, 0.6f, 1f),   // blue
        ItemRarity.Legendary => new Color(1f, 0.6f, 0f),     // gold
        _                    => Color.white
    };

    /// <summary>Muted background tint per rarity tier.</summary>
    public static Color GetBackgroundColor(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => new Color(0.85f, 0.85f, 0.85f),        // light grey
        ItemRarity.Uncommon  => new Color(0.7f,  0.92f, 0.7f),         // soft green
        ItemRarity.Rare      => new Color(0.7f,  0.78f, 1f),           // soft blue
        ItemRarity.Legendary => new Color(1f,    0.85f, 0.5f),         // soft gold
        _                    => new Color(0.85f, 0.85f, 0.85f)
    };

    public static string GetName(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => "Common",
        ItemRarity.Uncommon  => "Uncommon",
        ItemRarity.Rare      => "Rare",
        ItemRarity.Legendary => "Legendary",
        _                    => "Common"
    };

    /// <summary>
    /// Weighted RNG roll. Higher quality = better odds at rare/legendary.
    /// Quality is the main driver but RNG means any outcome is always possible.
    /// </summary>
    public static ItemRarity RollRarity(float qualityPct)
    {
        // weights: [Common, Uncommon, Rare, Legendary]
        float[] weights = qualityPct switch
        {
            >= 85f => new float[] {  5f, 25f, 40f, 30f },
            >= 65f => new float[] { 20f, 45f, 30f,  5f },
            >= 40f => new float[] { 50f, 35f, 13f,  2f },
            _      => new float[] { 75f, 20f,  5f,  0f }
        };

        float total = 0f;
        foreach (float w in weights) total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        ItemRarity[] rarities = { ItemRarity.Common, ItemRarity.Uncommon, ItemRarity.Rare, ItemRarity.Legendary };

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return rarities[i];
        }

        return ItemRarity.Common;
    }
}
