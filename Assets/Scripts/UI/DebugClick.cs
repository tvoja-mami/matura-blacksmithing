using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Temporary debug script. Attach to PlayButton to test if pointer events reach it.
/// Delete this after debugging.
/// </summary>
public class DebugClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerDownHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("DebugClick: POINTER CLICK on " + gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("DebugClick: POINTER ENTER on " + gameObject.name);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("DebugClick: POINTER DOWN on " + gameObject.name);
    }
}
