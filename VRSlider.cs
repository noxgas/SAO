#if STEAMVR_PRESENT
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Valve.VR;
using System;

/// <summary>
/// VR-optimized slider that can be controlled with controller thumbstick.
/// </summary>
public class VRSlider : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image handleImage;
    [SerializeField] private TextMeshProUGUI valueLabel;
    
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 1f;
    [SerializeField] private float currentValue = 0.5f;
    
    [SerializeField] private SteamVR_Input_Sources controllerHand = SteamVR_Input_Sources.RightHand;
    [SerializeField] private float thumbstickSensitivity = 0.5f;

    private RectTransform rectTransform;
    private RectTransform fillRect;
    private RectTransform handleRect;
    private bool isSelected = false;

    public event Action<float> OnValueChanged;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        fillRect = fillImage.GetComponent<RectTransform>();
        handleRect = handleImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Add button for selection
        Button selectButton = gameObject.AddComponent<Button>();
        selectButton.onClick.AddListener(() => isSelected = !isSelected);
    }

    private void Update()
    {
        if (!isSelected) return;

        // Get thumbstick input
        Vector2 thumbstickAxis = SteamVR_Input.GetAxis(SteamVR_Actions.default_Move, controllerHand);
        
        // Update slider value based on thumbstick X axis
        float delta = thumbstickAxis.x * thumbstickSensitivity * Time.deltaTime;
        currentValue = Mathf.Clamp(currentValue + delta, minValue, maxValue);

        UpdateSliderVisuals();
        OnValueChanged?.Invoke(currentValue);
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, minValue, maxValue);
        UpdateSliderVisuals();
    }

    private void UpdateSliderVisuals()
    {
        float normalizedValue = (currentValue - minValue) / (maxValue - minValue);
        fillRect.anchorMax = new Vector2(normalizedValue, 1f);
        
        if (valueLabel != null)
            valueLabel.text = $"{Mathf.RoundToInt(currentValue * 100)}%";
    }

    public float GetValue() => currentValue;
}
#endif // STEAMVR_PRESENT
