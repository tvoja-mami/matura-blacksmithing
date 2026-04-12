using UnityEngine;

/// <summary>
/// Central audio manager. Place one instance in the gameplay scene.
/// Assign AudioClips in the Inspector — any clip left empty is simply skipped.
///
/// Scene setup:
///   Create a GameObject "SoundManager" with three child AudioSources
///   (or add them on the same object) and drag them into the slots below.
///   - sfxSource   — for one-shot gameplay sounds (hammer, coins, level-up)
///   - ambientSource — for the looping forge ambience
///   - uiSource    — for UI sounds (open/close, button click/hover)
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource footstepSource;

    [Header("Minigame")]
    [SerializeField] private AudioClip hammerHit;
    [SerializeField] private AudioClip hitGreat;
    [SerializeField] private AudioClip hitOk;
    [SerializeField] private AudioClip hitMeh;
    [SerializeField] private AudioClip hitMiss;
    [SerializeField] private AudioClip craftComplete;

    [Header("Ambient")]
    [SerializeField] private AudioClip forgeAmbient;

    [Header("UI")]
    [SerializeField] private AudioClip uiOpen;
    [SerializeField] private AudioClip uiClose;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip buttonHover;

    [Header("Economy")]
    [SerializeField] private AudioClip coinSound;

    [Header("Player")]
    [SerializeField] private AudioClip footstepLoop;
    [SerializeField] private AudioClip levelUp;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float ambientVolume = 0.4f;
    [Range(0f, 1f)] [SerializeField] private float uiVolume = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float footstepVolume = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartAmbient();
    }

    // ── Ambient ────────────────────────────────────────────────────────────────

    private void StartAmbient()
    {
        if (ambientSource == null || forgeAmbient == null) return;
        ambientSource.clip = forgeAmbient;
        ambientSource.loop = true;
        ambientSource.volume = ambientVolume;
        ambientSource.Play();
    }

    // ── Minigame ───────────────────────────────────────────────────────────────

    public void PlayHammerHit() => PlaySFX(hammerHit);

    public void PlayHitFeedback(HitQuality quality)
    {
        AudioClip clip = quality switch
        {
            HitQuality.Great => hitGreat,
            HitQuality.Ok    => hitOk,
            HitQuality.Meh   => hitMeh,
            _                => hitMiss
        };
        PlaySFX(clip);
    }

    public void PlayCraftComplete() => PlaySFX(craftComplete);

    // ── UI ──────────────────────────────────────────────────────────────────────

    public void PlayUIOpen()      => PlayUI(uiOpen);
    public void PlayUIClose()     => PlayUI(uiClose);
    public void PlayButtonClick() => PlayUI(buttonClick);
    public void PlayButtonHover() => PlayUI(buttonHover);

    // ── Economy ─────────────────────────────────────────────────────────────────

    public void PlayCoinSound() => PlaySFX(coinSound);

    // ── Player ──────────────────────────────────────────────────────────────────

    public void StartFootsteps()
    {
        if (footstepSource == null || footstepLoop == null) return;
        if (footstepSource.isPlaying) return;
        footstepSource.clip = footstepLoop;
        footstepSource.loop = true;
        footstepSource.volume = footstepVolume;
        footstepSource.Play();
    }

    public void StopFootsteps()
    {
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.Stop();
    }

    public void PlayLevelUp() => PlaySFX(levelUp);

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void PlayUI(AudioClip clip)
    {
        if (clip != null && uiSource != null)
            uiSource.PlayOneShot(clip, uiVolume);
    }
}
