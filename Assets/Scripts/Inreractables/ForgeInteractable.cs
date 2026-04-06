using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ForgeInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private ForgeUI forgeUI;
    [SerializeField] private string prompt = "[E] Use Forge";

    private void Awake()
    {
        if (forgeUI == null)
        {
            forgeUI = FindFirstObjectByType<ForgeUI>(FindObjectsInactive.Include);
        }
    }

    public void Interact()
    {
        if (forgeUI == null) return;

        // Ensure the GameObject is active so ForgeUI can run.
        forgeUI.gameObject.SetActive(true);
        forgeUI.ToggleForge();
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}