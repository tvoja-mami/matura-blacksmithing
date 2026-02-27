using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "[E] to Sleep";
    private static bool isTriggered;
    public static bool IsTriggered
    {
        get => isTriggered;
        set => isTriggered = value;
    }

    public static bool ConsumeTrigger()
    {
        if (!isTriggered)
        {
            return false;
        }

        isTriggered = false;
        return true;
    }

    public void Interact()
    {
        IsTriggered = true;
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}