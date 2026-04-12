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

        if (GameManager.Instance != null && GameManager.Instance.IsWaitingForNextDay)
        {
            Debug.Log("ForgeInteractable: Shop is closed for the day.");
            return;
        }

        // Ensure the GameObject is active so ForgeUI can run.
        forgeUI.gameObject.SetActive(true);
        forgeUI.ToggleForge();
    }

    public string GetInteractionPrompt()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsWaitingForNextDay)
            return "[E] Forge (closed for the day)";

        return prompt;
    }
}