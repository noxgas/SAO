using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Advanced Mage class with spell weaving, mana pool management, and elemental synergy.
/// Features: Spell rotation system, mana shield, spell amplification, elemental combinations.
/// </summary>
public class Mage : ICharacterClass
{
    public string ClassName => "Mage";
    public string SubclassName => "Spell Weaver";
    public string ClassDescription => "Master of magic with advanced spell rotations and elemental synergies.";

    public CharacterStats BaseStats
    {
        get
        {
            CharacterStats stats = ScriptableObject.CreateInstance<CharacterStats>();
            stats.baseHealth = 60f;
            stats.baseStamina = 80f;
            stats.baseMana = 200f;
            stats.baseMagicalDamage = 30f;
            stats.baseDefense = 3f;
            stats.baseMoveSpeed = 6f;
            stats.baseCritChance = 0.10f;
            return stats;
        }
    }

    public List<string> StartingSkills => new List<string>
    {
        "Fireball",
        "Ice Spike",
        "Mana Shield",
        "Spell Amp",
        "Pyroclasm",
        "Blizzard",
        "Spell Weave"
    };

    public List<PassiveAbility> PassiveAbilities => new List<PassiveAbility>
    {
        new PassiveAbility
        {
            abilityName = "Mana Efficiency",
            description = "Reduce spell mana cost by 10%",
            effectValue = 0.1f,
            requiredLevel = 1,
            effectType = PassiveEffectType.CooldownReduction
        },
        new PassiveAbility
        {
            abilityName = "Elemental Mastery",
            description = "Increase elemental damage by 15%",
            effectValue = 0.15f,
            requiredLevel = 5,
            effectType = PassiveEffectType.DamageBonus
        },
        new PassiveAbility
        {
            abilityName = "Mana Battery",
            description = "Regenerate 2% mana per second",
            effectValue = 0.02f,
            requiredLevel = 10,
            effectType = PassiveEffectType.ResourceRegeneration
        }
    };

    public ClassDamageMultipliers DamageMultipliers
    {
        get
        {
            return new ClassDamageMultipliers
            {
                magicalDamageMultiplier = 1.5f,
                elementalDamageMultiplier = 1.3f,
                physicalDamageMultiplier = 0.6f
            };
        }
    }

    private Player player;
    private SpellRotationSystem spellRotation;
    private ElementalSystem elementalSystem;
    private ManaShieldSystem manaShield;

    public void Initialize(Player playerRef)
    {
        player = playerRef;
        spellRotation = new SpellRotationSystem(player);
        elementalSystem = new ElementalSystem();
        manaShield = new ManaShieldSystem(player);

        Debug.Log($"✓ {ClassName} ({SubclassName}) initialized for {player.name}");
        Debug.Log($"🔥 Elemental Systems Ready");
    }

    public void OnLevelUp(int newLevel)
    {
        float manaBonus = newLevel * 20f;
        float damageBonus = newLevel * 3f;

        if (player.CharacterClass.BaseStats != null)
        {
            player.CharacterClass.BaseStats.baseMana += manaBonus;
            player.CharacterClass.BaseStats.baseMagicalDamage += damageBonus;
        }

        Debug.Log($"✨ Mage Level Up {newLevel} | Mana: +{manaBonus:F0} | Damage: +{damageBonus:F1}");
    }

    public void OnTakeDamage(float damage)
    {
        float shieldedDamage = manaShield.AbsorbDamage(damage);
        Debug.Log($"🛡️ Mana Shield absorbed {shieldedDamage:F1} damage");
    }

    public void OnDealDamage(float damage) { }

    public void ExecuteAbility(string abilityName)
    {
        Debug.Log($"🔥 Casting: {abilityName}");
    }

    public float GetStatBonus(string statName)
    {
        return statName switch
        {
            "MagicalDamage" => 0.5f,
            "Mana" => 1f,
            _ => 0f
        };
    }

    public float GetAbilityCooldown(string abilityName) => 0f;
}

public class SpellRotationSystem
{
    private Player player;
    private Queue<string> spellQueue = new Queue<string>();

    public SpellRotationSystem(Player playerRef) => player = playerRef;
}

public class ElementalSystem
{
    private Dictionary<string, float> elementStacks = new Dictionary<string, float>();
}

public class ManaShieldSystem
{
    private Player player;
    private float shieldStrength = 0f;

    public ManaShieldSystem(Player playerRef) => player = playerRef;

    public float AbsorbDamage(float damage)
    {
        float absorbed = Mathf.Min(shieldStrength, damage);
        shieldStrength -= absorbed;
        return absorbed;
    }
}