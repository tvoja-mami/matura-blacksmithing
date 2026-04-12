using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to any UI Button to automatically play click and hover sounds
/// via SoundManager. No wiring needed — just add the component.
/// </summary>
public class UIButtonSFX : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonHover();
    }
}
