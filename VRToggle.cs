using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// VR-optimized toggle button.
/// </summary>
public class VRToggle : MonoBehaviour
{
    [SerializeField] private Image toggleImage;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Color onColor = new Color(0, 1, 0.5f);
    [SerializeField] private Color offColor = new Color(1, 0.2f, 0.2f);
    
    private bool isToggled = false;
    private Button button;

    public event Action<bool> OnValueChanged;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();

        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        isToggled = !isToggled;
        UpdateVisuals();
        OnValueChanged?.Invoke(isToggled);
    }

    public void SetValue(bool value)
    {
        isToggled = value;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        toggleImage.color = isToggled ? onColor : offColor;
        if (labelText != null)
            labelText.text = isToggled ? "ON" : "OFF";
    }

    public bool GetValue() => isToggled;
}