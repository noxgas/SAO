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
    [SerializeField] private Color hoverColor = new Color(0.227f, 0.627f, 1f);
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
        buttonText.color = new Color(1, 1, 1, 1);

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        buttonImage.color = selectedColor;
        if (parentPanel != null)
            parentPanel.OnMenuOptionSelected(optionName);
    }

    private void OnMouseEnter()
    {
        buttonImage.color = hoverColor;
        transform.localScale = Vector3.one * 1.05f;
    }

    private void OnMouseExit()
    {
        buttonImage.color = normalColor;
        transform.localScale = Vector3.one;
    }
}