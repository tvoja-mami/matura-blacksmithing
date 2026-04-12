using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CatalogueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private CatalogueInput catalogueInput;
    [SerializeField] private string prompt = "[E] Open Catalogue";

    private void Awake()
    {
        if (catalogueInput == null)
        {
            catalogueInput = FindFirstObjectByType<CatalogueInput>();
        }
    }

    public void Interact()
    {
        if (catalogueInput == null) return;

        if (GameManager.Instance != null && GameManager.Instance.IsWaitingForNextDay)
        {
            Debug.Log("CatalogueInteractable: Shop is closed for the day.");
            return;
        }

        catalogueInput.ToggleCatalogue();
    }

    public string GetInteractionPrompt()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsWaitingForNextDay)
            return "[E] Catalogue (closed for the day)";

        return prompt;
    }
}
