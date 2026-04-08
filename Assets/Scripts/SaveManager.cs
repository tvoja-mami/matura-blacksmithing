using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameManager gameManager;

    [Header("Item Database")]
    [Tooltip("Drag ALL ItemData assets here (items + ores) so the save system can look them up by ID.")]
    [SerializeField] private List<ItemData> allItems;

    private const string SaveExistsKey = "HasSave";
    private const string GoldKey = "Save_Gold";
    private const string LevelKey = "Save_Level";
    private const string XPKey = "Save_XP";
    private const string DayKey = "Save_Day";
    private const string TimeKey = "Save_Time";
    private const string MaterialsKey = "Save_Materials";
    private const string CraftedKey = "Save_Crafted";

    private Dictionary<int, ItemData> itemLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (playerGold == null)
            playerGold = FindFirstObjectByType<PlayerGold>();
        if (playerLevel == null)
            playerLevel = FindFirstObjectByType<PlayerLevel>();
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        BuildItemLookup();

        if (MainMenuUI.ShouldLoadSave)
            LoadGame();
    }

    private void Start()
    {
        if (MainMenuUI.ShouldLoadSave)
            StartCoroutine(RefreshUINextFrame());
    }

    private IEnumerator RefreshUINextFrame()
    {
        yield return null; // wait one frame so all Start() methods have finished

        playerGold.SetGold(playerGold.CurrentGold);
        playerLevel.SetLevelAndXP(playerLevel.CurrentLevel, playerLevel.CurrentXP);

        InventoryUI inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (inventoryUI != null)
            inventoryUI.UpdateInventoryUI(playerInventory);
    }

    private void BuildItemLookup()
    {
        itemLookup = new Dictionary<int, ItemData>();
        foreach (ItemData item in allItems)
        {
            if (item == null) continue;
            if (itemLookup.ContainsKey(item.itemID))
            {
                Debug.LogWarning($"SaveManager: Duplicate itemID {item.itemID} on {item.itemName}");
                continue;
            }
            itemLookup[item.itemID] = item;
        }
    }

    // ── Save ────────────────────────────────────────────────────────────────

    public void SaveGame()
    {
        PlayerPrefs.SetInt(GoldKey, playerGold.CurrentGold);
        PlayerPrefs.SetInt(LevelKey, playerLevel.CurrentLevel);
        PlayerPrefs.SetInt(XPKey, playerLevel.CurrentXP);
        PlayerPrefs.SetInt(DayKey, gameManager.dayNumber);
        PlayerPrefs.SetFloat(TimeKey, gameManager.currentTime);

        // Materials: "id:qty,id:qty"
        var matParts = new List<string>();
        foreach (var kvp in playerInventory.items)
        {
            if (kvp.Key == null) continue;
            matParts.Add($"{kvp.Key.itemID}:{kvp.Value}");
        }
        PlayerPrefs.SetString(MaterialsKey, string.Join(",", matParts));

        // Crafted items: "id:rarity,id:rarity"
        var craftParts = new List<string>();
        foreach (CraftedItem ci in playerInventory.craftedItems)
        {
            if (ci == null || ci.item == null) continue;
            craftParts.Add($"{ci.item.itemID}:{(int)ci.rarity}");
        }
        PlayerPrefs.SetString(CraftedKey, string.Join(",", craftParts));

        PlayerPrefs.SetInt(SaveExistsKey, 1);
        PlayerPrefs.Save();
        Debug.Log("SaveManager: Game saved.");
    }

    // ── Load ────────────────────────────────────────────────────────────────

    public void LoadGame()
    {
        if (PlayerPrefs.GetInt(SaveExistsKey, 0) == 0)
        {
            Debug.Log("SaveManager: No save found.");
            return;
        }

        playerGold.SetGold(PlayerPrefs.GetInt(GoldKey, 250));
        playerLevel.SetLevelAndXP(
            PlayerPrefs.GetInt(LevelKey, 1),
            PlayerPrefs.GetInt(XPKey, 0)
        );
        gameManager.dayNumber = PlayerPrefs.GetInt(DayKey, 1);
        gameManager.currentTime = PlayerPrefs.GetFloat(TimeKey, 8f);

        // Materials
        playerInventory.items.Clear();
        string matSave = PlayerPrefs.GetString(MaterialsKey, "");
        if (!string.IsNullOrEmpty(matSave))
        {
            foreach (string entry in matSave.Split(','))
            {
                string[] parts = entry.Split(':');
                if (parts.Length != 2) continue;
                if (!int.TryParse(parts[0], out int id)) continue;
                if (!int.TryParse(parts[1], out int qty)) continue;
                if (itemLookup.TryGetValue(id, out ItemData item))
                    playerInventory.AddItem(item, qty);
            }
        }

        // Crafted items
        playerInventory.craftedItems.Clear();
        string craftSave = PlayerPrefs.GetString(CraftedKey, "");
        if (!string.IsNullOrEmpty(craftSave))
        {
            foreach (string entry in craftSave.Split(','))
            {
                string[] parts = entry.Split(':');
                if (parts.Length != 2) continue;
                if (!int.TryParse(parts[0], out int id)) continue;
                if (!int.TryParse(parts[1], out int rarityInt)) continue;
                if (itemLookup.TryGetValue(id, out ItemData item))
                    playerInventory.AddItemWithRarity(item, 1, (ItemRarity)rarityInt);
            }
        }

        Debug.Log("SaveManager: Game loaded.");
    }

    // ── Auto-save on quit ───────────────────────────────────────────────────

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public static bool HasSave()
    {
        return PlayerPrefs.GetInt(SaveExistsKey, 0) == 1;
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("SaveManager: All save data deleted.");
    }
}
