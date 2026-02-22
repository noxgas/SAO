using UnityEngine;

/// <summary>
/// Boss drop with hidden dual wield capability.
/// Description only hints at dual wield, not explicitly states it.
/// </summary>
[System.Serializable]
public class BossDrop
{
    public string dropId;
    public string dropName;
    public string description;  // This reveals if it's a dual wield sword!
    
    [Header("Reward Type")]
    public BossRewardType rewardType;
    
    [Header("Rarity & Stats")]
    public ItemRarity rarity;
    public float dropChance; // 0-1 probability
    
    [Header("Item Stats")]
    public float statBonus;
    public int goldReward;
    public int experienceReward;
    
    [Header("Special Properties")]
    public bool isBoundToCharacter;
    
    [Header("Dual Wield Properties (Hidden)")]
    public bool isDualWieldCapable;     // NOT shown in UI, only in description
    public bool isBossDrop = true;
    public string bossThatDropsIt;
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