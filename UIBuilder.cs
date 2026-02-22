using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to quickly build SAO-style UI elements.
/// </summary>
public static class UIBuilder
{
    /// <summary>
    /// Create a styled button.
    /// </summary>
    public static Button CreateButton(Transform parent, string buttonName, string text)
    {
        GameObject buttonGO = new GameObject(buttonName);
        buttonGO.transform.SetParent(parent);

        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.15f);

        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = image;

        TextMeshProUGUI textComponent = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        textComponent.transform.SetParent(buttonGO.transform);
        textComponent.text = text;
        textComponent.color = new Color(0, 1, 1);
        textComponent.alignment = TextAlignmentOptions.Center;

        return button;
    }

    /// <summary>
    /// Create a styled slider.
    /// </summary>
    public static Slider CreateSlider(Transform parent, string sliderName, float minValue, float maxValue)
    {
        GameObject sliderGO = new GameObject(sliderName);
        sliderGO.transform.SetParent(parent);

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = (minValue + maxValue) / 2;

        // Background
        GameObject backgroundGO = new GameObject("Background");
        backgroundGO.transform.SetParent(sliderGO.transform);
        Image backgroundImage = backgroundGO.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.15f);

        // Fill
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(sliderGO.transform);
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = new Color(0, 1, 1);

        slider.fillRect = fillGO.GetComponent<RectTransform>();

        return slider;
    }
}