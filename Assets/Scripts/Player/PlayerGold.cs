using UnityEngine;
using TMPro;

public class PlayerGold : MonoBehaviour
{
    [SerializeField, Min(0)]
    private int currentGold = 250;
    [SerializeField, Min(1)]
    private int debugGoldIncrement = 100;
    
    [SerializeField]
    private TextMeshProUGUI goldText;
    private TextMeshProUGUI goldTextCatalogue;

    public static event System.Action<int> OnGoldChanged;
    public int CurrentGold => currentGold;
    
    private void Start()
    {
        UpdateGoldDisplay();
    }

    public void DebugAddGold()
    {
        AddGold(debugGoldIncrement);
    }

    public void DebugRemoveGold()
    {
        RemoveGold(Mathf.Min(debugGoldIncrement, currentGold));
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
        if (goldTextCatalogue != null)
        {
            goldTextCatalogue.text = "Catalogue gold: " + currentGold;
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("Add Debug Gold")]
    private void ContextAddGold()
    {
        DebugAddGold();
    }

    [ContextMenu("Remove Debug Gold")]
    private void ContextRemoveGold()
    {
        DebugRemoveGold();
    }
#endif
}
