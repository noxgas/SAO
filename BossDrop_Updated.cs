using UnityEngine;

/// <summary>
/// Data structure for boss defeat rewards.
/// Only boss-dropped weapons flagged as dual wield capable can be used for dual wielding.
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
    public int experienceReward;
    
    [Header("Special Properties")]
    public bool isBoundToCharacter;      // Can't be traded
    
    [Header("Dual Wield Properties")]
    public bool isDualWieldCapable;      // Can this be dual wielded?
    public bool isBossDrop = true;       // All BossDrops are boss drops
    public string bossThatDropsIt;       // Which boss drops this weapon
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