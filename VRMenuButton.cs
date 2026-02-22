using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interactive menu button for VR with hover effects.
/// </summary>
public class VRMenuButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;

    [SerializeField] private Color normalColor = new Color(0.05f, 0.15f, 0.35f);
    [SerializeField] private Color hoverColor = new Color(0.227f, 0.627f, 1f); // #3aa0ff
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.8f, 1f);

    private VRMainMenuPanel parentPanel;
    private string optionName;

    public void Initialize(string name, VRMainMenuPanel parent)
    {
        optionName = name;
        parentPanel = parent;

        if (button == null)
            button = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        buttonText.text = optionName;
        buttonText.color = new Color(1, 1, 1, 1); // White text

        // Add hover event listeners
        button.onClick.AddListener(OnButtonClicked);

        // Setup EventTrigger for hover
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<EventTrigger>();

        AddEventTrigger(trigger, EventTriggerType.PointerEnter, OnPointerEnter);
        AddEventTrigger(trigger, EventTriggerType.PointerExit, OnPointerExit);
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => callback(data));
        trigger.triggers.Add(entry);
    }

    private void OnPointerEnter(BaseEventData data)
    {
        buttonImage.color = hoverColor;
        transform.localScale = Vector3.one * 1.05f;
    }

    private void OnPointerExit(BaseEventData data)
    {
        buttonImage.color = normalColor;
        transform.localScale = Vector3.one;
    }

    private void OnButtonClicked()
    {
        buttonImage.color = selectedColor;
        parentPanel.OnMenuOptionSelected(optionName);
    }
}