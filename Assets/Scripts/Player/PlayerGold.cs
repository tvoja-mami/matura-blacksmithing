using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int currentGold = 100;

    public static event System.Action<int> OnGoldChanged;
    public int CurrentGold => currentGold;

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"PlayerGold: Tried to add invalid amount: {amount}");
            return;
        }
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
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
        Debug.Log($"PlayerGold: Removed {amount} gold. Total: {currentGold}");
    }
}
