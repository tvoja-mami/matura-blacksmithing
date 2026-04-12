using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Popup mini-game controller for the crafting desk.
///
/// Scene setup (all children of a CraftingMinigame canvas group):
///   GamePanel        — shown during play
///     BarRect        — tall, narrow RectTransform (the track). Pivot = centre.
///       mehZoneBottom / okZoneBottom / greatZone / okZoneTop / mehZoneTop
///                    — coloured Image RectTransforms, children of BarRect
///       ArrowRect    — the indicator (HitArrow component)
///     TimerText      — TMP countdown
///     QualityText    — live quality %
///     HitsText       — G / OK / M / X counts
///     FeedbackText   — GREAT / OK / MEH / MISS pop
///     HitButton      — optional on-screen button that calls RegisterHit()
///   ResultPanel      — shown after game ends
///     ResultItemName
///     ResultGradeText
///     ResultRarityText
///     ContinueButton — calls CloseResult()
/// </summary>
public class CraftingMinigame : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Bar & Arrow")]
    [SerializeField] private RectTransform barRect;
    [SerializeField] private HitArrow hitArrow;
    [SerializeField] private CraftBar craftBar;

    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private TextMeshProUGUI hitsText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Result UI")]
    [SerializeField] private TextMeshProUGUI resultItemNameText;
    [SerializeField] private TextMeshProUGUI resultGradeText;
    [SerializeField] private TextMeshProUGUI resultRarityText;
    [SerializeField] private TextMeshProUGUI resultXPText;

    [Header("Settings")]
    [SerializeField] private float gameDuration = 30f;

    [Header("Quality Decay & Gains")]
    [Tooltip("Quality lost per second when not hitting")]
    [SerializeField] private float decayPerSecond = 5f;
    [Tooltip("Quality gained on Great hit")]
    [SerializeField] private float greatGain = 15f;
    [Tooltip("Quality gained on Ok hit")]
    [SerializeField] private float okGain = 7f;
    [Tooltip("Quality gained on Meh hit")]
    [SerializeField] private float mehGain = 2f;
    [Tooltip("Quality lost on Miss")]
    [SerializeField] private float missPenalty = 10f;

    [Header("Dependencies")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerLevel playerLevel;

    // ── State ──────────────────────────────────────────────────────────────────
    private RecipeData    currentRecipe;
    private QualityResult qualityResult;
    private float         timeRemaining;
    private float         currentQuality;
    private bool          gameActive;
    private Coroutine     feedbackCoroutine;

    // ── Unity ──────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerLevel == null)
            playerLevel = FindFirstObjectByType<PlayerLevel>();

        gamePanel.SetActive(false);
        resultPanel.SetActive(false);
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f) { timeRemaining = 0f; EndGame(); return; }

        // Passive quality decay
        currentQuality -= decayPerSecond * Time.deltaTime;
        currentQuality = Mathf.Clamp(currentQuality, 0f, 100f);
        qualityResult.qualityPercentage = currentQuality;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) RegisterHit();

        UpdateGameUI();
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by ForgeUI when the player clicks the CraftButton.
    /// Returns false if materials are missing or no recipe is selected.
    /// </summary>
    public bool StartCraft(RecipeData recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("CraftingMinigame: No recipe selected.");
            return false;
        }

        if (!HasMaterials(recipe))
        {
            Debug.LogWarning("CraftingMinigame: Not enough materials.");
            return false;
        }

        currentRecipe  = recipe;
        qualityResult  = new QualityResult();
        timeRemaining  = gameDuration;
        currentQuality = 100f;
        qualityResult.qualityPercentage = 100f;
        gameActive     = true;

        // Initialize bar height AFTER the panel is active so RectTransform is laid out.
        gamePanel.SetActive(true);
        resultPanel.SetActive(false);
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);

        // Delay one frame so Unity finishes layout before we read rect.height
        StartCoroutine(InitBarNextFrame());

        PlayerMovement.ActiveMenuCount++;
        UpdateGameUI();
        return true;
    }

    /// <summary>Register a hit at the current arrow position. Also callable from a UI Button.</summary>
    public void RegisterHit()
    {
        if (!gameActive) return;

        float normalized = hitArrow.GetNormalizedPosition();
        HitQuality quality = craftBar.CheckHit(normalized);

        float gain = quality switch
        {
            HitQuality.Great => greatGain,
            HitQuality.Ok    => okGain,
            HitQuality.Meh   => mehGain,
            HitQuality.Miss  => -missPenalty,
            _                => 0f
        };

        currentQuality = Mathf.Clamp(currentQuality + gain, 0f, 100f);
        qualityResult.qualityPercentage = currentQuality;

        qualityResult.AddHit(quality);
        craftBar.RandomizeZones();
        ShowFeedback(quality);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHammerHit();
            SoundManager.Instance.PlayHitFeedback(quality);
        }
        UpdateGameUI();
    }

    /// <summary>Called by the Continue button on the result panel.</summary>
    public void CloseResult()
    {
        resultPanel.SetActive(false);
        hitArrow.SetActive(false);
        PlayerMovement.ActiveMenuCount--;
        currentRecipe = null;
    }

    // ── Private helpers ────────────────────────────────────────────────────────

    private IEnumerator InitBarNextFrame()
    {
        yield return null; // wait one frame for layout to resolve

        float barHeight = barRect.rect.height;
        if (barHeight <= 0f) barHeight = 400f; // fallback if layout not ready

        hitArrow.Initialize(barHeight * 0.5f);
        craftBar.Initialize(barHeight);
    }

    private void EndGame()
    {
        gameActive = false;
        hitArrow.SetActive(false);

        int luck = playerLevel != null ? playerLevel.Luck : 0;
        ItemRarity rarity = RarityHelper.RollRarity(qualityResult.qualityPercentage, luck);

        ConsumeMaterials(currentRecipe);
        playerInventory.AddItemWithRarity(currentRecipe.outputItem, 1, rarity);

        int xpGained = Mathf.RoundToInt(currentRecipe.craftXP * RarityHelper.GetXPMultiplier(rarity));
        playerLevel?.AddXP(xpGained);

        Debug.Log($"Craft complete — {currentRecipe.outputItem.itemName} | {qualityResult} | Rarity: {RarityHelper.GetName(rarity)} | XP gained: {xpGained}");

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayCraftComplete();

        ShowResultPanel(rarity, xpGained);
    }

    private void ShowResultPanel(ItemRarity rarity, int xpGained)
    {
        gamePanel.SetActive(false);
        resultPanel.SetActive(true);

        if (resultItemNameText != null)
            resultItemNameText.text = currentRecipe.outputItem.itemName;

        if (resultGradeText != null)
            resultGradeText.text = $"Grade: {qualityResult.GetQualityGrade()}  ({qualityResult.qualityPercentage:F0}%)";

        if (resultRarityText != null)
        {
            resultRarityText.text  = RarityHelper.GetName(rarity);
            resultRarityText.color = RarityHelper.GetColor(rarity);
        }

        if (resultXPText != null)
            resultXPText.text = $"+{xpGained} XP";
    }

    private void ShowFeedback(HitQuality quality)
    {
        if (feedbackText == null) return;
        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(FeedbackRoutine(quality));
    }

    private IEnumerator FeedbackRoutine(HitQuality quality)
    {
        feedbackText.gameObject.SetActive(true);

        (feedbackText.text, feedbackText.color) = quality switch
        {
            HitQuality.Great => ("GREAT!", Color.cyan),
            HitQuality.Ok    => ("OK",     Color.green),
            HitQuality.Meh   => ("MEH",    Color.yellow),
            _                => ("MISS",   Color.red)
        };

        yield return new WaitForSeconds(0.6f);
        feedbackText.gameObject.SetActive(false);
    }

    private void UpdateGameUI()
    {
        if (timerText   != null) timerText.text   = $"{timeRemaining:F1}s";
        if (qualityText != null) qualityText.text = $"Quality: {qualityResult.qualityPercentage:F0}%";
        if (hitsText    != null)
            hitsText.text = $"G:{qualityResult.greatHits}  OK:{qualityResult.okHits}  M:{qualityResult.mehHits}  X:{qualityResult.missedHits}";
    }

    private bool HasMaterials(RecipeData recipe)
    {
        foreach (var req in recipe.requiredMaterials)
        {
            if (!playerInventory.items.TryGetValue(req.item, out int count) || count < req.amount)
                return false;
        }
        return true;
    }

    private void ConsumeMaterials(RecipeData recipe)
    {
        foreach (var req in recipe.requiredMaterials)
            playerInventory.RemoveItem(req.item, req.amount);
    }
}
