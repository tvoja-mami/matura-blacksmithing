using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInteractor2D : MonoBehaviour
{
    [Header("Prompt UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private readonly Dictionary<Collider2D, IInteractable> inRange = new();
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (controls == null) return;

        controls.Gameplay.Interact.performed -= OnInteract;
        controls.Gameplay.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        IInteractable target = GetCurrentInteractable();
        if (target != null)
        {
            HidePrompt();
            target.Interact();
        }
    }

    private void Update()
    {
        // Hide prompt while a menu is open
        if (PlayerMovement.ActiveMenuCount > 0)
        {
            HidePrompt();
            return;
        }

        IInteractable target = GetCurrentInteractable();
        if (target != null)
            ShowPrompt(target.GetInteractionPrompt());
        else
            HidePrompt();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        var interactable = GetInteractable(other);
        if (interactable != null)
            inRange[other] = interactable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        inRange.Remove(other);
    }

    private static IInteractable GetInteractable(Collider2D collider)
    {
        var interactable = collider.GetComponentInParent<IInteractable>();
        if (interactable == null)
            interactable = collider.GetComponent<IInteractable>();
        return interactable;
    }

    private IInteractable GetCurrentInteractable()
    {
        foreach (var kvp in inRange)
        {
            if (kvp.Key != null && kvp.Value is MonoBehaviour mb && mb != null)
                return kvp.Value;
        }
        return null;
    }

    // ────────── Prompt UI ──────────

    private void ShowPrompt(string text)
    {
        if (promptPanel != null) promptPanel.SetActive(true);
        if (promptText  != null) promptText.text = text;
    }

    private void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        if (promptText  != null) promptText.text = "";
    }
}
