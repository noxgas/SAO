using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Skill Tree System - Character progression through attribute allocation.
/// Players earn skill points by leveling up.
/// Spending points increases various player attributes.
/// 
/// Attributes Available:
/// - Health (Max HP)
/// - Damage (Physical Damage)
/// - Stamina (Max Stamina)
/// - Mana (Max Mana)
/// - Defense (Damage Reduction)
/// - Regen (HP/Stamina regen rate)
/// - Crit Chance (Critical hit chance)
/// - Move Speed (Character movement speed)
/// 
/// Points Earned:
/// - 1 point per level
/// - Bonus points for reaching milestone levels
/// - Bonus points from achievements
/// </summary>
public class SkillTreeSystem : MonoBehaviour
{
    [Header("Point Allocation")]
    [SerializeField] private int skillPointsPerLevel = 1;
    [SerializeField] private int bonusPointsPerMilestone = 5; // Every 10 levels
    [SerializeField] private int milestoneInterval = 10;

    [Header("Cost Per Attribute")]
    [SerializeField] private int healthCostPerPoint = 1;      // 1 point = 10 HP
    [SerializeField] private int damageCostPerPoint = 1;      // 1 point = 1 damage
    [SerializeField] private int staminaCostPerPoint = 1;     // 1 point = 5 stamina
    [SerializeField] private int manaCostPerPoint = 1;        // 1 point = 5 mana
    [SerializeField] private int defenseCostPerPoint = 2;     // 2 points = 1 defense
    [SerializeField] private int regenCostPerPoint = 1;       // 1 point = 0.5 regen
    [SerializeField] private int critCostPerPoint = 5;        // 5 points = 1% crit
    [SerializeField] private int speedCostPerPoint = 2;       // 2 points = 0.5 move speed

    [Header("Attribute Values Per Point")]
    [SerializeField] private float healthPerPoint = 10f;
    [SerializeField] private float damagePerPoint = 1f;
    [SerializeField] private float staminaPerPoint = 5f;
    [SerializeField] private float manaPerPoint = 5f;
    [SerializeField] private float defensePerPoint = 0.5f;
    [SerializeField] private float regenPerPoint = 0.5f;
    [SerializeField] private float critPerPoint = 0.01f;      // 1% per point
    [SerializeField] private float speedPerPoint = 0.5f;

    private Player player;
    private CharacterStats characterStats;

    // Current attribute allocations
    private Dictionary<AttributeType, int> attributeAllocations = new Dictionary<AttributeType, int>();
    private int availableSkillPoints = 0;
    private int spentSkillPoints = 0;
    private int totalSkillPointsEarned = 0;

    public enum AttributeType
    {
        Health,
        Damage,
        Stamina,
        Mana,
        Defense,
        Regen,
        CritChance,
        MoveSpeed
    }

    [System.Serializable]
    public class AttributeInfo
    {
        public AttributeType type;
        public string displayName;
        public string description;
        public int pointsInvested;
        public float currentValue;
        public float baseValue;
        public int costPerPoint;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        characterStats = GetComponent<CharacterStats>();

        InitializeAttributeAllocations();
    }

    private void InitializeAttributeAllocations()
    {
        attributeAllocations[AttributeType.Health] = 0;
        attributeAllocations[AttributeType.Damage] = 0;
        attributeAllocations[AttributeType.Stamina] = 0;
        attributeAllocations[AttributeType.Mana] = 0;
        attributeAllocations[AttributeType.Defense] = 0;
        attributeAllocations[AttributeType.Regen] = 0;
        attributeAllocations[AttributeType.CritChance] = 0;
        attributeAllocations[AttributeType.MoveSpeed] = 0;
    }

    /// <summary>
    /// Called when player levels up.
    /// Awards skill points based on level.
    /// </summary>
    public void OnPlayerLevelUp(int newLevel)
    {
        // Award base skill points
        int pointsEarned = skillPointsPerLevel;

        // Award bonus points at milestones (every 10 levels)
        if (newLevel % milestoneInterval == 0)
        {
            pointsEarned += bonusPointsPerMilestone;
            Debug.Log($"🎉 MILESTONE LEVEL {newLevel}! Earned {bonusPointsPerMilestone} bonus skill points!");
        }

        availableSkillPoints += pointsEarned;
        totalSkillPointsEarned += pointsEarned;

        Debug.Log($"⬆️ Level {newLevel} reached!");
        Debug.Log($"✚ Gained {pointsEarned} skill points");
        Debug.Log($"📊 Available points: {availableSkillPoints}");
    }

    /// <summary>
    /// Allocate a skill point to an attribute.
    /// </summary>
    public bool AllocatePoint(AttributeType attributeType)
    {
        if (availableSkillPoints <= 0)
        {
            Debug.LogWarning("❌ No available skill points!");
            return false;
        }

        int costPerPoint = GetCostPerPoint(attributeType);

        if (availableSkillPoints < costPerPoint)
        {
            Debug.LogWarning($"❌ Not enough points! Need {costPerPoint}, have {availableSkillPoints}");
            return false;
        }

        // Deduct points
        availableSkillPoints -= costPerPoint;
        attributeAllocations[attributeType]++;
        spentSkillPoints += costPerPoint;

        // Apply attribute bonus
        ApplyAttributeBonus(attributeType);

        Debug.Log($"✅ Allocated point to {attributeType}");
        Debug.Log($"   Level: {attributeAllocations[attributeType]}");
        Debug.Log($"   Available: {availableSkillPoints}");

        return true;
    }

    /// <summary>
    /// Allocate multiple points to an attribute.
    /// </summary>
    public bool AllocatePoints(AttributeType attributeType, int pointCount)
    {
        for (int i = 0; i < pointCount; i++)
        {
            if (!AllocatePoint(attributeType))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Apply the attribute bonus to player stats.
    /// </summary>
    private void ApplyAttributeBonus(AttributeType attributeType)
    {
        if (characterStats == null)
            return;

        int pointsInvested = attributeAllocations[attributeType];

        switch (attributeType)
        {
            case AttributeType.Health:
                characterStats.baseHealth = characterStats.baseHealth + healthPerPoint;
                break;

            case AttributeType.Damage:
                characterStats.basePhysicalDamage += damagePerPoint;
                break;

            case AttributeType.Stamina:
                characterStats.baseStamina += staminaPerPoint;
                break;

            case AttributeType.Mana:
                characterStats.baseMana += manaPerPoint;
                break;

            case AttributeType.Defense:
                characterStats.baseDefense += defensePerPoint;
                break;

            case AttributeType.Regen:
                // Regen handled in PlayerCombat
                PlayerCombat combat = GetComponent<PlayerCombat>();
                if (combat != null)
                {
                    combat.SetStaminaRegenRate(combat.GetStaminaRegenRate() + regenPerPoint);
                }
                break;

            case AttributeType.CritChance:
                characterStats.baseCritChance += critPerPoint;
                break;

            case AttributeType.MoveSpeed:
                characterStats.baseMoveSpeed += speedPerPoint;
                break;
        }
    }

    /// <summary>
    /// Get the cost in points for an attribute.
    /// </summary>
    private int GetCostPerPoint(AttributeType attributeType)
    {
        return attributeType switch
        {
            AttributeType.Health => healthCostPerPoint,
            AttributeType.Damage => damageCostPerPoint,
            AttributeType.Stamina => staminaCostPerPoint,
            AttributeType.Mana => manaCostPerPoint,
            AttributeType.Defense => defenseCostPerPoint,
            AttributeType.Regen => regenCostPerPoint,
            AttributeType.CritChance => critCostPerPoint,
            AttributeType.MoveSpeed => speedCostPerPoint,
            _ => 1
        };
    }

    /// <summary>
    /// Get attribute info for UI display.
    /// </summary>
    public AttributeInfo GetAttributeInfo(AttributeType attributeType)
    {
        float currentValue = GetCurrentAttributeValue(attributeType);
        float baseValue = GetBaseAttributeValue(attributeType);

        return new AttributeInfo
        {
            type = attributeType,
            displayName = attributeType.ToString(),
            description = GetAttributeDescription(attributeType),
            pointsInvested = attributeAllocations[attributeType],
            currentValue = currentValue,
            baseValue = baseValue,
            costPerPoint = GetCostPerPoint(attributeType)
        };
    }

    /// <summary>
    /// Get current value of an attribute (base + allocated).
    /// </summary>
    private float GetCurrentAttributeValue(AttributeType attributeType)
    {
        if (characterStats == null)
            return 0f;

        int pointsInvested = attributeAllocations[attributeType];

        return attributeType switch
        {
            AttributeType.Health => characterStats.baseHealth,
            AttributeType.Damage => characterStats.basePhysicalDamage,
            AttributeType.Stamina => characterStats.baseStamina,
            AttributeType.Mana => characterStats.baseMana,
            AttributeType.Defense => characterStats.baseDefense,
            AttributeType.Regen => GetStaminaRegenValue(),
            AttributeType.CritChance => characterStats.baseCritChance,
            AttributeType.MoveSpeed => characterStats.baseMoveSpeed,
            _ => 0f
        };
    }

    /// <summary>
    /// Get base value of an attribute (without allocations).
    /// </summary>
    private float GetBaseAttributeValue(AttributeType attributeType)
    {
        if (characterStats == null)
            return 0f;

        int pointsInvested = attributeAllocations[attributeType];

        return attributeType switch
        {
            AttributeType.Health => characterStats.baseHealth - (pointsInvested * (int)healthPerPoint),
            AttributeType.Damage => characterStats.basePhysicalDamage - (pointsInvested * damagePerPoint),
            AttributeType.Stamina => characterStats.baseStamina - (pointsInvested * staminaPerPoint),
            AttributeType.Mana => characterStats.baseMana - (pointsInvested * manaPerPoint),
            AttributeType.Defense => characterStats.baseDefense - (pointsInvested * defensePerPoint),
            AttributeType.Regen => GetStaminaRegenValue() - (pointsInvested * regenPerPoint),
            AttributeType.CritChance => characterStats.baseCritChance - (pointsInvested * critPerPoint),
            AttributeType.MoveSpeed => characterStats.baseMoveSpeed - (pointsInvested * speedPerPoint),
            _ => 0f
        };
    }

    private float GetStaminaRegenValue()
    {
        PlayerCombat combat = GetComponent<PlayerCombat>();
        return combat != null ? combat.GetStaminaRegenRate() : 30f;
    }

    /// <summary>
    /// Get description for an attribute.
    /// </summary>
    private string GetAttributeDescription(AttributeType attributeType)
    {
        return attributeType switch
        {
            AttributeType.Health => "Increases maximum health points",
            AttributeType.Damage => "Increases physical damage output",
            AttributeType.Stamina => "Increases maximum stamina for combat",
            AttributeType.Mana => "Increases maximum mana for spells",
            AttributeType.Defense => "Increases damage reduction",
            AttributeType.Regen => "Increases stamina regeneration rate",
            AttributeType.CritChance => "Increases critical hit chance",
            AttributeType.MoveSpeed => "Increases movement speed",
            _ => "Unknown attribute"
        };
    }

    /// <summary>
    /// Get all allocated attributes for display.
    /// </summary>
    public List<AttributeInfo> GetAllAttributes()
    {
        List<AttributeInfo> attributes = new List<AttributeInfo>();

        foreach (AttributeType type in System.Enum.GetValues(typeof(AttributeType)))
        {
            attributes.Add(GetAttributeInfo(type));
        }

        return attributes;
    }

    /// <summary>
    /// Reset all allocations (for respec - costs gold).
    /// </summary>
    public void ResetAllocations(int respecCost = 1000)
    {
        Debug.Log($"Resetting skill tree allocations (Cost: {respecCost} Gold)");

        availableSkillPoints += spentSkillPoints;
        spentSkillPoints = 0;
        InitializeAttributeAllocations();

        // TODO: Deduct respecCost from player gold
    }

    // Getters
    public int GetAvailableSkillPoints() => availableSkillPoints;
    public int GetSpentSkillPoints() => spentSkillPoints;
    public int GetTotalSkillPointsEarned() => totalSkillPointsEarned;
}