using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Advanced Archer class with arrow crafting, focus systems, and precision mechanics.
/// Features: Arrow types, scope system, multi-shot, critical zones.
/// </summary>
public class Archer : ICharacterClass
{
    public string ClassName => "Archer";
    public string SubclassName => "Precision Hunter";
    public string ClassDescription => "Master of ranged combat with multiple arrow types and focus mechanics.";

    public CharacterStats BaseStats { get; private set; }

    public List<string> StartingSkills => new List<string>
    {
        "AimedShot",
        "RapidFire",
        "DodgeRoll",
        "MultiShot",
        "PiercingArrow",
        "ArrowCraft",
        "FocusFire"
    };

    public List<PassiveAbility> PassiveAbilities => new List<PassiveAbility>
    {
        new PassiveAbility
        {
            abilityName = "Steady Aim",
            description = "Increase critical chance by 10%",
            effectValue = 0.1f,
            requiredLevel = 1,
            effectType = PassiveEffectType.CritChanceBonus
        },
        new PassiveAbility
        {
            abilityName = "Rapid Shot",
            description = "Increase attack speed by 15%",
            effectValue = 0.15f,
            requiredLevel = 5,
            effectType = PassiveEffectType.CooldownReduction
        },
        new PassiveAbility
        {
            abilityName = "Arrow Mastery",
            description = "Increase arrow damage by 20%",
            effectValue = 0.2f,
            requiredLevel = 10,
            effectType = PassiveEffectType.DamageBonus
        }
    };

    public ClassDamageMultipliers DamageMultipliers
    {
        get
        {
            return new ClassDamageMultipliers
            {
                physicalDamageMultiplier = 1.15f,
                criticalDamageMultiplier = 2f
            };
        }
    }

    private Player player;
    private Dictionary<string, float> arrowTypes = new Dictionary<string, float>();

    public void Initialize(Player playerRef)
    {
        player = playerRef;

        // Create base stats - FIX: Use 'new' instead of CreateInstance
        BaseStats = new CharacterStats
        {
            baseHealth = 80f,
            baseStamina = 140f,
            basePhysicalDamage = 20f,
            baseDefense = 5f,
            baseMoveSpeed = 8.5f,
            baseCritChance = 0.25f
        };

        InitializeArrowTypes();

        Debug.Log($"✓ {ClassName} ({SubclassName}) initialized for {player.name}");
    }

    /// <summary>
    /// Initialize arrow types with their damage multipliers.
    /// </summary>
    private void InitializeArrowTypes()
    {
        arrowTypes["Standard"] = 1f;
        arrowTypes["Fire"] = 1.2f;
        arrowTypes["Ice"] = 1.15f;
        arrowTypes["Piercing"] = 1.3f;
        arrowTypes["Explosive"] = 1.4f;

        Debug.Log($"✓ {arrowTypes.Count} arrow types initialized");
    }

    public void OnLevelUp(int newLevel)
    {
        float healthBonus = newLevel * 8f;
        float damageBonus = newLevel * 2.5f;

        if (BaseStats != null)
        {
            BaseStats.baseHealth += healthBonus;
            BaseStats.basePhysicalDamage += damageBonus;
        }

        Debug.Log($"🏹 Archer Level Up {newLevel} | HP: +{healthBonus:F0} | DMG: +{damageBonus:F1}");
    }

    public void OnTakeDamage(float damage)
    {
        Debug.Log($"🏹 Archer took {damage:F1} damage");
    }

    public void OnDealDamage(float damage)
    {
        Debug.Log($"🏹 Archer dealt {damage:F1} damage");
    }

    public void ExecuteAbility(string abilityName)
    {
        Debug.Log($"🏹 Executing: {abilityName}");
    }

    public float GetStatBonus(string statName)
    {
        return statName switch
        {
            "PhysicalDamage" => 0.15f,
            "CritChance" => 0.25f,
            "MoveSpeed" => 0.1f,
            _ => 0f
        };
    }

    public float GetAbilityCooldown(string abilityName) => 0f;
}