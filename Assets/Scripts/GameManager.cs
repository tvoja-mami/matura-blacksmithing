using System;
using UnityEngine;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public ItemData ironOreItem;
    public ItemData ironSwordItem;
    public InventoryUI inventoryUi;
    public PlayerGold playerGold;
     [Header("Time & Day Cycle")]
    [Tooltip("Length of a full in-game day in real-world seconds.")]
    public float dayLengthSeconds = 300f;
    [Tooltip("The current time in hours (0-24).")]
    [Range(0f, 24f)] public float currentTime = 8f;
    [Tooltip("Hour when the clock stops and waits for the player (e.g. 20 = 20:00).")]
    [Range(0f, 24f)] public float dayStopHour = 20f;
    [Tooltip("The current day number (1=Mon, 2=Tues, etc.).")]
    public int dayNumber = 1;
    public static event System.Action<float, int> OnTimeChanged;

    [Header("Time Pause")]
    [Tooltip("True when time has reached dayStopHour and is waiting for user action to advance.")]
    [SerializeField] private bool isWaitingForNextDay = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("GameManager: Could not find PlayerInventory in scene!");
                enabled = false;
                return;
            }
        }

        if (inventoryUi == null)
        {
            inventoryUi = FindFirstObjectByType<InventoryUI>();
            if (inventoryUi == null)
            {
                Debug.LogError("GameManager: Could not find InventoryUI in scene!");
                enabled = false;
                return;
            }
        }

        if (playerGold == null)
        {
            playerGold = FindFirstObjectByType<PlayerGold>();
            if (playerGold == null)
            {
                Debug.LogError("GameManager: Could not find PlayerGold in scene!");
                enabled = false;
                return;
            }
        }


        if (ironOreItem == null || ironSwordItem == null)
        {
            Debug.LogError("GameManager: ItemData references (ironOreItem or ironSwordItem) are missing!");
            enabled = false;
            return;
        }
        inventoryUi.UpdateInventoryUI(playerInventory);
    }

    // Update is called once per frame
 
    void Update()
    {
        UpdateTime();
    }


//čas

    private void UpdateTime()
    {
        if (dayLengthSeconds <= 0f) return;

        // Stop time progression once we reach the end-of-day threshold.
        if (isWaitingForNextDay)
        {
            return;
        }

        float hoursPerSecond = 24f / dayLengthSeconds;
        currentTime += hoursPerSecond * Time.deltaTime;

        if (currentTime >= dayStopHour)
        {
            currentTime = dayStopHour;
            isWaitingForNextDay = true;
        }

        OnTimeChanged?.Invoke(currentTime, dayNumber);
    }

    //nov dan
    public void AdvanceToNextDay()
    {
        if (!isWaitingForNextDay)
        {
            Debug.Log("GameManager: AdvanceToNextDay called, but the game is not waiting for next day yet.");
            return;
        }

        isWaitingForNextDay = false;
        EndDay();
    }

    public void EndDay()
    {
        Debug.Log("Nov dan");
        currentTime = 8f; 
        dayNumber++;
        if (dayNumber > 5) 
        {
            dayNumber = 1;
        }

        OnTimeChanged?.Invoke(currentTime, dayNumber);
    }
    public void GetOre_DEBUG()
    {
        playerInventory.AddItem(ironOreItem, 5);
        if (playerInventory.items.TryGetValue(ironOreItem, out int newCount))
        {
            Debug.Log($"The player now has {newCount} Iron ores");
        }
        else
        {
            Debug.Log("The player has no Iron ores");
        }
        inventoryUi.UpdateInventoryUI(playerInventory);
    }
    public void CraftSword_DEBUG()
    {
        if (playerInventory.items.TryGetValue(ironOreItem, out int currentCount))
        {
            int removeQuantity = 3;
            if (currentCount >= removeQuantity)
            {
                playerInventory.RemoveItem(ironOreItem, removeQuantity);
                playerInventory.AddItem(ironSwordItem, 1);
                int remainingOre = playerInventory.items.ContainsKey(ironOreItem) ? playerInventory.items[ironOreItem] : 0;
                int swordCount = playerInventory.items.ContainsKey(ironSwordItem) ? playerInventory.items[ironSwordItem] : 0;
                Debug.Log($"One iron sword crafted. Remaining: {remainingOre} iron. You have {swordCount} iron swords");
                inventoryUi.UpdateInventoryUI(playerInventory);
            }
            else
            {
                Debug.Log($"Not enough materials. You have {currentCount} materials");
            }
        }
    }
    public void SellSword_DEBUG()
    {
        if (playerInventory.items.TryGetValue(ironSwordItem, out int currentCount))
        {
            if (currentCount >= 1)
            {
                playerInventory.RemoveItem(ironSwordItem, 1);
                playerGold.AddGold(100); // Add 100 gold for selling a sword
                int remaining = playerInventory.items.ContainsKey(ironSwordItem) ? playerInventory.items[ironSwordItem] : 0;
                Debug.Log("Removed 1 iron sword. You have " + remaining + " left");
                inventoryUi.UpdateInventoryUI(playerInventory);
            }
        }
        else
        {
            Debug.Log("You don't have an iron sword");
        }

    }
    public void GetGold_DEBUG(int amount)
    {
        amount = 5;
        playerGold.AddGold(amount);
    }
}