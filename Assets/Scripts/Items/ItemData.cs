using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject 
{
    public string itemName;
	public int itemID;
    public Sprite icon;
    public int baseValue;
    public string description;
    public int quantity;
}