using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the five coloured hit-zones on the vertical craft bar.
/// Layout (bottom to top): Meh | Ok | Great | Ok | Meh
/// The whole zone group is randomly shifted up/down within the bar after every hit.
///
/// Setup in the Inspector:
///   - Assign the five zone RectTransforms (children of the bar).
///   - Each zone should have an Image component whose colour you set in the Inspector
///     (yellow for Meh, green for Ok, blue/cyan for Great).
///   - The bar itself should have a dark background Image.
/// </summary>
public class CraftBar : MonoBehaviour
{
    [Header("Zone RectTransforms (assign in Inspector)")]
    public RectTransform mehZoneBottom;
    public RectTransform okZoneBottom;
    public RectTransform greatZone;
    public RectTransform okZoneTop;
    public RectTransform mehZoneTop;

    [Header("Zone proportions (fraction of bar height, must sum <= 1)")]
    [Range(0.05f, 0.35f)] public float greatPct = 0.10f;  // 10% — blue
    [Range(0.05f, 0.35f)] public float okPct    = 0.18f;  // 18% each — green
    [Range(0.05f, 0.35f)] public float mehPct   = 0.16f;  // 16% each — yellow

    private float barHeight;
    private float zoneStartNorm; // normalized bottom edge of the zone group [0,1]

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>Call once when the panel opens. height = barRect.rect.height</summary>
    public void Initialize(float height)
    {
        barHeight = height;
        RandomizeZones();
    }

    /// <summary>Shift the zone group to a new random position within the bar.</summary>
    public void RandomizeZones()
    {
        float totalZone = greatPct + okPct * 2f + mehPct * 2f;
        float slack = Mathf.Max(0f, 1f - totalZone);
        zoneStartNorm = Random.Range(0f, slack);
        LayoutZones();
    }

    /// <summary>Check which zone the arrow's normalised position (0=bottom, 1=top) falls in.</summary>
    public HitQuality CheckHit(float normalizedArrowPos)
    {
        float local = normalizedArrowPos - zoneStartNorm;
        float totalZone = greatPct + okPct * 2f + mehPct * 2f;

        if (local < 0f || local > totalZone) return HitQuality.Miss;
        if (local < mehPct)                              return HitQuality.Meh;
        if (local < mehPct + okPct)                      return HitQuality.Ok;
        if (local < mehPct + okPct + greatPct)           return HitQuality.Great;
        if (local < mehPct + okPct + greatPct + okPct)   return HitQuality.Ok;
        return HitQuality.Meh;
    }

    // ── Internal layout ────────────────────────────────────────────────────────

    private void LayoutZones()
    {
        if (barHeight == 0f) return;

        float cursor = zoneStartNorm;
        cursor = PlaceZone(mehZoneBottom, cursor, mehPct);
        cursor = PlaceZone(okZoneBottom,  cursor, okPct);
        cursor = PlaceZone(greatZone,     cursor, greatPct);
        cursor = PlaceZone(okZoneTop,     cursor, okPct);
               PlaceZone(mehZoneTop,    cursor, mehPct);
    }

    /// Positions a zone RectTransform. Returns next cursor (normalised).
    private float PlaceZone(RectTransform zone, float normStart, float normPct)
    {
        if (zone == null) return normStart + normPct;

        float height  = normPct * barHeight;
        // anchoredPosition.y is relative to bar centre (pivot = 0.5)
        float centerY = (normStart + normPct * 0.5f) * barHeight - barHeight * 0.5f;

        zone.sizeDelta        = new Vector2(zone.sizeDelta.x, height);
        zone.anchoredPosition = new Vector2(zone.anchoredPosition.x, centerY);

        return normStart + normPct;
    }
}
