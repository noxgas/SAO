using System.Collections.Generic;

/// <summary>
/// Centralized database for all skills in the game.
/// Manages skill creation, loading, and retrieval.
/// </summary>
public static class SkillDatabase
{
    private static Dictionary<string, Skill> skillDatabase = new Dictionary<string, Skill>();
    private static bool isInitialized = false;

    /// <summary>
    /// Initialize the skill database with all available skills.
    /// </summary>
    public static void Initialize()
    {
        if (isInitialized)
            return;

        // Swordsman Skills
        CreateSkill("Slash", "Slash", "Basic slash attack", SkillType.Active, DamageType.Physical, 10f, 0.5f, 0, 10);
        CreateSkill("PowerAttack", "Power Attack", "Heavy attack with increased damage", SkillType.Active, DamageType.Physical, 25f, 2f, 0, 40);
        CreateSkill("DualWieldCombo", "Dual Wield Combo", "Perform combo with dual wield weapons", SkillType.Active, DamageType.Physical, 35f, 3f, 0, 50);
        CreateSkill("Parry", "Parry", "Parry incoming attacks", SkillType.Active, DamageType.Physical, 0f, 1f, 15, 0);
        CreateSkill("SpinningSlash", "Spinning Slash", "Spin and attack all enemies around", SkillType.Active, DamageType.Physical, 30f, 2.5f, 0, 45);

        // Mage Skills
        CreateSkill("Fireball", "Fireball", "Launch a fireball at enemies", SkillType.Active, DamageType.Fire, 30f, 2f, 30, 0);
        CreateSkill("IceSpike", "Ice Spike", "Freeze enemies with ice spikes", SkillType.Active, DamageType.Ice, 25f, 1.5f, 25, 0);
        CreateSkill("ManaShield", "Mana Shield", "Create a shield using mana", SkillType.Active, DamageType.Magical, 0f, 3f, 50, 0);
        CreateSkill("Pyroclasm", "Pyroclasm", "Massive fire explosion", SkillType.Ultimate, DamageType.Fire, 50f, 5f, 75, 0);
        CreateSkill("Blizzard", "Blizzard", "Freeze all enemies in an area", SkillType.Ultimate, DamageType.Ice, 45f, 4.5f, 70, 0);

        // Archer Skills
        CreateSkill("AimedShot", "Aimed Shot", "Precise arrow shot with high damage", SkillType.Active, DamageType.Physical, 20f, 1.5f, 0, 25);
        CreateSkill("RapidFire", "Rapid Fire", "Shoot multiple arrows quickly", SkillType.Active, DamageType.Physical, 15f, 1f, 0, 30);
        CreateSkill("DodgeRoll", "Dodge Roll", "Roll to avoid incoming attacks", SkillType.Active, DamageType.Physical, 0f, 1f, 0, 15);
        CreateSkill("MultiShot", "Multi-Shot", "Shoot arrows in multiple directions", SkillType.Active, DamageType.Physical, 40f, 3f, 0, 50);
        CreateSkill("PiercingArrow", "Piercing Arrow", "Arrow that pierces through enemies", SkillType.Active, DamageType.Physical, 35f, 2.5f, 0, 45);

        // Paladin Skills
        CreateSkill("HolyStrike", "Holy Strike", "Strike with holy power", SkillType.Active, DamageType.Holy, 20f, 1.5f, 20, 0);
        CreateSkill("DivineShield", "Divine Shield", "Create a divine protective shield", SkillType.Active, DamageType.Holy, 0f, 2f, 40, 0);
        CreateSkill("Heal", "Heal", "Restore health to self or ally", SkillType.Active, DamageType.Holy, 0f, 2f, 30, 0);
        CreateSkill("HolyAura", "Holy Aura", "Create an aura of protection", SkillType.Passive, DamageType.Holy, 0f, 0f, 50, 0);
        CreateSkill("Consecration", "Consecration", "Bless the ground with holy power", SkillType.Active, DamageType.Holy, 25f, 2f, 45, 0);

        // Rogue Skills
        CreateSkill("Backstab", "Backstab", "Strike from behind for massive damage", SkillType.Active, DamageType.Physical, 40f, 2f, 0, 35);
        CreateSkill("ShadowClone", "Shadow Clone", "Create a shadow clone to aid in combat", SkillType.Active, DamageType.Shadow, 0f, 3f, 40, 0);
        CreateSkill("PoisonStrike", "Poison Strike", "Strike with poisoned weapon", SkillType.Active, DamageType.Poison, 20f, 1.5f, 0, 25);
        CreateSkill("Evasion", "Evasion", "Evade incoming attacks", SkillType.Passive, DamageType.Physical, 0f, 0f, 0, 0);
        CreateSkill("Assassination", "Assassination", "Execute a finishing blow", SkillType.Ultimate, DamageType.Physical, 60f, 4f, 0, 50);

        isInitialized = true;
        Debug.Log($"✓ Skill Database initialized with {skillDatabase.Count} skills");
    }

    /// <summary>
    /// Create and register a skill.
    /// </summary>
    private static void CreateSkill(string id, string name, string desc, SkillType type, DamageType damageType, float baseDmg, float cooldown, float manaCost, float staminaCost)
    {
        var skill = new Skill();
        // Reflection to set private fields
        var skillIdField = typeof(Skill).GetField("skillId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var skillNameField = typeof(Skill).GetField("skillName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var descriptionField = typeof(Skill).GetField("description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var typeField = typeof(Skill).GetField("skillType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var damageTypeField = typeof(Skill).GetField("damageType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var baseDamageField = typeof(Skill).GetField("baseDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var cooldownField = typeof(Skill).GetField("cooldownTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var manaField = typeof(Skill).GetField("manaCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var staminaField = typeof(Skill).GetField("staminaCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        skillIdField?.SetValue(skill, id);
        skillNameField?.SetValue(skill, name);
        descriptionField?.SetValue(skill, desc);
        typeField?.SetValue(skill, type);
        damageTypeField?.SetValue(skill, damageType);
        baseDamageField?.SetValue(skill, baseDmg);
        cooldownField?.SetValue(skill, cooldown);
        manaField?.SetValue(skill, manaCost);
        staminaField?.SetValue(skill, staminaCost);

        skillDatabase[id] = skill;
    }

    /// <summary>
    /// Get a skill by ID.
    /// </summary>
    public static Skill GetSkill(string skillId)
    {
        if (!isInitialized)
            Initialize();

        return skillDatabase.TryGetValue(skillId, out var skill) ? skill : null;
    }

    /// <summary>
    /// Get all skills.
    /// </summary>
    public static Dictionary<string, Skill> GetAllSkills()
    {
        if (!isInitialized)
            Initialize();

        return new Dictionary<string, Skill>(skillDatabase);
    }

    /// <summary>
    /// Get all skills by type.
    /// </summary>
    public static List<Skill> GetSkillsByType(SkillType type)
    {
        if (!isInitialized)
            Initialize();

        var skills = new List<Skill>();
        foreach (var skill in skillDatabase.Values)
        {
            if (skill.Type == type)
                skills.Add(skill);
        }
        return skills;
    }
}