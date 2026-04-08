using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic
}

public static class RarityHelper
{
    /// <summary>Sell price multiplier for each rarity tier.</summary>
    public static float GetValueMultiplier(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => 1.0f,
        ItemRarity.Uncommon  => 1.5f,
        ItemRarity.Rare      => 2.5f,
        ItemRarity.Epic      => 4.0f,
        ItemRarity.Legendary => 7.0f,
        ItemRarity.Mythic    => 12.0f,
        _                    => 1.0f
    };

    /// <summary>Text/name colour per rarity tier.</summary>
    public static Color GetColor(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => new Color(0.6f,  0.6f,  0.6f),   // dark grey
        ItemRarity.Uncommon  => new Color(0.1f,  0.5f,  0.1f),   // dark green
        ItemRarity.Rare      => new Color(0.2f,  0.35f, 0.7f),   // dark blue
        ItemRarity.Epic      => new Color(0.45f, 0.15f, 0.7f),   // dark purple
        ItemRarity.Legendary => new Color(0.7f,  0.4f,  0f),     // dark gold
        ItemRarity.Mythic    => new Color(0.7f,  0.1f,  0.1f),   // dark red
        _                    => new Color(0.6f,  0.6f,  0.6f)
    };

    /// <summary>Muted background tint per rarity tier.</summary>
    public static Color GetBackgroundColor(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => new Color(0.85f, 0.85f, 0.85f),  // light grey
        ItemRarity.Uncommon  => new Color(0.7f,  0.92f, 0.7f),   // soft green
        ItemRarity.Rare      => new Color(0.7f,  0.78f, 1f),     // soft blue
        ItemRarity.Epic      => new Color(0.88f, 0.75f, 1f),     // soft purple
        ItemRarity.Legendary => new Color(1f,    0.85f, 0.5f),   // soft gold
        ItemRarity.Mythic    => new Color(1f,    0.6f,  0.6f),   // soft red
        _                    => new Color(0.85f, 0.85f, 0.85f)
    };

    /// <summary>XP multiplier applied to a recipe's craftXP based on the rarity rolled.</summary>
    public static float GetXPMultiplier(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => 1.0f,
        ItemRarity.Uncommon  => 1.3f,
        ItemRarity.Rare      => 1.6f,
        ItemRarity.Epic      => 2.0f,
        ItemRarity.Legendary => 2.5f,
        ItemRarity.Mythic    => 3.0f,
        _                    => 1.0f
    };

    public static string GetName(ItemRarity rarity) => rarity switch
    {
        ItemRarity.Common    => "Common",
        ItemRarity.Uncommon  => "Uncommon",
        ItemRarity.Rare      => "Rare",
        ItemRarity.Epic      => "Epic",
        ItemRarity.Legendary => "Legendary",
        ItemRarity.Mythic    => "Mythic",
        _                    => "Common"
    };

    /// <summary>
    /// Roll rarity based on quality and player luck (0 at level 1, +1 per level, caps at LuckCap).
    /// Per luck point: Common -1.5%, Uncommon -0.5%, Rare +1%, Epic +0.5%, Legendary +0.3%, Mythic +0.2%
    /// </summary>
    public static ItemRarity RollRarity(float qualityPct, int luck = 0)
    {
        // Base weights [Common, Uncommon, Rare, Epic, Legendary, Mythic]
        float[] weights = qualityPct switch
        {
            >= 85f => new float[] { 20f, 40f, 26f,  8f, 4f, 2f },
            >= 65f => new float[] { 40f, 36f, 16f,  5f, 2f, 1f },
            >= 40f => new float[] { 62f, 26f,  8f, 2.5f, 1f, 0.5f },
            _      => new float[] { 80f, 15f, 3.5f,  1f, 0.5f, 0f }
        };

        // Luck shifts weight from Common/Uncommon toward rarer tiers
        weights[0] = Mathf.Max(0f, weights[0] - luck * 1.5f);
        weights[1] = Mathf.Max(0f, weights[1] - luck * 0.5f);
        weights[2] += luck * 1.0f;
        weights[3] += luck * 0.5f;
        weights[4] += luck * 0.3f;
        weights[5] += luck * 0.2f;

        float total = 0f;
        foreach (float w in weights) total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        ItemRarity[] rarities =
        {
            ItemRarity.Common, ItemRarity.Uncommon, ItemRarity.Rare,
            ItemRarity.Epic,   ItemRarity.Legendary, ItemRarity.Mythic
        };

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return rarities[i];
        }

        return ItemRarity.Common;
    }
}
