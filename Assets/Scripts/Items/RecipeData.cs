using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemQuantity
{
    public ItemData item;
    public int amount;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("UI Info")]
    public string recipeName;

    public int recipeID;

    [Header("Crafting Data")]
    public List<ItemQuantity> requiredMaterials;
    public ItemData outputItem;

    [Header("Progression")]
    [Tooltip("Minimum player level required to see and use this recipe.")]
    public int requiredLevel = 1;
    [Tooltip("Base XP awarded on craft. Multiplied by rarity (Common 1×, Uncommon 1.3×, Rare 1.6×, Legendary 2×).")]
    public int craftXP = 20;
}