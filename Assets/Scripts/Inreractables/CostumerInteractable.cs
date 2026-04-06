using UnityEngine;

public class CostumerInteractable : MonoBehaviour, IInteractable
{    [SerializeField] private string prompt = "[E] to Interact with Customer";
    [SerializeField] private SellUI sellUI;

    private void Awake()
    {
        sellUI ??= FindFirstObjectByType<SellUI>(FindObjectsInactive.Include);
    }

    public void Interact()
    {
        if (sellUI == null) return;
        sellUI.gameObject.SetActive(true);
        sellUI.ToggleShop();
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}
