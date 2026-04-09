using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Removes duplicate EventSystems and AudioListeners at startup.
/// Attach to one GameObject in each scene (e.g. the main Camera or a manager object).
/// Ideally, fix the duplicates in the scene hierarchy and remove this script.
/// </summary>
public class SceneCleanup : MonoBehaviour
{
    private void Awake()
    {
        DeduplicateEventSystems();
        DeduplicateAudioListeners();
    }

    private void DeduplicateEventSystems()
    {
        var all = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (all.Length <= 1) return;

        for (int i = 1; i < all.Length; i++)
        {
            Debug.Log($"SceneCleanup: Destroying duplicate EventSystem on '{all[i].gameObject.name}'");
            Destroy(all[i].gameObject);
        }
    }

    private void DeduplicateAudioListeners()
    {
        var all = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (all.Length <= 1) return;

        for (int i = 1; i < all.Length; i++)
        {
            Debug.Log($"SceneCleanup: Disabling duplicate AudioListener on '{all[i].gameObject.name}'");
            Destroy(all[i]);
        }
    }
}
