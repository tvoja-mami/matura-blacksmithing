using UnityEngine;
using System.Collections;
using TMPro;
using image = UnityEngine.UI.Image;


public class NormalCrateCatalogueUI : MonoBehaviour
{

    public OreCrate oreCrate;
    public TextMeshProUGUI crateTypeText;
    public TextMeshProUGUI cratePriceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OreCrate normalCrate = ScriptableObject.CreateInstance<OreCrate>();
        normalCrate.crateType = "Normal Ore Crate";
        normalCrate.cratePrice = 100;
        normalCrate.oreAmount = 25;
        crateTypeText.text = normalCrate.crateType;
        cratePriceText.text = "Price: " + normalCrate.cratePrice.ToString();
    }
}
