using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tank Category - Multiple distinct classes
/// 
/// Classes:
/// - Guardian: Balanced tank
/// - Protector: Shield specialist (sword and shield)
/// </summary>
public class Tank : ICharacterClass
{
    public string ClassName => "Tank";
    public string SubclassName { get; private set; }
    public CharacterStats BaseStats { get; private set; }
    public List<string> StartingSkills { get; private set; }

    private TankClass tankType;

    public Tank(TankClass type)
    {
        tankType = type;
        SubclassName = GetSubclassDisplayName(type);
        InitializeStats();
        InitializeSkills();
    }

    /// <summary>
    /// Get display name for the Tank type.
    /// </summary>
    private string GetSubclassDisplayName(TankClass type)
    {
        return type switch
        {
            TankClass.Guardian => "Guardian",
            TankClass.Protector => "Protector",
            _ => "Unknown"
        };
    }

    private void InitializeStats()
    {
        BaseStats = new CharacterStats();

        // Base stats for Tank category
        BaseStats.baseHealth = 150f;
        BaseStats.baseStamina = 90f;
        BaseStats.baseMana = 0f;
        BaseStats.basePhysicalDamage = 12f;
        BaseStats.baseMagicalDamage = 0f;
        BaseStats.baseDefense = 20f;
        BaseStats.baseMoveSpeed = 5f;
        BaseStats.baseCritChance = 0.05f;

        // Tank-specific stat adjustments
        switch (tankType)
        {
            case TankClass.Guardian:
                // Balanced tank - uses defaults above
                break;

            case TankClass.Protector:
                // Shield specialist - higher defense and block
                BaseStats.baseDefense = 25f;            // Very high defense
                BaseStats.baseHealth = 160f;            // Even more health
                BaseStats.basePhysicalDamage = 10f;     // Lower damage (trade-off)
                BaseStats.baseStamina = 100f;           // Shield usage stamina
                break;
        }
    }

    private void InitializeSkills()
    {
        StartingSkills = new List<string>();

        switch (tankType)
        {
            case TankClass.Guardian:
                // Balanced tank skills
                StartingSkills.AddRange(new[]
                {
                    "shield_bash",       // Stun
                    "taunt",             // Force aggro
                    "defensive_stance"   // Defense boost
                });
                break;

            case TankClass.Protector:
                // Shield specialist skills
                StartingSkills.AddRange(new[]
                {
                    "shield_bash",       // Shield attack
                    "shield_block",      // Active blocking
                    "shield_wall"        // Large AoE block
                });
                break;
        }
    }

    public void Initialize(Player player)
    {
        // All tanks use melee stamina combat
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
            combat.SetCombatStyle(CombatStyle.MeleeStamina);
    }

    public void OnLevelUp(int newLevel)
    {
        BaseStats.SetLevel(newLevel);
    }
}