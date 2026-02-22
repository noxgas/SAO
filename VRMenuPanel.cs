using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Base class for all VR menu panels.
/// Handles positioning, animations, and interactivity.
/// </summary>
public class VRMenuPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] protected RectTransform panelRect;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Image panelBackground;
    [SerializeField] protected float panelWidth = 400f;
    [SerializeField] protected float panelHeight = 500f;
    [SerializeField] protected float panelCurve = 12f; // Slight curve (10-15 degrees)

    [Header("Animation")]
    [SerializeField] protected float fadeInDuration = 0.2f;
    [SerializeField] protected float scaleAnimationDuration = 0.3f;

    [Header("Dragging")]
    [SerializeField] protected bool isDraggable = true;
    [SerializeField] protected Image dragHandle;

    protected VRMenuSystem menuSystem;
    protected string panelType;
    protected Vector3 spawnOffset;
    protected bool isVisible = false;
    protected Vector3 dragOffset;
    protected bool isDragging = false;

    protected virtual void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (panelBackground == null)
            panelBackground = GetComponent<Image>();

        SetupHolographicStyle();
    }

    private void SetupHolographicStyle()
    {
        // Set holographic blue background
        if (panelBackground != null)
        {
            panelBackground.color = new Color(0.05f, 0.1f, 0.2f, 0.7f); // Dark blue, semi-transparent
        }

        // Add border image for glow effect
        if (panelRect != null)
        {
            panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        }
    }

    public virtual void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        menuSystem = system;
        panelType = type;
        spawnOffset = offset;

        if (isDraggable && dragHandle != null)
        {
            Button dragButton = dragHandle.GetComponent<Button>();
            if (dragButton == null)
                dragButton = dragHandle.gameObject.AddComponent<Button>();

            // Setup dragging
            EventTrigger trigger = dragHandle.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = dragHandle.gameObject.AddComponent<EventTrigger>();
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(AnimateIn());
        isVisible = true;
    }

    public virtual void Hide()
    {
        StartCoroutine(AnimateOut());
    }

    protected virtual IEnumerator AnimateIn()
    {
        // Fade in
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;

        // Scale from 95% to 100%
        Vector3 startScale = Vector3.one * 0.95f;
        Vector3 endScale = Vector3.one;

        while (elapsedTime < scaleAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleAnimationDuration;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localScale = endScale;

        // Subtle glow pulse
        StartCoroutine(AnimateGlowPulse());
    }

    protected virtual IEnumerator AnimateOut()
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < scaleAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleAnimationDuration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.95f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        isVisible = false;
    }

    protected virtual IEnumerator AnimateGlowPulse()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Subtle glow effect
            if (panelBackground != null)
            {
                Color pulseColor = new Color(0.05f, 0.1f, 0.2f, 0.7f + Mathf.Sin(t * Mathf.PI) * 0.1f);
                panelBackground.color = pulseColor;
            }

            yield return null;
        }
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }
}