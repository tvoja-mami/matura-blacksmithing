using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    // This is called by the PlayerInteractor when the player presses 'E'
    public void Interact()
    {
        Debug.Log("Player interacted with the bed.");
        // The bed doesn't know how to end the day. It just tells the GameManager to do it.
        FindObjectOfType<GameManager>().EndDay();
    }

    public string GetInteractionPrompt()
    {
        return "[E] to Sleep";
    }
}