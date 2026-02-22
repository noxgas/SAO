using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced Rogue class with stealth mechanics, poison systems, and assassination abilities.
/// Features: Stealth meter, poison tracking, shadow clones, evasion mastery, assassination combos.
/// </summary>
public class Rogue : ICharacterClass
{
    public string ClassName => "Rogue";
    public string SubclassName => "Shadow Assassin";
    public string ClassDescription => "Stealth and assassination specialist with poison mastery and evasion.";

    public CharacterStats BaseStats { get; private set; }

    public List<string> StartingSkills => new List<string>
    {
        "Backstab",
        "PoisonStrike",
        "ShadowClone",
        "Evasion",
        "Assassination",
        "PoisonCloud",
        "ShadowDance"
    };

    public List<PassiveAbility> PassiveAbilities => new List<PassiveAbility>
    {
        new PassiveAbility
        {
            abilityName = "Shadow Mastery",
            description = "Increase shadow damage by 20%",
            effectValue = 0.2f,
            requiredLevel = 1,
            effectType = PassiveEffectType.DamageBonus
        },
        new PassiveAbility
        {
            abilityName = "Poison Mastery",
            description = "Poison damage increased by 25% and applies faster",
            effectValue = 0.25f,
            requiredLevel = 5,
            effectType = PassiveEffectType.DamageBonus
        },
        new PassiveAbility
        {
            abilityName = "Evasion Mastery",
            description = "Dodge chance increased by 30%",
            effectValue = 0.3f,
            requiredLevel = 10,
            effectType = PassiveEffectType.DefenseBonus
        },
        new PassiveAbility
        {
            abilityName = "Assassination",
            description = "Critical strikes on unaware enemies deal 3x damage",
            effectValue = 3f,
            requiredLevel = 15,
            effectType = PassiveEffectType.CriticalDamageBonus
        }
    };

    public ClassDamageMultipliers DamageMultipliers
    {
        get
        {
            return new ClassDamageMultipliers
            {
                physicalDamageMultiplier = 1.25f,
                criticalDamageMultiplier = 2.5f
            };
        }
    }

    // Rogue-specific systems
    private StealthSystem stealthSystem;
    private PoisonSystem poisonSystem;
    private EvasionSystem evasionSystem;
    private Player player;

    public void Initialize(Player playerRef)
    {
        player = playerRef;

        BaseStats = new CharacterStats
        {
            baseHealth = 75f,
            baseStamina = 130f,
            baseMana = 80f,
            basePhysicalDamage = 22f,
            baseMagicalDamage = 12f,
            baseDefense = 6f,
            baseMoveSpeed = 9f,
            baseCritChance = 0.30f
        };

        // Initialize Rogue systems
        stealthSystem = new StealthSystem();
        poisonSystem = new PoisonSystem();
        evasionSystem = new EvasionSystem();

        Debug.Log($"✓ {ClassName} ({SubclassName}) initialized");
        Debug.Log($"  Stealth System: Active");
        Debug.Log($"  Poison System: Active");
        Debug.Log($"  Evasion System: Active");
    }

    public void OnLevelUp(int newLevel)
    {
        float healthBonus = newLevel * 5f;
        float damageBonus = newLevel * 3f;
        float stealthBonus = newLevel * 0.02f;

        if (BaseStats != null)
        {
            BaseStats.baseHealth += healthBonus;
            BaseStats.basePhysicalDamage += damageBonus;
        }

        if (stealthSystem != null)
            stealthSystem.maxStealthMeter += stealthBonus;

        Debug.Log($"🗡️ Rogue Level Up {newLevel} | HP: +{healthBonus:F0} | DMG: +{damageBonus:F1} | Stealth: +{stealthBonus:F2}");
    }

    public void OnTakeDamage(float damage)
    {
        if (evasionSystem != null && evasionSystem.CanEvade())
        {
            float reducedDamage = damage * 0.3f; // Only take 30% damage when evading
            Debug.Log($"🗡️ Rogue EVADES! {damage:F1} → {reducedDamage:F1}");
        }
        else
        {
            Debug.Log($"🗡️ Rogue took {damage:F1} damage");
        }
    }

    public void OnDealDamage(float damage)
    {
        if (stealthSystem != null && stealthSystem.IsStealthActive)
        {
            damage *= 1.5f; // 50% damage bonus from stealth
            Debug.Log($"🗡️ Rogue backstab from stealth! {damage:F1}");
            stealthSystem.BreakStealth();
        }
        else
        {
            Debug.Log($"🗡️ Rogue dealt {damage:F1} damage");
        }

        if (poisonSystem != null)
            poisonSystem.OnDamageDealt(damage);
    }

    public void ExecuteAbility(string abilityName)
    {
        Debug.Log($"🗡️ Executing: {abilityName}");

        switch (abilityName)
        {
            case "Backstab":
                ExecuteBackstab();
                break;
            case "PoisonStrike":
                ExecutePoisonStrike();
                break;
            case "ShadowClone":
                ExecuteShadowClone();
                break;
            case "Evasion":
                evasionSystem?.ActivateEvasion();
                break;
            case "Assassination":
                ExecuteAssassination();
                break;
            case "PoisonCloud":
                poisonSystem?.CreatePoisonCloud();
                break;
        }
    }

    private void ExecuteBackstab()
    {
        float backstabDamage = BaseStats.PhysicalDamage * 2.5f;
        Debug.Log($"🗡️ Backstab: {backstabDamage:F1} damage");
    }

    private void ExecutePoisonStrike()
    {
        float poisonDamage = BaseStats.PhysicalDamage * 1.5f;
        poisonSystem?.ApplyPoison(poisonDamage, 5f);
        Debug.Log($"🗡️ Poison Strike: {poisonDamage:F1} base + poison DoT");
    }

    private void ExecuteShadowClone()
    {
        Debug.Log($"🗡️ Shadow Clone summoned!");
    }

    private void ExecuteAssassination()
    {
        float assassinationDamage = BaseStats.PhysicalDamage * 4f;
        Debug.Log($"🗡️ Assassination: {assassinationDamage:F1} damage (executing blow)");
    }

    public float GetStatBonus(string statName)
    {
        return statName switch
        {
            "PhysicalDamage" => 0.25f,
            "CritChance" => 0.30f,
            "MoveSpeed" => 0.15f,
            "Evasion" => 0.30f,
            _ => 0f
        };
    }

    public float GetAbilityCooldown(string abilityName) => 0f;

    /// <summary>
    /// Get Rogue statistics.
    /// </summary>
    public string GetRogueStats()
    {
        return $"Stealth: {stealthSystem?.GetStealthPercent():F0}% | " +
               $"Poison Stacks: {poisonSystem?.GetPoisonStacks() ?? 0} | " +
               $"Evasion Ready: {evasionSystem?.IsEvasionReady() ?? false}";
    }
}

/// <summary>
/// Stealth system for Rogue class.
/// </summary>
public class StealthSystem
{
    public float maxStealthMeter = 1f;
    private float currentStealth = 0f;
    public bool IsStealthActive => currentStealth >= 0.5f;

    public void IncreaseStealth(float amount)
    {
        currentStealth = Mathf.Min(currentStealth + amount, maxStealthMeter);
    }

    public void BreakStealth()
    {
        currentStealth = 0f;
        Debug.Log("🗡️ Stealth broken!");
    }

    public float GetStealthPercent()
    {
        return (currentStealth / maxStealthMeter) * 100f;
    }
}

/// <summary>
/// Poison system for Rogue class.
/// </summary>
public class PoisonSystem
{
    private Dictionary<string, float> poisonTargets = new Dictionary<string, float>();
    private int poisonStacks = 0;

    public void ApplyPoison(float damage, float duration)
    {
        poisonStacks++;
        Debug.Log($"🗡️ Poison applied! Stacks: {poisonStacks}");
    }

    public void CreatePoisonCloud()
    {
        poisonStacks += 5;
        Debug.Log($"🗡️ Poison Cloud created! Total stacks: {poisonStacks}");
    }

    public void OnDamageDealt(float damage)
    {
        if (poisonStacks > 0)
        {
            float bonusDamage = damage * (poisonStacks * 0.1f);
            Debug.Log($"🗡️ Poison bonus damage: +{bonusDamage:F1} ({poisonStacks} stacks)");
        }
    }

    public int GetPoisonStacks() => poisonStacks;
}

/// <summary>
/// Evasion system for Rogue class.
/// </summary>
public class EvasionSystem
{
    private float evasionCooldown = 0f;
    private float maxEvasionCooldown = 5f;

    public bool CanEvade() => evasionCooldown <= 0f && Random.value < 0.3f;

    public void ActivateEvasion()
    {
        evasionCooldown = maxEvasionCooldown;
        Debug.Log("🗡️ Evasion activated!");
    }

    public bool IsEvasionReady() => evasionCooldown <= 0f;
}