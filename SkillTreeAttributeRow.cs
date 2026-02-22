using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single row in skill tree UI for one attribute.
/// Shows current value, base value, cost, and allocation buttons.
/// </summary>
public class SkillTreeAttributeRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attributeNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI currentValueText;
    [SerializeField] private TextMeshProUGUI investedPointsText;
    [SerializeField] private TextMeshProUGUI costPerPointText;
    
    [SerializeField] private Button addPointButton;
    [SerializeField] private Button addFivePointsButton;
    [SerializeField] private Button addTenPointsButton;
    
    [SerializeField] private Image valueBar;
    [SerializeField] private Color normalColor = new Color(0.227f, 0.627f, 1f);
    [SerializeField] private Color highlightColor = new Color(0, 1, 0.5f);

    private SkillTreeSystem.AttributeInfo attributeInfo;
    private SkillTreeSystem skillTreeSystem;
    private SkillTreeUI parentUI;

    public void Initialize(SkillTreeSystem.AttributeInfo info, SkillTreeSystem skillTree, SkillTreeUI ui)
    {
        attributeInfo = info;
        skillTreeSystem = skillTree;
        parentUI = ui;

        // Setup text
        attributeNameText.text = info.displayName;
        descriptionText.text = info.description;

        // Setup buttons
        addPointButton.onClick.AddListener(() => AllocatePoints(1));
        addFivePointsButton.onClick.AddListener(() => AllocatePoints(5));
        addTenPointsButton.onClick.AddListener(() => AllocatePoints(10));

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        currentValueText.text = $"Value: {attributeInfo.currentValue:F1}";
        investedPointsText.text = $"Level: {attributeInfo.pointsInvested}";
        costPerPointText.text = $"Cost: {attributeInfo.costPerPoint} pts/level";

        // Update bar
        float maxValue = attributeInfo.baseValue * 3f; // Display bar up to 3x base
        float fillAmount = Mathf.Clamp01(attributeInfo.currentValue / maxValue);
        valueBar.fillAmount = fillAmount;
    }

    private void AllocatePoints(int pointCount)
    {
        for (int i = 0; i < pointCount; i++)
        {
            if (!skillTreeSystem.AllocatePoint(attributeInfo.type))
                break;
        }

        UpdateDisplay();
        parentUI.RefreshUI();
    }
}