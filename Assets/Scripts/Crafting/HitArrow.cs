using UnityEngine;

/// <summary>
/// Bounces a UI element up and down within a vertical bar.
/// Attach to the arrow indicator RectTransform (child of the bar).
/// Call Initialize() once the bar height is known (after layout).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HitArrow : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Pixels per second the arrow travels")]
    public float bounceSpeed = 280f;

    private RectTransform rt;
    private float minY, maxY;
    private float direction = 1f;
    private bool active;

    /// <summary>Set up bounds and start moving. barHalfHeight is half the bar's rect height.</summary>
    public void Initialize(float barHalfHeight)
    {
        rt = GetComponent<RectTransform>();
        minY = -barHalfHeight;
        maxY =  barHalfHeight;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
        direction = 1f;
        active = true;
    }

    public void SetActive(bool value) => active = value;

    private void Update()
    {
        if (!active) return;

        float newY = rt.anchoredPosition.y + direction * bounceSpeed * Time.deltaTime;

        if (newY >= maxY)      { newY = maxY; direction = -1f; }
        else if (newY <= minY) { newY = minY; direction =  1f; }

        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, newY);
    }

    /// <summary>Returns 0 (bottom) to 1 (top) within the bar.</summary>
    public float GetNormalizedPosition()
    {
        float range = maxY - minY;
        return range == 0f ? 0.5f : (rt.anchoredPosition.y - minY) / range;
    }
}
