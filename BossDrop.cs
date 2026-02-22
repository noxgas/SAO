using UnityEngine;

/// <summary>
/// Single loot drop from a boss.
/// Can be a weapon, armor, accessory, or consumable.
/// </summary>
public class BossDrop
{
    public string dropId;
    public string dropName;
    public string description;
    public BossRewardType rewardType;
    public ItemRarity rarity;
    public float statBonus;
    public float dropChance;
    public int experienceReward;
    public int goldReward;
    public bool isBoundToCharacter;
    public bool isDualWieldCapable;
    public string bossThatDropsIt;

    public BossDrop Clone()
    {
        return new BossDrop
        {
            dropId = this.dropId,
            dropName = this.dropName,
            description = this.description,
            rewardType = this.rewardType,
            rarity = this.rarity,
            statBonus = this.statBonus,
            dropChance = this.dropChance,
            experienceReward = this.experienceReward,
            goldReward = this.goldReward,
            isBoundToCharacter = this.isBoundToCharacter,
            isDualWieldCapable = this.isDualWieldCapable,
            bossThatDropsIt = this.bossThatDropsIt
        };
    }
}

public enum BossRewardType
{
    Weapon,
    Armor,
    Accessory,
    Consumable,
    Gold,
    Experience
}