using UnityEngine;

/// <summary>
/// Data structure for boss defeat rewards.
/// Different from regular dungeon drops - these are exclusive to the boss killer.
/// </summary>
[System.Serializable]
public class BossDrop
{
    public string dropId;
    public string dropName;
    public string description;
    
    [Header("Reward Type")]
    public BossRewardType rewardType;
    
    [Header("Rarity & Stats")]
    public ItemRarity rarity;
    public float dropChance; // 0-1 probability
    
    [Header("Item Stats")]
    public float statBonus; // Varies by type (damage, defense, etc)
    public int goldReward;
    
    [Header("Experience")]
    public int experienceReward;
    
    [Header("Special Properties")]
    public bool isBoundToCharacter; // Can't be traded
    public bool isDualWieldCompatible; // For dual wield weapons
}

public enum BossRewardType
{
    Weapon,
    Armor,
    Accessory,
    Gold,
    Experience,
    SkillScroll,
    CosmeticItem
}