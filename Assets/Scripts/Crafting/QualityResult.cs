using UnityEngine;

[System.Serializable]
public class QualityResult
{
    [Header("Hit Statistics")]
    public int totalHits;
    public int greatHits;
    public int okHits;
    public int mehHits;
    public int missedHits;

    [Header("Quality Score")]
    [Range(0f, 100f)]
    public float qualityPercentage;

    public QualityResult()
    {
        totalHits        = 0;
        greatHits        = 0;
        okHits           = 0;
        mehHits          = 0;
        missedHits       = 0;
        qualityPercentage = 0f;
    }

    public void AddHit(HitQuality quality)
    {
        totalHits++;

        switch (quality)
        {
            case HitQuality.Great: greatHits++;  break;
            case HitQuality.Ok:    okHits++;     break;
            case HitQuality.Meh:   mehHits++;    break;
            case HitQuality.Miss:  missedHits++; break;
        }
        // qualityPercentage is managed live by CraftingMinigame (decay + gains)
    }

    public string GetQualityGrade()
    {
        if (qualityPercentage >= 95f) return "S";
        if (qualityPercentage >= 85f) return "A";
        if (qualityPercentage >= 75f) return "B";
        if (qualityPercentage >= 65f) return "C";
        if (qualityPercentage >= 50f) return "D";
        return "F";
    }

    public override string ToString() =>
        $"Quality: {qualityPercentage:F1}% [{GetQualityGrade()}] | " +
        $"Great: {greatHits}  Ok: {okHits}  Meh: {mehHits}  Miss: {missedHits}";
}

public enum HitQuality { Miss, Meh, Ok, Great }
