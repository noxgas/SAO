using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Swordsman Category - Multiple distinct classes
/// 
/// Classes:
/// - Swordsman: Balanced melee fighter
/// - Duelist: Precision strikes with rapier
/// - Vanguard: Heavy greatsword user
/// - Swordmaster: Dual wield mastery (unlocked with dual wield blade)
/// </summary>
public class Swordsman : ICharacterClass
{
    public string ClassName => "Swordsman";
    public string SubclassName { get; private set; }
    public CharacterStats BaseStats { get; private set; }
    public List<string> StartingSkills { get; private set; }

    private SwordsmanClass swordsmanType;

    public Swordsman(SwordsmanClass type)
    {
        swordsmanType = type;
        SubclassName = GetSubclassDisplayName(type);
        InitializeStats();
        InitializeSkills();
    }

    /// <summary>
    /// Get display name for the Swordsman type.
    /// </summary>
    private string GetSubclassDisplayName(SwordsmanClass type)
    {
        return type switch
        {
            SwordsmanClass.Swordsman => "Swordsman",
            SwordsmanClass.Duelist => "Duelist",
            SwordsmanClass.Vanguard => "Vanguard",
            SwordsmanClass.Swordmaster => "Swordmaster",
            _ => "Unknown"
        };
    }

    private void InitializeStats()
    {
        BaseStats = new CharacterStats();
        
        // Base stats for Swordsman category
        BaseStats.baseHealth = 100f;
        BaseStats.baseStamina = 120f;
        BaseStats.baseMana = 0f;
        BaseStats.basePhysicalDamage = 18f;
        BaseStats.baseMagicalDamage = 0f;
        BaseStats.baseDefense = 8f;
        BaseStats.baseMoveSpeed = 7f;
        BaseStats.baseCritChance = 0.15f;

        // Class-specific stat adjustments
        switch (swordsmanType)
        {
            case SwordsmanClass.Swordsman:
                // Base balanced fighter
                // Uses default stats above
                break;

            case SwordsmanClass.Duelist:
                // Precision-focused: High crit, fast attacks, lower defense
                BaseStats.basePhysicalDamage = 14f;      // Lower base damage
                BaseStats.baseCritChance = 0.25f;        // High crit chance
                BaseStats.baseMoveSpeed = 8f;            // Fast
                BaseStats.baseStamina = 125f;            // Slightly higher stamina
                BaseStats.baseDefense = 6f;              // Lower defense
                break;

            case SwordsmanClass.Vanguard:
                // Heavy weapon user: High damage, slow, high defense
                BaseStats.basePhysicalDamage = 22f;      // High damage
                BaseStats.baseStamina = 100f;            // Lower stamina (slower attacks)
                BaseStats.baseDefense = 12f;             // High defense
                BaseStats.baseMoveSpeed = 5.5f;          // Slower movement
                BaseStats.baseCritChance = 0.08f;        // Lower crit
                BaseStats.baseHealth = 120f;             // More health
                break;

            case SwordsmanClass.Swordmaster:
                // Dual wield focused: Balanced, combo-based
                BaseStats.basePhysicalDamage = 17f;      // Balanced damage (split between weapons)
                BaseStats.baseStamina = 140f;            // High stamina for combos
                BaseStats.baseCritChance = 0.12f;        // Lower crit (consistency)
                BaseStats.baseMoveSpeed = 7f;            // Normal movement
                BaseStats.baseDefense = 6f;              // Low defense (risk/reward)
                break;
        }
    }

    private void InitializeSkills()
    {
        StartingSkills = new List<string>();

        switch (swordsmanType)
        {
            case SwordsmanClass.Swordsman:
                // Base balanced skills
                StartingSkills.AddRange(new[]
                {
                    "basic_slash",
                    "power_slash",
                    "whirlwind_slash"
                });
                break;

            case SwordsmanClass.Duelist:
                // Precision-based skills
                StartingSkills.AddRange(new[]
                {
                    "riposte",           // Parry counter
                    "lunge",             // Gap closer
                    "fleche"             // Quick thrust combo
                });
                break;

            case SwordsmanClass.Vanguard:
                // Heavy weapon skills
                StartingSkills.AddRange(new[]
                {
                    "overhead_smash",    // High damage
                    "cleave",            // AoE
                    "armor_break"        // Debuff
                });
                break;

            case SwordsmanClass.Swordmaster:
                // Dual wield skills (combo-focused)
                StartingSkills.AddRange(new[]
                {
                    "dual_slash",        // Basic dual attack
                    "dual_combo",        // Rapid combo
                    "final_strike"       // Combo finisher (5 hits)
                });
                break;
        }
    }

    public void Initialize(Player player)
    {
        // All swordsmen use melee stamina combat
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
            combat.SetCombatStyle(CombatStyle.MeleeStamina);

        // ONLY Swordmaster gets dual wield system
        if (swordsmanType == SwordsmanClass.Swordmaster)
        {
            DualWieldSystem dualWield = player.GetComponent<DualWieldSystem>();
            if (dualWield == null)
                dualWield = player.gameObject.AddComponent<DualWieldSystem>();
            
            dualWield.Initialize();
            Debug.Log("⚡ Swordmaster Dual Wield System activated!");
        }
    }

    public void OnLevelUp(int newLevel)
    {
        BaseStats.SetLevel(newLevel);
    }

    /// <summary>
    /// Check if player is Swordmaster (has dual wield unlocked).
    /// </summary>
    public bool IsSwordmaster()
    {
        return swordsmanType == SwordsmanClass.Swordmaster;
    }
}