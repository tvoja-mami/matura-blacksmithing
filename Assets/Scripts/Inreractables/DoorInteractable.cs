using UnityEngine;

/// <summary>
/// Interactable door that lets the player buy the shop for 250 000 gold,
/// triggering the victory / ending sequence.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private int shopCost = 250000;
    [SerializeField] private Ending ending;

    private void Awake()
    {
        if (ending == null)
            ending = FindFirstObjectByType<Ending>(FindObjectsInactive.Include);
    }

    private bool ShopOwned =>
        RentManager.Instance != null && !RentManager.Instance.Rent;

    public void Interact()
    {
        if (ShopOwned) return;

        var playerGold = FindFirstObjectByType<PlayerGold>();
        if (playerGold == null) return;

        if (playerGold.CurrentGold < shopCost)
        {
            Debug.Log($"DoorInteractable: Not enough gold. Need {shopCost}g, have {playerGold.CurrentGold}g.");
            return;
        }

        playerGold.RemoveGold(shopCost);
        Debug.Log($"DoorInteractable: Shop purchased for {shopCost}g!");

        if (ending != null)
            ending.ShowVictory();
    }

    public string GetInteractionPrompt()
    {
        if (ShopOwned)
            return "Shop Owned";

        var playerGold = FindFirstObjectByType<PlayerGold>();
        if (playerGold != null && playerGold.CurrentGold >= shopCost)
            return $"[E] Buy the shop ({shopCost}g)";

        return $"<color=red>[E] Buy the shop ({shopCost}g)</color>";
    }
}
