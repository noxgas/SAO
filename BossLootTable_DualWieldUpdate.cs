using UnityEngine;
using System.Collections.Generic;

// Add this to Floor 5+ bosses in BossDefeatRewardSystem.cs InitializeBossLootTables()

// Example: Floor 5 Boss - Frost Dragon
BossLootTable floor5Boss = new BossLootTable("frost_dragon", "Frost Dragon", 5);

// ... existing drops ...

// ULTRA-RARE DUAL WIELD UNLOCK (< 1% chance, floor 5+)
floor5Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "dual_wield_unlock",
    dropName = "Dual Wield Codex",
    description = "An ancient codex teaching the art of dual wielding. Ultra-rare. Swordsmen only.",
    rewardType = BossRewardType.TraitUnlock,
    rarity = ItemRarity.Legendary,
    dropChance = 0.005f,  // 0.5% - Less than 1%
    experienceReward = 0,
    goldReward = 0,
    isBoundToCharacter = true,
    isDualWieldUnlock = true
});