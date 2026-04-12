using UnityEngine;

/// <summary>
/// Interactable door that lets the player buy the shop for 250 000 gold,
/// finishing the game when purchased.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private int shopPrice = 250000;
    [SerializeField] private string prompt = "[E] Buy the shop for 250 000g";
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private Ending ending;

    private void Awake()
    {
        playerGold ??= FindFirstObjectByType<PlayerGold>();
        ending     ??= FindFirstObjectByType<Ending>(FindObjectsInactive.Include);
    }

    public void Interact()
    {
        if (playerGold == null || ending == null) return;

        if (playerGold.CurrentGold < shopPrice)
        {
            Debug.Log($"ShopDoor: Not enough gold ({playerGold.CurrentGold}/{shopPrice}).");
            return;
        }

        playerGold.RemoveGold(shopPrice);
        ending.ShowVictory();
    }

    public string GetInteractionPrompt()
    {
        if (playerGold != null && playerGold.CurrentGold >= shopPrice)
            return prompt;

        return $"[E] Buy the shop ({shopPrice}g) \n\nnot enough gold";
    }
}
