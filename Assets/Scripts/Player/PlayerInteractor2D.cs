using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInteractor2D : MonoBehaviour
{
    private readonly List<IInteractable> inRange = new();
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }

        controls.Gameplay.Enable();
        controls.Gameplay.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (controls == null)
        {
            return;
        }

        controls.Gameplay.Interact.performed -= OnInteract;
        controls.Gameplay.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        IInteractable target = GetCurrentInteractable();
        if (target != null)
        {
            target.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        var interactable = GetInteractable(other);

        if (interactable != null && !inRange.Contains(interactable))
        {
            inRange.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;

        var interactable = GetInteractable(other);

        if (interactable != null)
        {
            inRange.Remove(interactable);
        }
    }

    private static IInteractable GetInteractable(Collider2D collider)
    {
        var interactable = collider.GetComponentInParent<IInteractable>();
        if (interactable == null)
        {
            interactable = collider.GetComponent<IInteractable>();
        }

        return interactable;
    }

    private IInteractable GetCurrentInteractable()
    {
        for (int i = inRange.Count - 1; i >= 0; i--)
        {
            if (inRange[i] != null)
            {
                return inRange[i];
            }
        }

        return null;
    }
}