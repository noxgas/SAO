using UnityEngine;

/// <summary>
/// Generates random loot drops based on floor level.
/// Higher floors AND challenging lower floors = better loot chances.
/// </summary>
public static class LootGenerator
{
    /// <summary>
    /// Roll for rarity tier based on floor.
    /// Higher floors have exponentially better loot chances.
    /// 
    /// Floor 1: Common 50%, Uncommon 30%, Rare 15%, Epic 4%, Legendary 1%
    /// Floor 25: Common 10%, Uncommon 20%, Rare 35%, Epic 25%, Legendary 10%
    /// Floor 50: Common 2%, Uncommon 8%, Rare 25%, Epic 40%, Legendary 25%
    /// </summary>
    public static ItemRarity RollRarity(int floorNumber, float difficultyMultiplier = 1f)
    {
        float roll = Random.value * difficultyMultiplier;
        
        // Calculate rarity thresholds based on floor
        float commonBase = 0.5f - (floorNumber * 0.004f);
        float uncommonBase = 0.3f - (floorNumber * 0.002f);
        float rareBase = 0.15f + (floorNumber * 0.004f);
        float epicBase = 0.04f + (floorNumber * 0.003f);

        // Clamp values
        float commonChance = Mathf.Clamp01(commonBase);
        float uncommonChance = Mathf.Clamp01(uncommonBase);
        float rareChance = Mathf.Clamp01(rareBase);
        float epicChance = Mathf.Clamp01(epicBase);

        // Normalize so they sum to 1
        float total = commonChance + uncommonChance + rareChance + epicChance;
        commonChance /= total;
        uncommonChance /= total;
        rareChance /= total;
        epicChance /= total;

        // Roll based on thresholds
        if (roll < commonChance) return ItemRarity.Common;
        if (roll < commonChance + uncommonChance) return ItemRarity.Uncommon;
        if (roll < commonChance + uncommonChance + rareChance) return ItemRarity.Rare;
        if (roll < commonChance + uncommonChance + rareChance + epicChance) return ItemRarity.Epic;
        
        return ItemRarity.Legendary;
    }

    /// <summary>
    /// Calculate item damage by rarity and floor.
    /// Scales exponentially with floor level.
    /// </summary>
    public static float CalculateItemDamage(ItemRarity rarity, int floorNumber)
    {
        float baseFloor1Damage = rarity switch
        {
            ItemRarity.Common => Random.Range(30f, 50f),
            ItemRarity.Uncommon => Random.Range(50f, 75f),
            ItemRarity.Rare => Random.Range(75f, 120f),
            ItemRarity.Epic => Random.Range(120f, 200f),
            ItemRarity.Legendary => Random.Range(200f, 350f),
            _ => 30f
        };

        // Scale by floor
        return DamageScalingSystem.Instance.ScaleDamage(baseFloor1Damage, floorNumber);
    }

    /// <summary>
    /// Calculate item sell value.
    /// Rarer items worth more on higher floors.
    /// </summary>
    public static int CalculateItemSellValue(ItemRarity rarity, int floorNumber, float itemDamage)
    {
        float baseSellValue = rarity switch
        {
            ItemRarity.Common => itemDamage * 5f,
            ItemRarity.Uncommon => itemDamage * 10f,
            ItemRarity.Rare => itemDamage * 25f,
            ItemRarity.Epic => itemDamage * 50f,
            ItemRarity.Legendary => itemDamage * 100f,
            _ => itemDamage * 5f
        };

        // Higher floor multiplier
        float floorBonus = 1f + (floorNumber * 0.05f);
        return (int)(baseSellValue * floorBonus);
    }
}