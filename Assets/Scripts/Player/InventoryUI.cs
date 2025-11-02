using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInventoryUI(PlayerInventory inventory)
    {
        Transform GridLeft = GameObject.Find("GridLeft").transform;
        foreach (Transform child in GridLeft) {
            DestroyImmediate(child.gameObject);
        }
    }
}
