using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "[E] to Sleep";

    private static bool isTriggered;
    private bool awaitingConfirmation;

    public static bool IsTriggered
    {
        get => isTriggered;
        set => isTriggered = value;
    }

    public static bool ConsumeTrigger()
    {
        if (!isTriggered)
        {
            return false;
        }

        isTriggered = false;
        return true;
    }

    private bool CanAffordRent()
    {
        if (RentManager.Instance == null || !RentManager.Instance.Rent) return true;
        if (GameManager.Instance == null) return true;

        var playerGold = FindFirstObjectByType<PlayerGold>();
        if (playerGold == null) return true;

        int rent = RentManager.Instance.CurrentRent(GameManager.Instance.dayNumber);
        return playerGold.CurrentGold >= rent;
    }

    public void Interact()
    {
        // Block sleeping before 20:00
        if (GameManager.Instance != null && !GameManager.Instance.IsWaitingForNextDay)
            return;

        if (!CanAffordRent())
        {
            if (!awaitingConfirmation)
            {
                // First press — ask for confirmation
                awaitingConfirmation = true;
                return;
            }
            // Second press — confirmed, trigger game over
        }

        awaitingConfirmation = false;
        IsTriggered = true;
    }

    /// <summary>Reset confirmation if the player walks away.</summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        awaitingConfirmation = false;
    }

    public string GetInteractionPrompt()
    {
        // Before 20:00 — no prompt
        if (GameManager.Instance != null && !GameManager.Instance.IsWaitingForNextDay)
            return null;

        if (RentManager.Instance != null && RentManager.Instance.Rent && GameManager.Instance != null)
        {
            var rent = RentManager.Instance.CurrentRent(GameManager.Instance.dayNumber);

            if (!CanAffordRent())
            {
                if (awaitingConfirmation)
                    return $"<color=red>[E] Are you sure? Your save will be deleted!</color>";

                return $"<color=red>[E] You cannot afford rent ({rent}g)</color>";
            }

            return $"[E] Pay the rent ({rent}g)";
        }

        return prompt;
    }
}