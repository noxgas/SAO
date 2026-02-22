using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Advanced character class interface with full progression, stat calculation, and ability systems.
/// Supports scaling, synergy systems, and dynamic stat modifiers.
/// </summary>
public interface ICharacterClass
{
    string ClassName { get; }
    string SubclassName { get; }
    string ClassDescription { get; }
    CharacterStats BaseStats { get; }
    List<string> StartingSkills { get; }
    List<PassiveAbility> PassiveAbilities { get; }
    ClassDamageMultipliers DamageMultipliers { get; }
    ClassSynergySystem SynergySystem { get; }
    ClassStatScaler StatScaler { get; }

    void Initialize(Player playerRef);
    void OnLevelUp(int newLevel);
    void OnTakeDamage(float damage);
    void OnDealDamage(float damage);
    void ExecuteAbility(string abilityName);
    float GetStatBonus(string statName);
    float GetAbilityCooldown(string abilityName);

    // Advanced methods
    void OnKill(BossEnemy enemy);
    void OnStatusEffectApplied(StatusEffectType effectType);
    void OnComboHit(int comboCount);
    float CalculateEffectiveDamage(float baseDamage, DamageType type, CharacterStats targetStats);
    string GetClassDetailedStats();
    Dictionary<string, float> GetAllStatBonuses();
}

/// <summary>
/// Advanced passive ability with conditional effects and scaling.
/// </summary>
[System.Serializable]
public class PassiveAbility
{
    public string abilityName;
    public string description;
    public float effectValue;
    public int requiredLevel;
    public PassiveEffectType effectType;
    [Range(1, 5)]
    public int rarity = 1; // 1=Common, 5=Legendary
    public bool isConditional = false;
    public string condition = ""; // "OnCrit", "OnKill", "OnDodge"
    public float triggerChance = 1f;
    public float duration = 0f; // 0 = permanent
    public float cooldown = 0f;

    private float lastTriggeredTime = 0f;
    private float activationTime = 0f;

    public bool CanTrigger()
    {
        if (Time.time - lastTriggeredTime < cooldown)
            return false;

        if (duration > 0 && Time.time - activationTime > duration)
            return false;

        return Random.value < triggerChance;
    }

    public void OnTriggered()
    {
        lastTriggeredTime = Time.time;
        if (duration > 0)
            activationTime = Time.time;
    }

    public string GetAbilityInfo()
    {
        return $"{abilityName} [{GetRarityName()}] | {description} | Value: {effectValue}";
    }

    private string GetRarityName()
    {
        return rarity switch
        {
            1 => "Common",
            2 => "Uncommon",
            3 => "Rare",
            4 => "Epic",
            5 => "Legendary",
            _ => "Unknown"
        };
    }
}

/// <summary>
/// Advanced damage multiplier system with type effectiveness.
/// </summary>
[System.Serializable]
public class ClassDamageMultipliers
{
    public float physicalDamageMultiplier = 1f;
    public float magicalDamageMultiplier = 1f;
    public float criticalDamageMultiplier = 1.5f;
    public float elementalDamageMultiplier = 1f;
    public float trueShortDamageMultiplier = 1f;

    // Type-specific effectiveness
    [Header("Element Effectiveness")]
    public float fireEffectiveness = 1f;
    public float iceEffectiveness = 1f;
    public float lightningEffectiveness = 1f;
    public float poisonEffectiveness = 1f;
    public float holyEffectiveness = 1f;
    public float shadowEffectiveness = 1f;

    public float GetDamageMultiplier(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physical => physicalDamageMultiplier,
            DamageType.Magical => magicalDamageMultiplier,
            DamageType.Fire => fireEffectiveness * elementalDamageMultiplier,
            DamageType.Ice => iceEffectiveness * elementalDamageMultiplier,
            DamageType.Lightning => lightningEffectiveness * elementalDamageMultiplier,
            DamageType.Poison => poisonEffectiveness * elementalDamageMultiplier,
            DamageType.Holy => holyEffectiveness * elementalDamageMultiplier,
            DamageType.Shadow => shadowEffectiveness * elementalDamageMultiplier,
            DamageType.True => trueShortDamageMultiplier,
            DamageType.Hybrid => (physicalDamageMultiplier + magicalDamageMultiplier) / 2f,
            _ => 1f
        };
    }

    public float GetCriticalDamage(float baseDamage, bool isCritical)
    {
        return isCritical ? baseDamage * criticalDamageMultiplier : baseDamage;
    }
}

/// <summary>
/// Passive ability effect type with advanced options.
/// </summary>
public enum PassiveEffectType
{
    DamageBonus,
    DefenseBonus,
    HealthBonus,
    CriticalDamageBonus,
    CriticalChanceBonus,
    CooldownReduction,
    HealingBonus,
    ElementalBonus,
    StatusEffectChanceBonus,
    ResourceRegenBonus,
    MovementSpeedBonus,
    DamageReflection,
    LifeSteal,
    ManaDrain,
    StaminaRegenBonus
}

/// <summary>
/// Advanced class synergy system - abilities that synergize together.
/// </summary>
public class ClassSynergySystem
{
    private Dictionary<string, float> activeSynergies = new Dictionary<string, float>();
    private Dictionary<string, int> synergyTriggerCounts = new Dictionary<string, int>();
    private float totalSynergyBonus = 0f;

    /// <summary>
    /// Trigger a synergy when conditions are met.
    /// </summary>
    public void TriggerSynergy(string synergyName, float bonusValue)
    {
        if (!activeSynergies.ContainsKey(synergyName))
        {
            activeSynergies[synergyName] = 0f;
            synergyTriggerCounts[synergyName] = 0;
        }

        activeSynergies[synergyName] += bonusValue;
        synergyTriggerCounts[synergyName]++;
        totalSynergyBonus += bonusValue;

        Debug.Log($"✨ Synergy triggered: {synergyName} (+{bonusValue:F2} bonus, Total: {totalSynergyBonus:F2})");
    }

    /// <summary>
    /// Get total synergy bonus multiplier.
    /// </summary>
    public float GetSynergyMultiplier()
    {
        return 1f + (totalSynergyBonus / 100f);
    }

    /// <summary>
    /// Reset all active synergies.
    /// </summary>
    public void ResetSynergies()
    {
        activeSynergies.Clear();
        synergyTriggerCounts.Clear();
        totalSynergyBonus = 0f;
    }

    public Dictionary<string, float> GetActiveSynergies() => new Dictionary<string, float>(activeSynergies);
}

/// <summary>
/// Advanced stat scaling system with curve functions.
/// </summary>
public class ClassStatScaler
{
    private float baseHealthScaling = 1.1f;
    private float baseDamageScaling = 1.08f;
    private float baseDefenseScaling = 1.05f;
    private AnimationCurve scalingCurve;

    public ClassStatScaler()
    {
        // Create a smooth curve for stat scaling
        scalingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    /// <summary>
    /// Calculate scaled stat value based on level.
    /// </summary>
    public float GetScaledStat(float baseStat, int level, StatScalingType scalingType)
    {
        float scalingFactor = scalingType switch
        {
            StatScalingType.Health => baseHealthScaling,
            StatScalingType.Damage => baseDamageScaling,
            StatScalingType.Defense => baseDefenseScaling,
            _ => 1f
        };

        float curveValue = scalingCurve.Evaluate(Mathf.Min((level - 1) / 100f, 1f));
        float scaling = Mathf.Pow(scalingFactor, level - 1);

        return baseStat * scaling * (1f + (curveValue * 0.5f));
    }

    /// <summary>
    /// Get all scaled stats for a level.
    /// </summary>
    public Dictionary<string, float> GetAllScaledStats(CharacterStats baseStats, int level)
    {
        return new Dictionary<string, float>
        {
            { "Health", GetScaledStat(baseStats.baseHealth, level, StatScalingType.Health) },
            { "Damage", GetScaledStat(baseStats.basePhysicalDamage, level, StatScalingType.Damage) },
            { "Defense", GetScaledStat(baseStats.baseDefense, level, StatScalingType.Defense) },
            { "MagicalDamage", GetScaledStat(baseStats.baseMagicalDamage, level, StatScalingType.Damage) }
        };
    }
}

public enum StatScalingType
{
    Health,
    Damage,
    Defense,
    MagicalDamage,
    Stamina,
    Mana
}

/// <summary>
/// Advanced class specialization modifier.
/// </summary>
public class SpecializationModifier
{
    public string specializationName;
    public Dictionary<DamageType, float> typeEffectiveness = new Dictionary<DamageType, float>();
    public Dictionary<string, float> statModifiers = new Dictionary<string, float>();
    public List<string> exclusiveAbilities = new List<string>();
    public float bonusExperienceGain = 0f;

    public float GetTypeMultiplier(DamageType type)
    {
        return typeEffectiveness.TryGetValue(type, out var mult) ? mult : 1f;
    }
}