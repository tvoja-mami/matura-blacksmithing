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

        catalogueInput.ToggleCatalogue();
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}
