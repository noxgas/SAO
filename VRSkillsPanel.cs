using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Skills panel - shows weapon, passive, crafting, and unique skills.
/// Includes skill tree visualization.
/// </summary>
public class VRSkillsPanel : VRMenuPanel
{
    [Header("Skill Tabs")]
    [SerializeField] private Transform skillTabsContainer;
    [SerializeField] private VRMenuButton skillTabPrefab;

    [Header("Skill Tree")]
    [SerializeField] private Transform skillTreeContainer;
    [SerializeField] private VRSkillNode skillNodePrefab;

    [Header("Skill Details")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private Image skillXPBar;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI spCostText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private string currentSkillTab = "Weapon Skills";

    private string[] skillTabs = new string[]
    {
        "Weapon Skills",
        "Passive Skills",
        "Crafting Skills",
        "Unique Skill"
    };

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 1000f;
        panelHeight = 900f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        CreateSkillTabs();
        RefreshSkillTree();
    }

    private void CreateSkillTabs()
    {
        foreach (string tab in skillTabs)
        {
            VRMenuButton tabButton = Instantiate(skillTabPrefab, skillTabsContainer);
            tabButton.Initialize(tab, null);
        }
    }

    private void RefreshSkillTree()
    {
        // Clear existing nodes
        foreach (Transform child in skillTreeContainer)
        {
            Destroy(child.gameObject);
        }

        // Create skill nodes based on current tab
        // TODO: Load skills from player
    }
}