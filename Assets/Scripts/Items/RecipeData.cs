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
}