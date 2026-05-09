using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialPopup : MonoBehaviour
{
    [Header("Configuração")]
    public float fadeDuration = 0.4f;

    private CanvasGroup canvasGroup;
    private Image currentImage;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(Image tutorialImage)
    {
        StopAllCoroutines();

        if (currentImage != null && currentImage != tutorialImage)
            currentImage.gameObject.SetActive(false);

        currentImage = tutorialImage;
        if (currentImage != null)
            currentImage.gameObject.SetActive(true);

        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        if (currentImage != null)
        {
            currentImage.gameObject.SetActive(false);
            currentImage = null;
        }
    }
}