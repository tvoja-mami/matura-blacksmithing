using UnityEngine;
using System.Collections;
using TMPro;
using image = UnityEngine.UI.Image;


public class CheapCrateCatalogueUI : MonoBehaviour
{

    public OreCrate oreCrate;
    public TextMeshProUGUI crateTypeText;
    public TextMeshProUGUI cratePriceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OreCrate cheapCrate = ScriptableObject.CreateInstance<OreCrate>();
        cheapCrate.crateType = "Cheap Ore Crate";
        cheapCrate.cratePrice = 50;
        cheapCrate.oreAmount = 10;
        crateTypeText.text = cheapCrate.crateType;
        cratePriceText.text = "Price: " + cheapCrate.cratePrice.ToString();
    }
}
