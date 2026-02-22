// In BossDefeatRewardSystem.cs InitializeBossLootTables()

// Floor 1 Boss - Goblin King
BossLootTable floor1Boss = new BossLootTable("goblin_king", "Goblin King", 1);

floor1Boss.AddGuaranteedDrop(new BossDrop
{
    dropId = "goblin_king_trophy",
    dropName = "Goblin King's Trophy",
    description = "A trophy from the goblin king - proof of victory",
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

// HIDDEN DUAL WIELD SWORD (0.5% - ultra rare, description hints at it)
floor1Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "iron_twin_edge",
    dropName = "Iron Twin Edge",
    description = "A finely crafted iron sword. Its perfect balance suggests it was made to pair with another...",  // HINT!
    rewardType = BossRewardType.Weapon,
    rarity = ItemRarity.Rare,
    statBonus = 14f,
    dropChance = 0.005f,  // 0.5%
    experienceReward = 500,
    goldReward = 200,
    isBoundToCharacter = true,
    isDualWieldCapable = true,  // HIDDEN - not in UI, just description hint
    bossThatDropsIt = "goblin_king"
});

// Floor 5 Boss - Frost Dragon
BossLootTable floor5Boss = new BossLootTable("frost_dragon", "Frost Dragon", 5);

// HIDDEN DUAL WIELD SWORD (0.8%)
floor5Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "frost_edge_dual",
    dropName = "Frost Edge",
    description = "A blade of pure ice. Lighter than it should be, as if meant to be wielded alongside another blade...",  // HINT!
    rewardType = BossRewardType.Weapon,
    rarity = ItemRarity.Epic,
    statBonus = 20f,
    dropChance = 0.008f,  // 0.8%
    experienceReward = 1000,
    goldReward = 500,
    isBoundToCharacter = true,
    isDualWieldCapable = true,  // HIDDEN
    bossThatDropsIt = "frost_dragon"
});

// Floor 10 Boss - Shadow Monarch
BossLootTable floor10Boss = new BossLootTable("shadow_monarch", "Shadow Monarch", 10);

// HIDDEN DUAL WIELD SWORD (1.0%)
floor10Boss.AddRandomPartyDrop(new BossDrop
{
    dropId = "shadow_edge_dual",
    dropName = "Shadow Edge",
    description = "A blade forged from shadow itself. Perfectly balanced for dual combat. When equipped with a main blade, its true power awakens...",  // CLEARER HINT!
    rewardType = BossRewardType.Weapon,
    rarity = ItemRarity.Legendary,
    statBonus = 26f,
    dropChance = 0.01f,  // 1.0%
    experienceReward = 2000,
    goldReward = 1000,
    isBoundToCharacter = true,
    isDualWieldCapable = true,  // HIDDEN
    bossThatDropsIt = "shadow_monarch"
});