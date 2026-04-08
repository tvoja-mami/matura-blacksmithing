using UnityEngine;

[CreateAssetMenu(fileName = "New Ore Crate", menuName = "Items/Ore Crate")]
public class OreCrate : ScriptableObject
{
    public string crateType;
    public int cratePrice;
    public int requiredLevel = 1;
    public Sprite crateIcon;
    [TextArea] public string description;

    [Header("Crate Contents")]
    public CrateDrop[] drops;
}

[System.Serializable]
public class CrateDrop
{
    public ItemData item;
    public int amount;
}
