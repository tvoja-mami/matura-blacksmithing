using UnityEngine;
using TMPro;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int currentGold = 100;
    
    [SerializeField]
    private TextMeshProUGUI goldText;

    public static event System.Action<int> OnGoldChanged;
    public int CurrentGold => currentGold;
    
    private void Start()
    {
        UpdateGoldDisplay();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"PlayerGold: Tried to add invalid amount: {amount}");
            return;
        }
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
        UpdateGoldDisplay();
        Debug.Log($"PlayerGold: Added {amount} gold. Total: {currentGold}");
    } 

    public void RemoveGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"PlayerGold: Tried to remove invalid amount: {amount}");
            return;
        }

        currentGold -= amount;
        OnGoldChanged?.Invoke(currentGold);
        UpdateGoldDisplay();
        Debug.Log($"PlayerGold: Removed {amount} gold. Total: {currentGold}");
    }
    
    private void UpdateGoldDisplay()
    {
        if (goldText != null)
        {
            goldText.text = "Player gold: " + currentGold;
        }
    }
}
