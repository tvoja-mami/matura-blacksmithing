using UnityEngine;
using System.Collections;
using TMPro;
using image = UnityEngine.UI.Image;


public class CatalogueUI : MonoBehaviour
{

    public OreCrate oreCrate;
    public TextMeshProUGUI crateTypeText;
    public TextMeshProUGUI cratePriceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OreCrate expensiveCrate = ScriptableObject.CreateInstance<OreCrate>();
        expensiveCrate.crateType = "Expensive Ore Crate";
        expensiveCrate.cratePrice = 200;
        expensiveCrate.oreAmount = 50;
        crateTypeText.text = expensiveCrate.crateType;
        cratePriceText.text = "Price: " + expensiveCrate.cratePrice.ToString();
    }
}
