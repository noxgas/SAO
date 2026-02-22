using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Base class for all VR menu panels.
/// </summary>
public class VRMenuPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] protected RectTransform panelRect;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Image panelBackground;
    [SerializeField] protected float panelWidth = 400f;
    [SerializeField] protected float panelHeight = 500f;

    [Header("Animation")]
    [SerializeField] protected float fadeInDuration = 0.2f;
    [SerializeField] protected float scaleAnimationDuration = 0.3f;

    protected VRMenuSystem menuSystem;
    protected string panelType;
    protected Vector3 spawnOffset;
    public bool isVisible { get; protected set; } = false;

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
        if (panelBackground != null)
        {
            panelBackground.color = new Color(0.05f, 0.1f, 0.2f, 0.7f);
        }

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
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;

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

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }
}