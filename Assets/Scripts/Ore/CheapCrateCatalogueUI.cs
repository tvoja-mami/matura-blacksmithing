using UnityEngine;
using System.Collections;
using TMPro;
using image = UnityEngine.UI.Image;


public class CheapCrateCatalogueUI : MonoBehaviour
{
    public OreCrate oreCrate;
    public TextMeshProUGUI crateTypeText;
    public TextMeshProUGUI cratePriceText;
    public PlayerGold playerGold;
    public PlayerInventory playerInventory;
    public ItemData oreItem;
    
    private OreCrate cheapCrate;
    
    void Awake()
    {
        if (playerGold == null)
            playerGold = FindFirstObjectByType<PlayerGold>();
        
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        
        cheapCrate = ScriptableObject.CreateInstance<OreCrate>();
        cheapCrate.crateType = "Cheap Ore Crate";
        cheapCrate.cratePrice = 50;
        cheapCrate.oreAmount = 10;
        crateTypeText.text = cheapCrate.crateType;
        cratePriceText.text = "Price: " + cheapCrate.cratePrice.ToString();
    }
    
    public void BuyCheapCrate()
    {
        if (cheapCrate == null || playerGold == null || playerInventory == null || oreItem == null)
            return;
            
        if (playerGold.CurrentGold >= cheapCrate.cratePrice)
        {
            playerGold.RemoveGold(cheapCrate.cratePrice);
            playerInventory.AddItem(oreItem, cheapCrate.oreAmount);
        }
    }
    
    public void TestMethod()
    {
        Debug.Log("Button connected!");
    }
}
