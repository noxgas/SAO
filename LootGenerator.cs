using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates random loot drops based on rarity and floor level.
/// </summary>
public static class LootGenerator
{
    /// <summary>
    /// Roll for rarity tier based on floor and difficulty.
    /// </summary>
    public static ItemRarity RollRarity(int floorNumber, float difficultyMultiplier = 1f)
    {
        float roll = Random.value * difficultyMultiplier;
        
        // Floor-based rarity scaling
        float commonChance = Mathf.Max(0.1f, 0.5f - (floorNumber * 0.02f));
        float uncommonChance = Mathf.Max(0.1f, 0.3f - (floorNumber * 0.01f));
        float rareChance = Mathf.Min(0.4f, 0.15f + (floorNumber * 0.015f));
        float epicChance = Mathf.Min(0.3f, 0.04f + (floorNumber * 0.008f));

        if (roll < commonChance) return ItemRarity.Common;
        if (roll < commonChance + uncommonChance) return ItemRarity.Uncommon;
        if (roll < commonChance + uncommonChance + rareChance) return ItemRarity.Rare;
        if (roll < commonChance + uncommonChance + rareChance + epicChance) return ItemRarity.Epic;
        
        return ItemRarity.Legendary;
    }

    /// <summary>
    /// Calculate base item value by rarity and floor.
    /// </summary>
    public static float CalculateItemValue(ItemRarity rarity, int floorNumber)
    {
        float baseValue = 100f * floorNumber;
        
        return rarity switch
        {
            ItemRarity.Common => baseValue * 0.5f,
            ItemRarity.Uncommon => baseValue * 1f,
            ItemRarity.Rare => baseValue * 2.5f,
            ItemRarity.Epic => baseValue * 5f,
            ItemRarity.Legendary => baseValue * 10f,
            _ => baseValue
        };
    }
}