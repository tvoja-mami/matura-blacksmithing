using UnityEngine;

public class CostumerInteractable : MonoBehaviour, IInteractable
{    [SerializeField] private string prompt = "[E] to Interact with Customer";

    public void Interact()
    {
        Debug.Log("Interacting with the customer!");
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}
