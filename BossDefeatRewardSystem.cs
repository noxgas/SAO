using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles boss defeat rewards and loot distribution.
/// 
/// REWARD DISTRIBUTION:
/// 1. ALL GUILD MEMBERS: Guaranteed drops + Guild base drops + Base gold/XP
/// 2. TOP 4 DAMAGE DEALERS (Party): Above + Party exclusive drops + Party bonus rewards
/// 3. FINAL HIT PLAYER: Above + Special final hit drop + Final hit bonuses
/// </summary>
public class BossDefeatRewardSystem : MonoBehaviour
{
    [System.Serializable]
    public class DefeatReward
    {
        public Player defeatingPlayer;
        public RewardType rewardType;
        public List<BossDrop> earnedLoot = new List<BossDrop>();
        public int goldEarned;
        public int experienceEarned;
    }

    public enum RewardType
    {
        GuildMember,
        PartyMember,
        FinalHitKiller
    }

    private static BossDefeatRewardSystem instance;

    private Dictionary<string, BossLootTable> bossLootTables = new Dictionary<string, BossLootTable>();
    private List<DefeatReward> recentRewards = new List<DefeatReward>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        InitializeBossLootTables();
    }

    public static BossDefeatRewardSystem Instance => instance;

    private void InitializeBossLootTables()
    {
        // Floor 1 Boss - Goblin King
        BossLootTable floor1Boss = new BossLootTable("goblin_king", "Goblin King", 1);

        floor1Boss.AddGuaranteedDrop(new BossDrop
        {
            dropId = "goblin_king_trophy",
            dropName = "Goblin King's Trophy",
            description = "A trophy from the goblin king - shared amongst those who fought",
            rewardType = BossRewardType.Armor,
            rarity = ItemRarity.Common,
            statBonus = 2f,
            dropChance = 1f,
            experienceReward = 200,
            goldReward = 50,
            isBoundToCharacter = false
        });

        floor1Boss.AddGuildBaseDrop(new BossDrop
        {
            dropId = "goblin_gold_coin",
            dropName = "Goblin Gold Coin",
            description = "Guild reward - standard bonus from defeating the boss",
            rewardType = BossRewardType.Gold,
            rarity = ItemRarity.Common,
            dropChance = 1f,
            experienceReward = 100,
            goldReward = 200,
            isBoundToCharacter = false
        });

        floor1Boss.SetFinalHitDrop(new BossDrop
        {
            dropId = "goblin_king_crown",
            dropName = "Goblin King's Crown",
            description = "The legendary crown of the goblin king - only for the one who delivered the final blow!",
            rewardType = BossRewardType.Armor,
            rarity = ItemRarity.Rare,
            statBonus = 8f,
            dropChance = 1f,
            experienceReward = 1000,
            goldReward = 500,
            isBoundToCharacter = true
        });

        floor1Boss.AddRandomPartyDrop(new BossDrop
        {
            dropId = "iron_great_sword",
            dropName = "Iron Great Sword",
            description = "A mighty greatsword for the party that defeated the king",
            rewardType = BossRewardType.Weapon,
            rarity = ItemRarity.Uncommon,
            statBonus = 18f,
            dropChance = 0.6f,
            experienceReward = 300,
            goldReward = 200,
            isBoundToCharacter = false
        });

        floor1Boss.AddRandomPartyDrop(new BossDrop
        {
            dropId = "twin_iron_blades_boss",
            dropName = "Twin Iron Blades",
            description = "A pair of perfectly matched iron swords. Can only be obtained from boss drops. DUAL WIELD CAPABLE!",
            rewardType = BossRewardType.Weapon,
            rarity = ItemRarity.Rare,
            statBonus = 14f,
            dropChance = 0.005f,
            experienceReward = 500,
            goldReward = 200,
            isBoundToCharacter = true,
            isDualWieldCapable = true,
            bossThatDropsIt = "goblin_king"
        });

        bossLootTables["goblin_king"] = floor1Boss;

        // Floor 5 Boss - Frost Dragon
        BossLootTable floor5Boss = new BossLootTable("frost_dragon", "Frost Dragon", 5);

        floor5Boss.AddGuaranteedDrop(new BossDrop
        {
            dropId = "frost_shard",
            dropName = "Frost Shard",
            description = "A shard of ice from the dragon - shared amongst all participants",
            rewardType = BossRewardType.Accessory,
            rarity = ItemRarity.Uncommon,
            statBonus = 5f,
            dropChance = 1f,
            experienceReward = 500,
            goldReward = 100,
            isBoundToCharacter = false
        });

        floor5Boss.AddGuildBaseDrop(new BossDrop
        {
            dropId = "dragon_scale_fragment",
            dropName = "Dragon Scale Fragment",
            description = "Guild members receive this bonus from the dragon",
            rewardType = BossRewardType.Armor,
            rarity = ItemRarity.Uncommon,
            statBonus = 3f,
            dropChance = 1f,
            experienceReward = 300,
            goldReward = 300,
            isBoundToCharacter = false
        });

        floor5Boss.SetFinalHitDrop(new BossDrop
        {
            dropId = "dragon_scale_armor_final",
            dropName = "Dragon Scale Armor",
            description = "Impenetrable dragon scales - awarded to the player who dealt the final blow",
            rewardType = BossRewardType.Armor,
            rarity = ItemRarity.Epic,
            statBonus = 20f,
            dropChance = 1f,
            experienceReward = 2500,
            goldReward = 1500,
            isBoundToCharacter = true
        });

        floor5Boss.AddRandomPartyDrop(new BossDrop
        {
            dropId = "frost_edge_party",
            dropName = "Frost Edge",
            description = "A blade infused with eternal ice - party exclusive",
            rewardType = BossRewardType.Weapon,
            rarity = ItemRarity.Rare,
            statBonus = 24f,
            dropChance = 0.5f,
            experienceReward = 800,
            goldReward = 400,
            isBoundToCharacter = false
        });

        floor5Boss.AddRandomPartyDrop(new BossDrop
        {
            dropId = "frost_edge_twin_boss",
            dropName = "Twin Frost Edges",
            description = "Twin blades infused with eternal ice. Boss-exclusive. DUAL WIELD CAPABLE!",
            rewardType = BossRewardType.Weapon,
            rarity = ItemRarity.Epic,
            statBonus = 20f,
            dropChance = 0.008f,
            experienceReward = 1000,
            goldReward = 500,
            isBoundToCharacter = true,
            isDualWieldCapable = true,
            bossThatDropsIt = "frost_dragon"
        });

        bossLootTables["frost_dragon"] = floor5Boss;

        // Floor 10 Boss - Shadow Monarch
        BossLootTable floor10Boss = new BossLootTable("shadow_monarch", "Shadow Monarch", 10);

        floor10Boss.AddGuaranteedDrop(new BossDrop
        {
            dropId = "shadow_essence",
            dropName = "Shadow Essence",
            description = "A drop of pure shadow - shared amongst all who participated",
            rewardType = BossRewardType.Accessory,
            rarity = ItemRarity.Rare,
            statBonus = 8f,
            dropChance = 1f,
            experienceReward = 1000,
            goldReward = 200,
            isBoundToCharacter = false
        });

        floor10Boss.AddGuildBaseDrop(new BossDrop
        {
            dropId = "shadow_blessing",
            dropName = "Shadow Blessing",
            description = "The guild receives this blessing from defeating the Shadow Monarch",
            rewardType = BossRewardType.Accessory,
            rarity = ItemRarity.Rare,
            statBonus = 6f,
            dropChance = 1f,
            experienceReward = 800,
            goldReward = 600,
            isBoundToCharacter = false
        });

        floor10Boss.SetFinalHitDrop(new BossDrop
        {
            dropId = "shadow_crown_final",
            dropName = "Shadow Monarch's Crown",
            description = "The crown of the Shadow Monarch - only for the one who delivered the fatal strike",
            rewardType = BossRewardType.Armor,
            rarity = ItemRarity.Legendary,
            statBonus = 25f,
            dropChance = 1f,
            experienceReward = 4000,
            goldReward = 3000,
            isBoundToCharacter = true
        });

        floor10Boss.AddRandomPartyDrop(new BossDrop
        {
            dropId = "shadow_twin_blades_party",
            dropName = "Shadow Twin Blades",
            description = "Blades that pierce through shadows - party exclusive",
            rewardType = BossRewardType.Weapon,
            rarity = ItemRarity.Epic,
            statBonus = 26f,
            dropChance = 0.4f,
            experienceReward = 1200,
            goldReward = 600,
            isBoundToCharacter = false
        });

        floor10Boss.AddRandomPartyDrop(new BossDrop
        {
            dropId = "shadow_twin_blades_boss",
            dropName = "Twin Shadow Blades",
            description = "Legendary blades forged from pure shadow. Ultra-rare boss exclusive. DUAL WIELD CAPABLE!",
            rewardType = BossRewardType.Weapon,
            rarity = ItemRarity.Legendary,
            statBonus = 26f,
            dropChance = 0.01f,
            experienceReward = 2000,
            goldReward = 1000,
            isBoundToCharacter = true,
            isDualWieldCapable = true,
            bossThatDropsIt = "shadow_monarch"
        });

        bossLootTables["shadow_monarch"] = floor10Boss;
    }

    public List<DefeatReward> ProcessBossDefeat(
        string bossId,
        Player finalHitPlayer,
        List<Player> partyMembers,
        Guild defeatersGuild)
    {
        if (!bossLootTables.TryGetValue(bossId, out var lootTable))
        {
            Debug.LogError($"Boss loot table not found: {bossId}");
            return new List<DefeatReward>();
        }

        List<DefeatReward> rewards = new List<DefeatReward>();

        if (defeatersGuild != null)
        {
            foreach (var guildMember in defeatersGuild.members)
            {
                DefeatReward guildReward = new DefeatReward
                {
                    defeatingPlayer = guildMember,
                    rewardType = RewardType.GuildMember,
                    goldEarned = lootTable.GuildBaseGoldReward,
                    experienceEarned = lootTable.GuildBaseExperienceReward
                };

                guildReward.earnedLoot.AddRange(lootTable.GetGuaranteedDrops());
                guildReward.earnedLoot.AddRange(lootTable.GetGuildBaseDrops());

                rewards.Add(guildReward);
                ApplyRewardToPlayer(guildMember, guildReward);

                Debug.Log($"[GUILD REWARD] {guildMember.name} received: {guildReward.goldEarned} Gold, {guildReward.experienceEarned} XP");
            }
        }

        List<BossDrop> partyDrops = RollRandomDrops(lootTable.GetRandomPartyDrops());

        for (int i = 0; i < partyMembers.Count; i++)
        {
            Player partyMember = partyMembers[i];

            if (partyMember == finalHitPlayer)
                continue;

            DefeatReward partyReward = new DefeatReward
            {
                defeatingPlayer = partyMember,
                rewardType = RewardType.PartyMember,
                goldEarned = lootTable.PartyMemberGoldReward,
                experienceEarned = lootTable.PartyMemberExperienceReward
            };

            if (i < partyDrops.Count)
            {
                partyReward.earnedLoot.Add(partyDrops[i]);
            }

            rewards.Add(partyReward);
            ApplyRewardToPlayer(partyMember, partyReward);

            Debug.Log($"[PARTY REWARD] {partyMember.name} received: {partyReward.goldEarned} Gold, {partyReward.experienceEarned} XP");
        }

        DefeatReward finalHitReward = new DefeatReward
        {
            defeatingPlayer = finalHitPlayer,
            rewardType = RewardType.FinalHitKiller,
            goldEarned = lootTable.FinalHitBonusGold,
            experienceEarned = lootTable.FinalHitBonusExperience
        };

        if (lootTable.GetFinalHitDrop() != null)
        {
            finalHitReward.earnedLoot.Add(lootTable.GetFinalHitDrop());
        }

        if (partyDrops.Count > 0)
        {
            finalHitReward.earnedLoot.Add(partyDrops[0]);
        }

        rewards.Add(finalHitReward);
        ApplyRewardToPlayer(finalHitPlayer, finalHitReward);

        Debug.Log($"⚡ [FINAL HIT BONUS] {finalHitPlayer.name} received: {finalHitReward.goldEarned} Gold, {finalHitReward.experienceEarned} XP + SPECIAL DROP!");

        recentRewards.AddRange(rewards);

        return rewards;
    }

    private List<BossDrop> RollRandomDrops(List<BossDrop> possibleDrops)
    {
        List<BossDrop> rolledDrops = new List<BossDrop>();

        foreach (var drop in possibleDrops)
        {
            if (Random.value <= drop.dropChance)
            {
                rolledDrops.Add(drop);
            }
        }

        return rolledDrops.Count > 2 ? rolledDrops.GetRange(0, 2) : rolledDrops;
    }

    private void ApplyRewardToPlayer(Player player, DefeatReward reward)
    {
        player.GainExperience(reward.experienceEarned);

        Equipment equipment = player.GetComponent<Equipment>();
        if (equipment != null)
        {
            foreach (var drop in reward.earnedLoot)
            {
                AddBossDropToInventory(player, drop);
            }
        }
    }

    private void AddBossDropToInventory(Player player, BossDrop drop)
    {
        Equipment equipment = player.GetComponent<Equipment>();
        if (equipment == null) return;

        Equipment.EquipmentItem item = new Equipment.EquipmentItem
        {
            itemId = drop.dropId,
            itemName = drop.dropName,
            description = drop.description,
            rarity = drop.rarity,
            damage = drop.statBonus,
            isDualWieldSword = drop.isDualWieldCapable
        };

        equipment.AddItemToInventory(item);
        Debug.Log($"✓ Added {drop.dropName} ({drop.rarity}) to {player.name}'s inventory");

        if (drop.isDualWieldCapable)
        {
            Debug.Log($"⚡ ULTRA-RARE DUAL WIELD WEAPON DROPPED!");
            DualWieldWeaponDetector.CheckAndEnableDualWield(player, drop);
        }
    }

    public BossLootTable GetBossLootTable(string bossId)
    {
        return bossLootTables.ContainsKey(bossId) ? bossLootTables[bossId] : null;
    }

    public List<DefeatReward> GetRecentRewards() => recentRewards;
}