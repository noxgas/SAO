using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Party panel - shows party members and management options.
/// </summary>
public class VRPartyPanel : VRMenuPanel
{
    [Header("Party Members")]
    [SerializeField] private Transform partyMembersContainer;
    [SerializeField] private VRPartyMember partyMemberPrefab;

    [Header("Actions")]
    [SerializeField] private VRMenuButton inviteButton;
    [SerializeField] private VRMenuButton promoteButton;
    [SerializeField] private VRMenuButton kickButton;
    [SerializeField] private VRMenuButton leaveButton;

    private List<VRPartyMember> memberUIs = new List<VRPartyMember>();

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 600f;
        panelHeight = 700f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        RefreshParty();
    }

    private void RefreshParty()
    {
        // TODO: Load party members
    }
}