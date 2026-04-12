using UnityEngine;

/// <summary>
/// Manages the daily rent the player must pay when going to sleep.
/// Rent starts at 1000g and increases by 100g each day.
/// Once the player buys the shop, Rent becomes false and no further rent is charged.
/// </summary>
public class RentManager : MonoBehaviour
{
    public static RentManager Instance { get; private set; }

    [Header("Rent Settings")]
    [SerializeField] private int baseRent = 1000;
    [SerializeField] private int rentIncreasePerDay = 100;

    /// <summary>True while the player still rents the shop. False after purchase.</summary>
    public bool Rent { get; set; } = true;

    /// <summary>Current rent amount based on the day number.</summary>
    public int CurrentRent(int dayNumber)
    {
        return baseRent + rentIncreasePerDay * Mathf.Max(0, dayNumber - 1);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called when the player goes to sleep. Returns true if rent was paid,
    /// false if the player cannot afford it (game over).
    /// </summary>
    public bool TryPayRent(PlayerGold playerGold, int dayNumber)
    {
        if (!Rent) return true;

        int amount = CurrentRent(dayNumber);

        if (playerGold.CurrentGold < amount)
        {
            Debug.Log($"RentManager: Cannot afford rent! Need {amount}g, have {playerGold.CurrentGold}g.");
            return false;
        }

        playerGold.RemoveGold(amount);
        Debug.Log($"RentManager: Paid {amount}g rent for day {dayNumber}. Remaining: {playerGold.CurrentGold}g.");
        return true;
    }
}
