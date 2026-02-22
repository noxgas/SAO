using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Base class for all UI panels.
/// Handles animations and transitions with SAO-style effects.
/// </summary>
public abstract class UIPanel : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected float animationDuration = 0.3f;

    [Header("SAO Style")]
    [SerializeField] protected Image borderImage;
    [SerializeField] protected Color accentColor = new Color(0, 1, 1); // Cyan

    protected bool isVisible = false;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
        isVisible = true;
        OnShow();
    }

    public virtual void Hide()
    {
        StartCoroutine(FadeOut());
        isVisible = false;
    }

    protected IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / animationDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    protected IEnumerator FadeOut()
    {
        canvasGroup.alpha = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / animationDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    protected virtual void OnShow() { }
}