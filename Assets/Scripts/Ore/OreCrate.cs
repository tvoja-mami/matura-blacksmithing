using UnityEngine;

[CreateAssetMenu(fileName = "New Ore Crate", menuName = "Items/Ore Crate")]
public class OreCrate : ScriptableObject
{
    public string crateType;
    public int cratePrice;
    public int oreAmount;
}
