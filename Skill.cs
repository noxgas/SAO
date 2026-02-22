using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced skill system with cooldowns, resource costs, and scaling mechanics.
/// Supports skill trees, upgrades, and progression.
/// </summary>
[System.Serializable]
public class Skill
{
    [SerializeField] private string skillId;
    [SerializeField] private string skillName;
    [SerializeField] private string description;
    [SerializeField] private SkillType skillType;
    [SerializeField] private DamageType damageType;

    [Header("Resource Costs")]
    [SerializeField] private float manaCost = 0f;
    [SerializeField] private float staminaCost = 0f;
    [SerializeField] private float healthCost = 0f;

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 0f;
    [SerializeField] private float currentCooldown = 0f;

    [Header("Damage")]
    [SerializeField] private float baseDamage = 0f;
    [SerializeField] private float scalingFactor = 1f;
    [SerializeField] private int skillLevel = 1;
    [SerializeField] private int maxSkillLevel = 10;

    [Header("Effects")]
    [SerializeField] private List<StatusEffect> statusEffects = new List<StatusEffect>();
    [SerializeField] private float range = 0f;
    [SerializeField] private bool isInstant = true;
    [SerializeField] private bool canCrit = true;
    [SerializeField] private float critMultiplier = 1.5f;

    private float lastUsedTime = 0f;
    private int timesUsed = 0;
    private float totalDamageDealt = 0f;

    // PUBLIC PROPERTIES
    public string SkillId => skillId;
    public string SkillName => skillName;
    public string Description => description;
    public SkillType Type => skillType;
    public DamageType DamageType => damageType;
    public float CurrentCooldown => currentCooldown;
    public float BaseDamage => baseDamage;
    public int SkillLevel => skillLevel;
    public int MaxSkillLevel => maxSkillLevel;
    public float ManaCost => manaCost;
    public float StaminaCost => staminaCost;
    public float Range => range;
    public bool IsInstant => isInstant;
    public bool CanCrit => canCrit;
    public int TimesUsed => timesUsed;
    public float TotalDamageDealt => totalDamageDealt;

    /// <summary>
    /// Check if the skill is off cooldown.
    /// </summary>
    public bool IsReady => currentCooldown <= 0f;

    /// <summary>
    /// Calculate the final damage with all multipliers.
    /// </summary>
    public float CalculateDamage(CharacterStats stats, int level)
    {
        float damage = baseDamage;

        // Apply level scaling
        damage += (level - 1) * scalingFactor;

        // Apply stat scaling
        damage *= stats.PhysicalDamage;

        // Apply skill level bonus
        damage *= (1f + ((skillLevel - 1) * 0.1f));

        // Apply damage type multiplier
        damage *= GetDamageTypeMultiplier(damageType);

        return damage;
    }

    /// <summary>
    /// Get the damage type multiplier.
    /// </summary>
    private float GetDamageTypeMultiplier(DamageType type)
    {
        return type switch
        {
            DamageType.Physical => 1.0f,
            DamageType.Magical => 1.2f,
            DamageType.Elemental => 1.15f,
            DamageType.True => 1.0f,
            DamageType.Hybrid => 1.1f,
            DamageType.Fire => 1.15f,
            DamageType.Ice => 1.15f,
            DamageType.Lightning => 1.15f,
            DamageType.Earth => 1.15f,
            DamageType.Poison => 1.2f,
            DamageType.Bleed => 1.0f,
            DamageType.Holy => 1.25f,
            DamageType.Shadow => 1.25f,
            _ => 1.0f
        };
    }

    /// <summary>
    /// Use the skill.
    /// </summary>
    public bool Use(CharacterStats stats, int level)
    {
        if (!IsReady)
        {
            Debug.LogWarning($"⏱️ {skillName} is on cooldown for {currentCooldown:F2}s");
            return false;
        }

        currentCooldown = cooldownTime;
        lastUsedTime = Time.time;
        timesUsed++;

        float damage = CalculateDamage(stats, level);

        // Check for critical hit
        if (canCrit && UnityEngine.Random.value < 0.15f)
        {
            damage *= critMultiplier;
            Debug.Log($"💥 {skillName} - CRITICAL HIT! {damage:F1} damage");
        }
        else
        {
            Debug.Log($"⚡ {skillName} - {damage:F1} damage");
        }

        totalDamageDealt += damage;
        return true;
    }

    /// <summary>
    /// Upgrade the skill.
    /// </summary>
    public bool UpgradeSkill()
    {
        if (skillLevel >= maxSkillLevel)
        {
            Debug.LogWarning($"❌ {skillName} is already at max level!");
            return false;
        }

        skillLevel++;
        Debug.Log($"📈 {skillName} upgraded to level {skillLevel}!");
        return true;
    }

    /// <summary>
    /// Update cooldown.
    /// </summary>
    public void UpdateCooldown()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Get skill statistics.
    /// </summary>
    public string GetStats()
    {
        return $"{skillName} | Lvl: {skillLevel}/{maxSkillLevel} | DMG: {baseDamage:F1} | " +
               $"CD: {cooldownTime:F2}s | Used: {timesUsed} times | Total DMG: {totalDamageDealt:F1}";
    }
}

/// <summary>
/// Skill type enumeration.
/// </summary>
public enum SkillType
{
    Active,
    Passive,
    Ultimate,
    Special,
    Channeled
}

/// <summary>
/// Status effect configuration.
/// </summary>
[System.Serializable]
public class StatusEffect
{
    public string effectName;
    public float duration;
    public float damagePerTick;
    public int tickCount;
}

/// <summary>
/// Skill manager for tracking all player skills.
/// </summary>
public class SkillManager : MonoBehaviour
{
    private Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // Update all skill cooldowns
        foreach (var skill in skills.Values)
        {
            skill.UpdateCooldown();
        }
    }

    /// <summary>
    /// Add a skill to the manager.
    /// </summary>
    public void AddSkill(Skill skill)
    {
        skills[skill.SkillId] = skill;
        Debug.Log($"✓ Skill added: {skill.SkillName}");
    }

    /// <summary>
    /// Use a skill by ID.
    /// </summary>
    public bool UseSkill(string skillId)
    {
        if (skills.TryGetValue(skillId, out var skill))
        {
            return skill.Use(player.CharacterClass.BaseStats, player.Level);
        }

        Debug.LogWarning($"❌ Skill {skillId} not found!");
        return false;
    }

    /// <summary>
    /// Get a skill by ID.
    /// </summary>
    public Skill GetSkill(string skillId)
    {
        return skills.TryGetValue(skillId, out var skill) ? skill : null;
    }

    /// <summary>
    /// Get all skills.
    /// </summary>
    public Dictionary<string, Skill> GetAllSkills() => skills;
}