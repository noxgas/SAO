using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Advanced skill tree system with talent nodes, prerequisites, and progression tracking.
/// Supports multiple specialization paths, respec mechanics, and achievement integration.
/// </summary>
public class SkillTree : MonoBehaviour
{
    [Header("Skill Tree Configuration")]
    [SerializeField] private int skillPoints = 0;
    [SerializeField] private int maxSkillPoints = 100;
    [SerializeField] private float skillPointRegenRate = 1f;

    private Player player;
    private Dictionary<string, TalentNode> talentNodes = new Dictionary<string, TalentNode>();
    private Dictionary<string, SkillTreePath> skillPaths = new Dictionary<string, SkillTreePath>();
    private HashSet<string> unlockedNodes = new HashSet<string>();

    // Bonuses tracking
    private float comboMultiplierBonus = 0f;
    private float damageBonus = 0f;
    private float defenseBonus = 0f;
    private float healthBonus = 0f;
    private float critChanceBonus = 0f;
    private float manaRegenBonus = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();
        InitializeSkillTree();
    }

    private void Update()
    {
        RegenerateSkillPoints();
    }

    /// <summary>
    /// Initialize the skill tree with all talent nodes.
    /// </summary>
    private void InitializeSkillTree()
    {
        // Create main paths
        skillPaths["Offense"] = new SkillTreePath("Offense", "Increase damage and critical chance");
        skillPaths["Defense"] = new SkillTreePath("Defense", "Increase health and defense");
        skillPaths["Utility"] = new SkillTreePath("Utility", "Increase special abilities");
        skillPaths["Mastery"] = new SkillTreePath("Mastery", "Unlock class-specific abilities");

        // Offense Path Nodes
        CreateNode("OffenseStart", "Weapon Training", "Increase physical damage by 5%", "Offense", null, 0, new BonusStats { damageBonus = 0.05f });
        CreateNode("CritChance", "Critical Strike", "Increase critical chance by 10%", "Offense", "OffenseStart", 2, new BonusStats { critChanceBonus = 0.1f });
        CreateNode("CritDamage", "Executioner", "Increase critical damage by 50%", "Offense", "CritChance", 3, new BonusStats { critDamageBonus = 0.5f });
        CreateNode("OffenseMastery", "Offensive Mastery", "Increase all damage by 20%", "Offense", "CritDamage", 5, new BonusStats { damageBonus = 0.2f });

        // Defense Path Nodes
        CreateNode("DefenseStart", "Thick Skin", "Increase defense by 10%", "Defense", null, 0, new BonusStats { defenseBonus = 0.1f });
        CreateNode("HealthPool", "Iron Constitution", "Increase max health by 50", "Defense", "DefenseStart", 2, new BonusStats { healthBonus = 50f });
        CreateNode("Regeneration", "Life Drain", "Regenerate 2% health per second", "Defense", "HealthPool", 3, new BonusStats { healthRegenBonus = 0.02f });
        CreateNode("DefenseMastery", "Defensive Mastery", "Increase defense by 30%", "Defense", "Regeneration", 5, new BonusStats { defenseBonus = 0.3f });

        // Utility Path Nodes
        CreateNode("UtilityStart", "Agility", "Increase movement speed by 10%", "Utility", null, 0, new BonusStats { moveSpeedBonus = 0.1f });
        CreateNode("ComboMastery", "Combo Master", "Increase combo multiplier by 15%", "Utility", "UtilityStart", 2, new BonusStats { comboMultiplierBonus = 0.15f });
        CreateNode("ResourceManagement", "Resource Management", "Increase mana regen by 25%", "Utility", "ComboMastery", 3, new BonusStats { manaRegenBonus = 0.25f });
        CreateNode("UtilityMastery", "Utility Mastery", "Unlock special mechanics", "Utility", "ResourceManagement", 5, new BonusStats { });

        // Mastery Path Nodes
        CreateNode("ClassMastery", "Class Mastery", "Unlock class-specific abilities", "Mastery", null, 0, new BonusStats { damageBonus = 0.1f, defenseBonus = 0.1f });

        Debug.Log($"✓ Skill tree initialized with {talentNodes.Count} talent nodes");
    }

    /// <summary>
    /// Create a talent node.
    /// </summary>
    private void CreateNode(string nodeId, string nodeName, string description, string path, string prerequisite, int requiredLevel, BonusStats bonuses)
    {
        var node = new TalentNode
        {
            nodeId = nodeId,
            nodeName = nodeName,
            description = description,
            path = path,
            prerequisiteNode = prerequisite,
            requiredLevel = requiredLevel,
            skillPointCost = Mathf.Max(1, requiredLevel / 2),
            bonuses = bonuses
        };

        talentNodes[nodeId] = node;
    }

    /// <summary>
    /// Unlock a talent node if prerequisites are met.
    /// </summary>
    public bool UnlockNode(string nodeId)
    {
        if (!talentNodes.TryGetValue(nodeId, out var node))
        {
            Debug.LogWarning($"❌ Node {nodeId} not found!");
            return false;
        }

        // Check if already unlocked
        if (unlockedNodes.Contains(nodeId))
        {
            Debug.LogWarning($"⚠️ {node.nodeName} already unlocked!");
            return false;
        }

        // Check level requirement
        if (player.Level < node.requiredLevel)
        {
            Debug.LogWarning($"❌ Requires level {node.requiredLevel}, you are level {player.Level}");
            return false;
        }

        // Check prerequisite
        if (!string.IsNullOrEmpty(node.prerequisiteNode) && !unlockedNodes.Contains(node.prerequisiteNode))
        {
            Debug.LogWarning($"❌ Prerequisite {node.prerequisiteNode} not unlocked!");
            return false;
        }

        // Check skill points
        if (skillPoints < node.skillPointCost)
        {
            Debug.LogWarning($"❌ Not enough skill points! Need {node.skillPointCost}, have {skillPoints}");
            return false;
        }

        // Unlock node
        unlockedNodes.Add(nodeId);
        skillPoints -= node.skillPointCost;
        ApplyNodeBonuses(node.bonuses);

        Debug.Log($"🔓 Unlocked: {node.nodeName} | Path: {node.path}");
        Debug.Log($"Skill Points: {skillPoints}/{maxSkillPoints}");

        return true;
    }

    /// <summary>
    /// Apply bonuses from an unlocked node.
    /// </summary>
    private void ApplyNodeBonuses(BonusStats bonuses)
    {
        damageBonus += bonuses.damageBonus;
        defenseBonus += bonuses.defenseBonus;
        healthBonus += bonuses.healthBonus;
        critChanceBonus += bonuses.critChanceBonus;
        manaRegenBonus += bonuses.manaRegenBonus;
        comboMultiplierBonus += bonuses.comboMultiplierBonus;

        Debug.Log($"✨ Applied bonuses: DMG +{bonuses.damageBonus * 100:F0}% | DEF +{bonuses.defenseBonus * 100:F0}% | HP +{bonuses.healthBonus}");
    }

    /// <summary>
    /// Regenerate skill points over time.
    /// </summary>
    private void RegenerateSkillPoints()
    {
        if (skillPoints < maxSkillPoints)
        {
            skillPoints = Mathf.Min(skillPoints + (skillPointRegenRate * Time.deltaTime), maxSkillPoints);
        }
    }

    /// <summary>
    /// Get combo multiplier bonus from skill tree.
    /// </summary>
    public float GetComboMultiplierBonus()
    {
        return 1f + comboMultiplierBonus;
    }

    /// <summary>
    /// Get damage bonus from skill tree.
    /// </summary>
    public float GetDamageBonus()
    {
        return 1f + damageBonus;
    }

    /// <summary>
    /// Get defense bonus from skill tree.
    /// </summary>
    public float GetDefenseBonus()
    {
        return 1f + defenseBonus;
    }

    /// <summary>
    /// Get critical chance bonus.
    /// </summary>
    public float GetCritChanceBonus()
    {
        return critChanceBonus;
    }

    /// <summary>
    /// Get skill tree statistics.
    /// </summary>
    public string GetSkillTreeStats()
    {
        return $"Skill Points: {skillPoints:F0}/{maxSkillPoints} | Nodes Unlocked: {unlockedNodes.Count}/{talentNodes.Count} | " +
               $"DMG: +{(damageBonus * 100):F0}% | DEF: +{(defenseBonus * 100):F0}% | Crit: +{(critChanceBonus * 100):F0}%";
    }

    public int GetSkillPoints() => (int)skillPoints;
    public Dictionary<string, TalentNode> GetAllNodes() => talentNodes;
    public HashSet<string> GetUnlockedNodes() => unlockedNodes;
}

/// <summary>
/// Talent node in the skill tree.
/// </summary>
[System.Serializable]
public class TalentNode
{
    public string nodeId;
    public string nodeName;
    public string description;
    public string path;
    public string prerequisiteNode;
    public int requiredLevel;
    public int skillPointCost;
    public BonusStats bonuses;
}

/// <summary>
/// Skill tree path for organizing nodes.
/// </summary>
[System.Serializable]
public class SkillTreePath
{
    public string pathName;
    public string pathDescription;

    public SkillTreePath(string name, string description)
    {
        pathName = name;
        pathDescription = description;
    }
}

/// <summary>
/// Bonus stats applied by talent nodes.
/// </summary>
[System.Serializable]
public class BonusStats
{
    public float damageBonus = 0f;
    public float critDamageBonus = 0f;
    public float defenseBonus = 0f;
    public float healthBonus = 0f;
    public float critChanceBonus = 0f;
    public float moveSpeedBonus = 0f;
    public float healthRegenBonus = 0f;
    public float manaRegenBonus = 0f;
    public float comboMultiplierBonus = 0f;
}