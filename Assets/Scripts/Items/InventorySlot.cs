using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Image iconItem;
    public TextMeshProUGUI quantityText;
    public string nameItem;
    public string descriptionItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        icon.sprite = item.icon;
        iconItem.enabled = true;
        quantityText.text = quantity.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
