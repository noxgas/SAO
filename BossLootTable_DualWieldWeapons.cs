using UnityEngine;
using System.Collections.Generic;

// In BossDefeatRewardSystem.cs InitializeBossLootTables(), update boss loot tables:

// Floor 1 Boss - Goblin King
BossLootTable floor1Boss = new BossLootTable("goblin_king", "Goblin King", 1);

floor1Boss.AddGuaranteedDrop(new BossDrop
{
    dropId = "goblin_king_trophy",
    dropName = "Goblin King's Trophy",
    description = "A trophy from the goblin king",
    rewardType = BossRewardType.Armor,
    rarity = ItemRarity.Common,
    statBonus = 2f,
    dropChance = 1f,
    experienceReward = 200,
    goldReward = 50,
    isBoundToCharacter = false,
    isDualWieldCapable = false,
    bossThatDropsIt = "goblin_king"
});

// DUAL WIELD CAPABLE WEAPON (0.5% drop chance)
floor1Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "twin_iron_blades_boss",
    dropName = "Twin Iron Blades",
    description = "A pair of perfectly matched iron swords. Can only be obtained from boss drops. DUAL WIELD CAPABLE!",
    rewardType = BossRewardType.Weapon,
    rarity = ItemRarity.Rare,
    statBonus = 14f,
    dropChance = 0.005f,  // 0.5% - Ultra rare!
    experienceReward = 500,
    goldReward = 200,
    isBoundToCharacter = true,
    isDualWieldCapable = true,  // ⭐ THIS CAN BE DUAL WIELDED
    bossThatDropsIt = "goblin_king"
});

// Floor 5 Boss - Frost Dragon
BossLootTable floor5Boss = new BossLootTable("frost_dragon", "Frost Dragon", 5);

// DUAL WIELD CAPABLE WEAPON (0.8% drop chance)
floor5Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "frost_edge_twin_boss",
    dropName = "Twin Frost Edges",
    description = "Twin blades infused with eternal ice. Boss-exclusive. DUAL WIELD CAPABLE!",
    rewardType = BossRewardType.Weapon,
    rarity = ItemRarity.Epic,
    statBonus = 20f,
    dropChance = 0.008f,  // 0.8% - Ultra rare!
    experienceReward = 1000,
    goldReward = 500,
    isBoundToCharacter = true,
    isDualWieldCapable = true,  // ⭐ THIS CAN BE DUAL WIELDED
    bossThatDropsIt = "frost_dragon"
});

// Floor 10 Boss - Shadow Monarch
BossLootTable floor10Boss = new BossLootTable("shadow_monarch", "Shadow Monarch", 10);

// DUAL WIELD CAPABLE WEAPON (1.0% drop chance)
floor10Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "shadow_twin_blades_boss",
    dropName = "Twin Shadow Blades",
    description = "Legendary blades forged from pure shadow. Ultra-rare boss exclusive. DUAL WIELD CAPABLE!",
    rewardType = BossRewardType.Weapon,
    rarity = ItemRarity.Legendary,
    statBonus = 26f,
    dropChance = 0.01f,  // 1.0% - Ultra rare!
    experienceReward = 2000,
    goldReward = 1000,
    isBoundToCharacter = true,
    isDualWieldCapable = true,  // ⭐ THIS CAN BE DUAL WIELDED
    bossThatDropsIt = "shadow_monarch"
});