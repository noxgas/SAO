using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced Paladin class with holy mechanics, blessing systems, and divine protection.
/// Features: Divine shield, holy aura, blessing buffs, consecration zones, group healing.
/// </summary>
public class Paladin : ICharacterClass
{
    public string ClassName => "Paladin";
    public string SubclassName => "Holy Sentinel";
    public string ClassDescription => "Holy warrior with divine protection, group support, and consecration mastery.";

    public CharacterStats BaseStats { get; private set; }

    public List<string> StartingSkills => new List<string>
    {
        "HolyStrike",
        "DivineShield",
        "Heal",
        "HolyAura",
        "Consecration",
        "DivineIntervention",
        "Blessing"
    };

    public List<PassiveAbility> PassiveAbilities => new List<PassiveAbility>
    {
        new PassiveAbility
        {
            abilityName = "Holy Armor",
            description = "Increase defense by 20%",
            effectValue = 0.2f,
            requiredLevel = 1,
            effectType = PassiveEffectType.DefenseBonus
        },
        new PassiveAbility
        {
            abilityName = "Divine Blessing",
            description = "Healing effectiveness increased by 25%",
            effectValue = 0.25f,
            requiredLevel = 5,
            effectType = PassiveEffectType.HealingBonus
        },
        new PassiveAbility
        {
            abilityName = "Consecration Mastery",
            description = "Consecrated ground damage increased by 30%",
            effectValue = 0.3f,
            requiredLevel = 10,
            effectType = PassiveEffectType.DamageBonus
        },
        new PassiveAbility
        {
            abilityName = "Holy Shield",
            description = "Reflect 20% of blocked damage back to attackers",
            effectValue = 0.2f,
            requiredLevel = 15,
            effectType = PassiveEffectType.DefenseBonus
        },
        new PassiveAbility
        {
            abilityName = "Divine Wrath",
            description = "Holy damage increased by 35%",
            effectValue = 0.35f,
            requiredLevel = 20,
            effectType = PassiveEffectType.DamageBonus
        }
    };

    public ClassDamageMultipliers DamageMultipliers
    {
        get
        {
            return new ClassDamageMultipliers
            {
                physicalDamageMultiplier = 1.1f,
                magicalDamageMultiplier = 1.3f
            };
        }
    }

    // Paladin-specific systems
    private HolyShieldSystem shieldSystem;
    private BlessingSystem blessingSystem;
    private ConsecrationSystem consecrationSystem;
    private HealingSystem healingSystem;
    private Player player;

    public void Initialize(Player playerRef)
    {
        player = playerRef;

        BaseStats = new CharacterStats
        {
            baseHealth = 120f,
            baseStamina = 110f,
            baseMana = 100f,
            basePhysicalDamage = 16f,
            baseMagicalDamage = 18f,
            baseDefense = 12f,
            baseMoveSpeed = 6.5f,
            baseCritChance = 0.12f
        };

        // Initialize Paladin systems
        shieldSystem = new HolyShieldSystem();
        blessingSystem = new BlessingSystem();
        consecrationSystem = new ConsecrationSystem();
        healingSystem = new HealingSystem();

        Debug.Log($"✓ {ClassName} ({SubclassName}) initialized");
        Debug.Log($"  Holy Shield System: Active");
        Debug.Log($"  Blessing System: Active");
        Debug.Log($"  Consecration System: Active");
        Debug.Log($"  Healing System: Active");
    }

    public void OnLevelUp(int newLevel)
    {
        float healthBonus = newLevel * 12f;
        float manaBonus = newLevel * 5f;
        float defenseBonus = newLevel * 1.5f;
        float healingBonus = newLevel * 2f;

        if (BaseStats != null)
        {
            BaseStats.baseHealth += healthBonus;
            BaseStats.baseMana += manaBonus;
            BaseStats.baseDefense += defenseBonus;
        }

        if (healingSystem != null)
            healingSystem.IncreasePower(healingBonus);

        Debug.Log($"⚖️ Paladin Level Up {newLevel} | HP: +{healthBonus:F0} | Mana: +{manaBonus:F0} | DEF: +{defenseBonus:F1}");
    }

    public void OnTakeDamage(float damage)
    {
        float mitigatedDamage = damage;

        // Holy Shield mitigation
        if (shieldSystem != null && shieldSystem.IsShieldActive)
        {
            float shieldMitigation = damage * 0.4f; // Block 40%
            mitigatedDamage = damage - shieldMitigation;
            float reflectedDamage = shieldMitigation * 0.5f;
            Debug.Log($"⚖️ Holy Shield blocks {shieldMitigation:F1} and reflects {reflectedDamage:F1}!");
        }

        // Blessing protection
        if (blessingSystem != null && blessingSystem.HasProtectionBlessing)
        {
            float blessingMitigation = mitigatedDamage * 0.15f; // Additional 15% reduction
            mitigatedDamage -= blessingMitigation;
            Debug.Log($"⚖️ Blessing reduces damage by {blessingMitigation:F1}");
        }

        Debug.Log($"⚖️ Paladin took {mitigatedDamage:F1} damage (reduced from {damage:F1})");
    }

    public void OnDealDamage(float damage)
    {
        float holyDamage = damage * 1.3f; // Holy damage amplification

        if (consecrationSystem != null && consecrationSystem.IsConsecrated)
        {
            holyDamage *= 1.2f; // 20% bonus on consecrated ground
            Debug.Log($"⚖️ Holy Strike on consecrated ground! {holyDamage:F1} damage");
        }
        else
        {
            Debug.Log($"⚖️ Paladin dealt {holyDamage:F1} holy damage");
        }
    }

    public void ExecuteAbility(string abilityName)
    {
        Debug.Log($"⚖️ Executing: {abilityName}");

        switch (abilityName)
        {
            case "HolyStrike":
                ExecuteHolyStrike();
                break;
            case "DivineShield":
                shieldSystem?.ActivateShield();
                break;
            case "Heal":
                ExecuteHeal();
                break;
            case "HolyAura":
                blessingSystem?.ActivateHolyAura();
                break;
            case "Consecration":
                consecrationSystem?.CreateConsecration();
                break;
            case "DivineIntervention":
                shieldSystem?.ActivateDivineIntervention();
                break;
            case "Blessing":
                blessingSystem?.ApplyBlessing();
                break;
        }
    }

    private void ExecuteHolyStrike()
    {
        float holyStrikeDamage = BaseStats.MagicalDamage * 1.8f;
        Debug.Log($"⚖️ Holy Strike: {holyStrikeDamage:F1} damage");
    }

    private void ExecuteHeal()
    {
        float healAmount = BaseStats.MagicalDamage * 2f;
        healingSystem?.HealTarget(healAmount);
        Debug.Log($"⚖️ Healing applied: {healAmount:F1} HP restored");
    }

    public float GetStatBonus(string statName)
    {
        return statName switch
        {
            "PhysicalDamage" => 0.1f,
            "MagicalDamage" => 0.3f,
            "Defense" => 0.2f,
            "Health" => 0.15f,
            _ => 0f
        };
    }

    public float GetAbilityCooldown(string abilityName) => 0f;

    /// <summary>
    /// Get Paladin statistics.
    /// </summary>
    public string GetPaladinStats()
    {
        return $"Shield: {(shieldSystem?.IsShieldActive ? "Active" : "Inactive")} | " +
               $"Aura: {(blessingSystem?.HasHolyAura ? "Active" : "Inactive")} | " +
               $"Consecrated: {(consecrationSystem?.IsConsecrated ? "Yes" : "No")} | " +
               $"Healing Power: {healingSystem?.GetHealingPower():F1}";
    }
}

/// <summary>
/// Holy Shield system for Paladin class.
/// </summary>
public class HolyShieldSystem
{
    private float shieldHealth = 100f;
    private float maxShieldHealth = 100f;
    public bool IsShieldActive => shieldHealth > 0;
    private float divineInterventionCooldown = 0f;
    private float maxDivineInterventionCooldown = 30f;

    public void ActivateShield()
    {
        shieldHealth = maxShieldHealth;
        Debug.Log("⚖️ Divine Shield activated!");
    }

    public void TakeDamage(float damage)
    {
        shieldHealth -= damage;
        if (shieldHealth <= 0)
        {
            Debug.Log("⚖️ Divine Shield broken!");
        }
    }

    public void ActivateDivineIntervention()
    {
        if (divineInterventionCooldown <= 0)
        {
            ActivateShield();
            divineInterventionCooldown = maxDivineInterventionCooldown;
            Debug.Log("⚖️ Divine Intervention activated! Shield restored!");
        }
    }

    public float GetShieldPercent()
    {
        return (shieldHealth / maxShieldHealth) * 100f;
    }
}

/// <summary>
/// Blessing system for Paladin class.
/// </summary>
public class BlessingSystem
{
    private Dictionary<string, float> activeBlessings = new Dictionary<string, float>();
    public bool HasProtectionBlessing => activeBlessings.ContainsKey("Protection");
    public bool HasHolyAura => activeBlessings.ContainsKey("HolyAura");

    public void ApplyBlessing()
    {
        activeBlessings["Protection"] = 60f; // 60 second duration
        Debug.Log("⚖️ Blessing of Protection applied!");
    }

    public void ActivateHolyAura()
    {
        activeBlessings["HolyAura"] = 120f; // 120 second duration
        Debug.Log("⚖️ Holy Aura activated! +20% damage to nearby allies!");
    }

    public void RemoveBlessing(string blessingName)
    {
        if (activeBlessings.ContainsKey(blessingName))
        {
            activeBlessings.Remove(blessingName);
            Debug.Log($"⚖️ {blessingName} expired");
        }
    }

    public int GetActiveBlessingCount() => activeBlessings.Count;
}

/// <summary>
/// Consecration system for Paladin class.
/// </summary>
public class ConsecrationSystem
{
    private float consecrationDuration = 0f;
    private float maxConsecrationDuration = 30f;
    public bool IsConsecrated => consecrationDuration > 0;
    private int consecrationLevel = 1;

    public void CreateConsecration()
    {
        consecrationDuration = maxConsecrationDuration;
        consecrationLevel = Mathf.Min(consecrationLevel + 1, 5);
        Debug.Log($"⚖️ Consecration created! Level {consecrationLevel}");
    }

    public float GetConsecrationDamageBonus()
    {
        return 0.1f * consecrationLevel; // 10% bonus per level
    }

    public float GetRemainingDuration()
    {
        return consecrationDuration;
    }

    public int GetConsecrationLevel() => consecrationLevel;
}

/// <summary>
/// Healing system for Paladin class.
/// </summary>
public class HealingSystem
{
    private float healingPower = 20f;
    private float totalHealing = 0f;
    private int healsPerformed = 0;

    public void HealTarget(float healAmount)
    {
        totalHealing += healAmount;
        healsPerformed++;
        Debug.Log($"⚖️ Healed for {healAmount:F1} (Total: {totalHealing:F1})");
    }

    public void IncreasePower(float bonus)
    {
        healingPower += bonus;
        Debug.Log($"⚖️ Healing power increased: {healingPower:F1}");
    }

    public float GetHealingPower() => healingPower;

    public float GetTotalHealing() => totalHealing;

    public int GetHealsPerformed() => healsPerformed;
}