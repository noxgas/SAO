using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized database for all skills.
/// Can be expanded to load from JSON or ScriptableObjects.
/// </summary>
public static class SkillDatabase
{
    private static Dictionary<string, Skill> skillDatabase = new Dictionary<string, Skill>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;

        // Swordsman Skills
        skillDatabase["basic_slash"] = new Skill
        {
            skillId = "basic_slash",
            skillName = "Basic Slash",
            description = "A simple sword slash",
            staminaCost = 10f,
            baseDamage = 15f,
            cooldownSeconds = 0.5f,
            damageType = DamageType.Physical,
            animationDuration = 0.6f
        };

        skillDatabase["dual_combo"] = new Skill
        {
            skillId = "dual_combo",
            skillName = "Dual Combo",
            description = "Rapid dual-wield combo",
            staminaCost = 25f,
            baseDamage = 35f,
            cooldownSeconds = 2f,
            damageType = DamageType.Physical,
            animationDuration = 1.2f
        };

        skillDatabase["whirlwind"] = new Skill
        {
            skillId = "whirlwind",
            skillName = "Whirlwind",
            description = "Spin attack hitting all enemies",
            staminaCost = 40f,
            baseDamage = 30f,
            cooldownSeconds = 3f,
            damageType = DamageType.Physical,
            animationDuration = 1.5f
        };

        // Tank Skills
        skillDatabase["shield_bash"] = new Skill
        {
            skillId = "shield_bash",
            skillName = "Shield Bash",
            description = "Stun enemy with shield",
            staminaCost = 20f,
            baseDamage = 10f,
            cooldownSeconds = 2f,
            damageType = DamageType.Physical,
            animationDuration = 0.8f
        };

        skillDatabase["taunt"] = new Skill
        {
            skillId = "taunt",
            skillName = "Taunt",
            description = "Force enemy to attack you",
            staminaCost = 15f,
            baseDamage = 0f,
            cooldownSeconds = 3f,
            damageType = DamageType.Physical,
            animationDuration = 0.5f
        };

        // Mage Skills
        skillDatabase["fireball"] = new Skill
        {
            skillId = "fireball",
            skillName = "Fireball",
            description = "Launch a ball of fire",
            manaCost = 30f,
            baseDamage = 40f,
            cooldownSeconds = 2f,
            damageType = DamageType.Fire,
            animationDuration = 1.0f
        };

        skillDatabase["heal"] = new Skill
        {
            skillId = "heal",
            skillName = "Heal",
            description = "Restore health to target",
            manaCost = 25f,
            baseDamage = 0f,
            cooldownSeconds = 2f,
            damageType = DamageType.Physical,
            animationDuration = 1.0f
        };

        initialized = true;
    }

    public static Skill GetSkill(string skillId)
    {
        Initialize();
        return skillDatabase.ContainsKey(skillId) ? skillDatabase[skillId].Clone() : null;
    }

    public static List<Skill> GetAllSkills()
    {
        Initialize();
        return new List<Skill>(skillDatabase.Values);
    }
}