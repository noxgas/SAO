using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI panel for skill tree in VR menu.
/// Allows players to allocate skill points to attributes.
/// </summary>
public class SkillTreeUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI availablePointsText;
    [SerializeField] private TextMeshProUGUI spentPointsText;
    [SerializeField] private TextMeshProUGUI totalPointsText;
    
    [Header("Attribute Display")]
    [SerializeField] private Transform attributeContainer;
    [SerializeField] private SkillTreeAttributeRow attributeRowPrefab;
    
    [Header("Buttons")]
    [SerializeField] private Button respecButton;
    [SerializeField] private TextMeshProUGUI respecCostText;

    private SkillTreeSystem skillTreeSystem;
    private Player player;
    private List<SkillTreeAttributeRow> attributeRows = new List<SkillTreeAttributeRow>();

    private void Start()
    {
        player = FindObjectOfType<Player>();
        skillTreeSystem = player.GetComponent<SkillTreeSystem>();

        titleText.text = "🌳 SKILL TREE 🌳";
        respecButton.onClick.AddListener(OnRespecClicked);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (skillTreeSystem == null)
            return;

        // Update point totals
        availablePointsText.text = $"Available: {skillTreeSystem.GetAvailableSkillPoints()}";
        spentPointsText.text = $"Spent: {skillTreeSystem.GetSpentSkillPoints()}";
        totalPointsText.text = $"Total Earned: {skillTreeSystem.GetTotalSkillPointsEarned()}";

        // Clear and rebuild attribute rows
        foreach (var row in attributeRows)
        {
            Destroy(row.gameObject);
        }
        attributeRows.Clear();

        // Create rows for each attribute
        var attributes = skillTreeSystem.GetAllAttributes();
        foreach (var attribute in attributes)
        {
            SkillTreeAttributeRow row = Instantiate(attributeRowPrefab, attributeContainer);
            row.Initialize(attribute, skillTreeSystem, this);
            attributeRows.Add(row);
        }
    }

    private void OnRespecClicked()
    {
        Debug.Log("Respeccing skill tree...");
        skillTreeSystem.ResetAllocations(1000); // 1000 gold cost
        RefreshUI();
    }

    private void Update()
    {
        // Refresh UI every frame to show real-time updates
        if (skillTreeSystem != null && isActiveAndEnabled)
        {
            availablePointsText.text = $"Available: {skillTreeSystem.GetAvailableSkillPoints()}";
        }
    }
}