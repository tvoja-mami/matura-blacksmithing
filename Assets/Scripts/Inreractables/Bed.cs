using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "[E] to Sleep";

    public void Interact()
    {
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.EndDay();
        }
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}