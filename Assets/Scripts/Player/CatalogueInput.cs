using UnityEngine;

public class CatalogueInput : MonoBehaviour
{
    [SerializeField] private GameObject cataloguePanel;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool isCatalogueOpen = false;

    public bool IsCatalogueOpen => isCatalogueOpen;

    private void Start()
    {
        if (cataloguePanel == null)
        {
            Debug.LogError("CatalogueInput: cataloguePanel is not assigned.");
            return;
        }

        EnsureCanvasGroup();
        SetVisible(false);
    }

    private void ShowCatalogue()
    {
        SetVisible(true);
    }

    private void HideCatalogue()
    {
        SetVisible(false);
    }

    public void CloseCatalogue()
    {
        if (cataloguePanel != null)
        {
            HideCatalogue();
        }
    }

    public void OpenCatalogue()
    {
        if (cataloguePanel != null)
        {
            isCatalogueOpen = true;
            ShowCatalogue();
        }
    }

    public void ToggleCatalogue()
    {
        if (cataloguePanel == null)
        {
            return;
        }

        SetVisible(!isCatalogueOpen);
    }

    private void EnsureCanvasGroup()
    {
        if (canvasGroup == null)
        {
            canvasGroup = cataloguePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = cataloguePanel.AddComponent<CanvasGroup>();
            }
        }
    }

    private void SetVisible(bool visible)
    {
        isCatalogueOpen = visible;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            cataloguePanel.SetActive(true);
        }
        else
        {
            cataloguePanel.SetActive(visible);
        }
    }
}
