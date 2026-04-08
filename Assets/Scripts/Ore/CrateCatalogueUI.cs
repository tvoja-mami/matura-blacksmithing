using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unified crate catalogue UI.
/// Left side: three buttons (Cheap / Normal / Expensive).
/// Right side: detail panel showing the selected crate's image, name,
/// contents list and a Buy button.
/// Assign OreCrate ScriptableObjects in the Inspector (cheap is index 0 / default).
/// </summary>
public class CrateCatalogueUI : MonoBehaviour
{
    [Header("Crate Data (order: Cheap, Normal, Expensive)")]
    [Tooltip("Assign exactly 3 OreCrate assets.")]
    public OreCrate[] crates = new OreCrate[3];

    [Header("Selection Buttons (same order as crates)")]
    public Button[] crateButtons = new Button[3];

    [Header("Detail Panel – Right Side")]
    public Image crateImage;
    public TextMeshProUGUI crateNameText;
    public TextMeshProUGUI includesText;
    public TextMeshProUGUI crateDescriptionText;
    public Button buyButton;

    [Header("Button Colours")]
    public Color selectedColour = new Color(0.85f, 0.65f, 0.25f, 1f);
    public Color normalColour  = new Color(0.40f, 0.40f, 0.40f, 1f);

    private int selectedIndex = -1;

    private void OnEnable()
    {
        PlayerGold.OnGoldChanged += OnGoldChanged;
        PlayerLevel.OnLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        PlayerGold.OnGoldChanged -= OnGoldChanged;
        PlayerLevel.OnLevelUp -= OnLevelUp;
    }

    private void Start()
    {
        // Wire up each selection button
        for (int i = 0; i < crateButtons.Length; i++)
        {
            int index = i; // capture for closure
            if (crateButtons[i] != null)
                crateButtons[i].onClick.AddListener(() => SelectCrate(index));
        }

        // Wire up the buy button
        if (buyButton != null)
            buyButton.onClick.AddListener(BuySelectedCrate);

        // Select cheap crate by default
        SelectCrate(0);

        // Initial affordability check
        RefreshAffordability();
    }

    // ───────── Gold Changed ─────────

    private void OnGoldChanged(int newGold)
    {
        RefreshAffordability();
    }

    private void OnLevelUp(int newLevel)
    {
        RefreshAffordability();
    }

    /// <summary>
    /// Disable buttons for crates the player can't afford.
    /// If the currently selected crate is no longer affordable,
    /// auto-switch to the best crate they can still buy (expensive → cheap).
    /// </summary>
    private void RefreshAffordability()
    {
        var playerGold = FindFirstObjectByType<PlayerGold>();
        var playerLevel = FindFirstObjectByType<PlayerLevel>();
        int gold  = playerGold  != null ? playerGold.CurrentGold   : 0;
        int level = playerLevel != null ? playerLevel.CurrentLevel : 1;

        // Enable / disable each button based on affordability + level
        for (int i = 0; i < crates.Length; i++)
        {
            if (i >= crateButtons.Length || crateButtons[i] == null || crates[i] == null)
                continue;

            bool canAfford = gold >= crates[i].cratePrice && level >= crates[i].requiredLevel;
            crateButtons[i].interactable = canAfford;
        }

        // Also disable the buy button if we can't afford the selected crate
        bool canBuySelected = selectedIndex >= 0
            && selectedIndex < crates.Length
            && crates[selectedIndex] != null
            && gold >= crates[selectedIndex].cratePrice
            && level >= crates[selectedIndex].requiredLevel;

        if (buyButton != null)
            buyButton.interactable = canBuySelected;

        // If the selected crate is now too expensive, auto-switch
        if (!canBuySelected)
        {
            // Try from expensive → cheap to pick the best affordable crate
            for (int i = crates.Length - 1; i >= 0; i--)
            {
                if (crates[i] != null && gold >= crates[i].cratePrice && level >= crates[i].requiredLevel)
                {
                    SelectCrate(i);
                    return;
                }
            }
            // Can't afford anything — stay on cheapest so panel isn't empty
            if (selectedIndex != 0)
                SelectCrate(0);
        }

        // Refresh button colours (selected + disabled tints)
        UpdateButtonColours(selectedIndex);
    }

    // ───────── Selection ─────────

    public void SelectCrate(int index)
    {
        if (crates == null || index < 0 || index >= crates.Length || crates[index] == null)
        {
            Debug.LogWarning($"CrateCatalogueUI: Invalid crate index {index}.");
            return;
        }

        selectedIndex = index;
        OreCrate crate = crates[index];

        // Update detail panel
        if (crateImage != null)
        {
            crateImage.sprite = crate.crateIcon;
            crateImage.enabled = crate.crateIcon != null;
        }

        if (crateNameText != null)
            crateNameText.text = crate.crateType;

        if (includesText != null)
        {
            var sb = new System.Text.StringBuilder();
            if (crate.drops != null)
            {
                for (int j = 0; j < crate.drops.Length; j++)
                {
                    var drop = crate.drops[j];
                    if (drop.item != null)
                    {
                        sb.Append($"{drop.item.itemName}  x{drop.amount}");
                        if (j < crate.drops.Length - 1)
                            sb.AppendLine();
                    }
                }
            }
            includesText.text = sb.ToString();
        }

        if (crateDescriptionText != null)
            crateDescriptionText.text = crate.description;

        // Highlight the selected button
        UpdateButtonColours(index);
    }

    private void UpdateButtonColours(int activeIndex)
    {
        for (int i = 0; i < crateButtons.Length; i++)
        {
            if (crateButtons[i] == null) continue;

            Color tint = i == activeIndex ? selectedColour : normalColour;

            // Ensure the Image base colour is white so the ColorBlock tint
            // is the only thing controlling the final colour.
            var img = crateButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = Color.white;

            var colours = crateButtons[i].colors;
            colours.normalColor      = tint;
            colours.highlightedColor = tint;
            colours.selectedColor    = tint;
            colours.colorMultiplier  = 1f;
            crateButtons[i].colors   = colours;
        }
    }

    // ───────── Purchase ─────────

    public void BuySelectedCrate()
    {
        if (selectedIndex < 0 || selectedIndex >= crates.Length)
            return;

        OreCrate crate = crates[selectedIndex];

        if (crate.drops == null || crate.drops.Length == 0)
        {
            Debug.LogError("CrateCatalogueUI: Crate has no drops configured.", this);
            return;
        }

        var playerGold = FindFirstObjectByType<PlayerGold>();
        var playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerGold == null || playerInventory == null)
        {
            Debug.LogError("CrateCatalogueUI: Missing PlayerGold or PlayerInventory in scene.", this);
            return;
        }

        if (playerGold.CurrentGold < crate.cratePrice)
        {
            Debug.Log($"CrateCatalogueUI: Not enough gold ({playerGold.CurrentGold}/{crate.cratePrice}).");
            return;
        }

        playerGold.RemoveGold(crate.cratePrice);

        foreach (var drop in crate.drops)
        {
            if (drop.item != null && drop.amount > 0)
                playerInventory.AddItem(drop.item, drop.amount);
        }

        Debug.Log($"CrateCatalogueUI: Purchased {crate.crateType} for {crate.cratePrice}g.");

        // Gold changed — refresh which crates are affordable
        RefreshAffordability();
    }
}
