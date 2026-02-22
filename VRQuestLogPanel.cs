using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Quest log panel - shows active, completed, and failed quests.
/// </summary>
public class VRQuestLogPanel : VRMenuPanel
{
    [Header("Quest Tabs")]
    [SerializeField] private VRMenuButton activeTabButton;
    [SerializeField] private VRMenuButton completedTabButton;
    [SerializeField] private VRMenuButton failedTabButton;

    [Header("Quest List")]
    [SerializeField] private Transform questListContainer;
    [SerializeField] private VRQuestItem questItemPrefab;

    [Header("Quest Details")]
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Transform objectivesContainer;
    [SerializeField] private TextMeshProUGUI rewardsText;
    [SerializeField] private TextMeshProUGUI hintText;

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 800f;
        panelHeight = 900f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        RefreshQuestLog();
    }

    private void RefreshQuestLog()
    {
        // TODO: Load quests
    }
}