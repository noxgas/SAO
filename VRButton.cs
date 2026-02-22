using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// VR-optimized button with hover effects.
/// </summary>
public class VRButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    [SerializeField] private Color normalColor = new Color(0.1f, 0.1f, 0.15f);
    [SerializeField] private Color hoverColor = new Color(0, 1, 1);
    [SerializeField] private Color pressedColor = new Color(0, 0.7f, 0.7f);

    [SerializeField] private float scaleOnHover = 1.05f;
    private Vector3 originalScale;

    public event Action OnClicked;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button == null)
            button = gameObject.AddComponent<Button>();

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        originalScale = transform.localScale;
        
        button.onClick.AddListener(OnButtonClicked);
    }

    public void OnPointerEnter()
    {
        buttonImage.color = hoverColor;
        transform.localScale = originalScale * scaleOnHover;
    }

    public void OnPointerExit()
    {
        buttonImage.color = normalColor;
        transform.localScale = originalScale;
    }

    private void OnButtonClicked()
    {
        OnClicked?.Invoke();
    }

    public void SetText(string text)
    {
        if (buttonText != null)
            buttonText.text = text;
    }
}