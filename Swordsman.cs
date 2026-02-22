using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced Swordsman class with dual wield mastery, combo mechanics, and state-based progression.
/// Features: Parry system, riposte attacks, dual wield synchronization, bleeding effects.
/// </summary>
public class Swordsman : ICharacterClass
{
    public string ClassName => "Swordsman";
    public string SubclassName => "Dual Blade Master";
    public string ClassDescription => "Master of melee combat with dual wield capabilities and advanced parry mechanics.";

    public CharacterStats BaseStats
    {
        get
        {
            CharacterStats stats = ScriptableObject.CreateInstance<CharacterStats>();
            stats.baseHealth = 100f;
            stats.baseStamina = 120f;
            stats.basePhysicalDamage = 18f;
            stats.baseDefense = 8f;
            stats.baseMoveSpeed = 7.5f;
            stats.baseCritChance = 0.15f;
            return stats;
        }
    }

    public List<string> StartingSkills => new List<string>
    {
        "Slash",
        "Power Attack",
        "Dual Wield Combo",
        "Parry",
        "Spinning Slash",
        "Riposte",
        "Blade Dance"
    };

    public List<PassiveAbility> PassiveAbilities => new List<PassiveAbility>
    {
        new PassiveAbility
        {
            abilityName = "Weapon Mastery",
            description = "Increase physical damage by 10%",
            effectValue = 0.1f,
            requiredLevel = 1,
            effectType = PassiveEffectType.DamageBonus
        },
        new PassiveAbility
        {
            abilityName = "Dual Wield Synchronization",
            description = "Increase critical chance by 5% per dual wield sword",
            effectValue = 0.05f,
            requiredLevel = 5,
            effectType = PassiveEffectType.CritChanceBonus
        },
        new PassiveAbility
        {
            abilityName = "Bleeding Edge",
            description = "Physical attacks apply bleeding effect",
            effectValue = 0.02f,
            requiredLevel = 10,
            effectType = PassiveEffectType.Custom
        },
        new PassiveAbility
        {
            abilityName = "Parry Master",
            description = "Parry cooldown reduced by 30%",
            effectValue = 0.3f,
            requiredLevel = 15,
            effectType = PassiveEffectType.CooldownReduction
        }
    };

    public ClassDamageMultipliers DamageMultipliers
    {
        get
        {
            return new ClassDamageMultipliers
            {
                physicalDamageMultiplier = 1.25f,
                criticalDamageMultiplier = 1.75f,
                magicalDamageMultiplier = 0.8f
            };
        }
    }

    private Player player;
    private DualWieldSystem dualWieldSystem;
    private ComboSystem comboSystem;
    private StateManager stateManager;
    private EffectSystem effectSystem;
    private Dictionary<string, AbilityData> abilities = new Dictionary<string, AbilityData>();

    // State tracking
    private bool isParrying = false;
    private float parryCounter = 0f;
    private float bleedingDamageAccumulated = 0f;
    private int consecutiveParries = 0;
    private float lastParryTime = 0f;

    [System.Serializable]
    private class AbilityData
    {
        public string name;
        public float cooldown;
        public float currentCooldown;
        public float manaCost;
        public float staminaCost;
        public float damage;
    }

    public void Initialize(Player playerRef)
    {
        player = playerRef;

        dualWieldSystem = player.GetComponent<DualWieldSystem>();
        if (dualWieldSystem != null)
            dualWieldSystem.Initialize(player);

        comboSystem = player.GetComponent<ComboSystem>();
        if (comboSystem != null)
            comboSystem.SetCurrentFloor(1);

        stateManager = new StateManager(player);
        effectSystem = new EffectSystem();

        InitializeAbilities();

        Debug.Log($"✓ {ClassName} ({SubclassName}) initialized for {player.name}");
        Debug.Log($"Starting Skills: {string.Join(", ", StartingSkills)}");
        Debug.Log($"Passive Abilities: {string.Join(", ", PassiveAbilities.ConvertAll(p => p.abilityName))}");
    }

    private void InitializeAbilities()
    {
        abilities["Slash"] = new AbilityData { name = "Slash", cooldown = 0.5f, manaCost = 0, staminaCost = 20, damage = 10f };
        abilities["Power Attack"] = new AbilityData { name = "Power Attack", cooldown = 2f, manaCost = 0, staminaCost = 40, damage = 25f };
        abilities["Parry"] = new AbilityData { name = "Parry", cooldown = 1f, manaCost = 0, staminaCost = 15, damage = 0 };
        abilities["Riposte"] = new AbilityData { name = "Riposte", cooldown = 3f, manaCost = 0, staminaCost = 35, damage = 35f };
        abilities["Blade Dance"] = new AbilityData { name = "Blade Dance", cooldown = 4f, manaCost = 20, staminaCost = 50, damage = 40f };
    }

    public void OnLevelUp(int newLevel)
    {
        float healthBonus = newLevel * 10f;
        float damageBonus = newLevel * 2f;
        float defenseBonus = newLevel * 1f;

        if (player.CharacterClass.BaseStats != null)
        {
            player.CharacterClass.BaseStats.baseHealth += healthBonus;
            player.CharacterClass.BaseStats.basePhysicalDamage += damageBonus;
            player.CharacterClass.BaseStats.baseDefense += defenseBonus;
        }

        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║  ⚔️ SWORDSMAN LEVEL UP ⚔️              ║");
        Debug.Log($"║ New Level: {newLevel,26} ║");
        Debug.Log($"║ Health Bonus: +{healthBonus,20:F0} ║");
        Debug.Log($"║ Damage Bonus: +{damageBonus,20:F1} ║");
        Debug.Log($"║ Defense Bonus: +{defenseBonus,19:F1} ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");

        // Unlock new abilities
        if (newLevel == 5)
            Debug.Log("🔓 New Ability Unlocked: Riposte");
        if (newLevel == 10)
            Debug.Log("🔓 New Ability Unlocked: Blade Dance");
    }

    public void OnTakeDamage(float damage)
    {
        if (isParrying)
        {
            float parryMitigation = damage * 0.5f;
            damage -= parryMitigation;
            consecutiveParries++;
            Debug.Log($"✓ Parried {parryMitigation:F1} damage! Parry Counter: {consecutiveParries}");
        }
    }

    public void OnDealDamage(float damage)
    {
        if (comboSystem != null)
        {
            comboSystem.RegisterHit(damage);
        }

        // Apply bleeding if ability is unlocked
        if (player.Level >= 10)
        {
            float bleedDamage = damage * 0.2f;
            effectSystem.ApplyEffect("Bleeding", bleedDamage, 3f);
            bleedingDamageAccumulated += bleedDamage;
        }
    }

    public void ExecuteAbility(string abilityName)
    {
        if (!abilities.TryGetValue(abilityName, out var ability))
        {
            Debug.LogWarning($"❌ Ability {abilityName} not found!");
            return;
        }

        if (ability.currentCooldown > 0)
        {
            Debug.LogWarning($"⏱️ {abilityName} on cooldown for {ability.currentCooldown:F2}s");
            return;
        }

        Debug.Log($"⚡ Executing: {abilityName} | DMG: {ability.damage:F1} | Stamina Cost: {ability.staminaCost:F0}");

        ability.currentCooldown = ability.cooldown;

        switch (abilityName)
        {
            case "Parry":
                ExecuteParry();
                break;
            case "Riposte":
                ExecuteRiposte(ability.damage);
                break;
            case "Blade Dance":
                ExecuteBladeDance(ability.damage);
                break;
        }
    }

    private void ExecuteParry()
    {
        isParrying = true;
        lastParryTime = Time.time;
        Debug.Log("🛡️ PARRY STANCE ACTIVATED!");
    }

    private void ExecuteRiposte(float damage)
    {
        if (consecutiveParries > 0)
        {
            float riposteDamage = damage * (1f + (consecutiveParries * 0.25f));
            Debug.Log($"⚡ RIPOSTE! Damage: {riposteDamage:F1} (Parry Bonus: {(consecutiveParries * 25):F0}%)");
            consecutiveParries = 0;
        }
    }

    private void ExecuteBladeDance(float damage)
    {
        if (dualWieldSystem != null && dualWieldSystem.IsDualWieldActive())
        {
            float dualWieldDamage = damage * 1.5f;
            Debug.Log($"✨ BLADE DANCE (Dual Wield)! Damage: {dualWieldDamage:F1}");
        }
    }

    public float GetStatBonus(string statName)
    {
        return statName switch
        {
            "PhysicalDamage" => 0.25f,
            "CritChance" => 0.15f,
            "Stamina" => 0.2f,
            _ => 0f
        };
    }

    public float GetAbilityCooldown(string abilityName)
    {
        return abilities.TryGetValue(abilityName, out var ability) ? ability.currentCooldown : 0f;
    }

    public string GetClassStats()
    {
        return $"Class: {ClassName} | Subclass: {SubclassName} | Parries: {consecutiveParries} | Bleed DMG: {bleedingDamageAccumulated:F1}";
    }
}

/// <summary>
/// State manager for class-specific states.
/// </summary>
public class StateManager
{
    private Player player;
    private Dictionary<string, bool> states = new Dictionary<string, bool>();

    public StateManager(Player playerRef)
    {
        player = playerRef;
    }

    public void SetState(string stateName, bool value)
    {
        states[stateName] = value;
    }

    public bool GetState(string stateName)
    {
        return states.TryGetValue(stateName, out var value) && value;
    }
}

/// <summary>
/// Effect system for status effects like bleeding, poison, etc.
/// </summary>
public class EffectSystem
{
    private Dictionary<string, ActiveEffect> activeEffects = new Dictionary<string, ActiveEffect>();

    [System.Serializable]
    private class ActiveEffect
    {
        public string effectName;
        public float damage;
        public float duration;
        public float startTime;
    }

    public void ApplyEffect(string effectName, float damage, float duration)
    {
        activeEffects[effectName] = new ActiveEffect
        {
            effectName = effectName,
            damage = damage,
            duration = duration,
            startTime = Time.time
        };

        Debug.Log($"💀 Applied effect: {effectName} (DMG: {damage:F1}, Duration: {duration:F1}s)");
    }

    public float GetActiveDamage()
    {
        float totalDamage = 0f;
        var expiredEffects = new List<string>();

        foreach (var kvp in activeEffects)
        {
            float elapsed = Time.time - kvp.Value.startTime;
            if (elapsed < kvp.Value.duration)
            {
                totalDamage += kvp.Value.damage;
            }
            else
            {
                expiredEffects.Add(kvp.Key);
            }
        }

        foreach (var effect in expiredEffects)
        {
            activeEffects.Remove(effect);
        }

        return totalDamage;
    }
}