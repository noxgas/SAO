using UnityEngine;
using TMPro;

/// <summary>
/// Skill Tree panel for VR menu system.
/// Integrates with VRMenuSystem.
/// </summary>
public class SkillTreePanel : VRMenuPanel
{
    [Header("Skill Tree UI")]
    [SerializeField] private SkillTreeUI skillTreeUI;
    [SerializeField] private TextMeshProUGUI titleText;

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 1200f;
        panelHeight = 900f;
    }

    private void Start()
    {
        titleText.text = "🌳 SKILL TREE 🌳";
        titleText.color = VRUIManager.Instance.GetAccentColor(AccentType.Green);
    }

    protected override void OnShow()
    {
        skillTreeUI.RefreshUI();
    }
}