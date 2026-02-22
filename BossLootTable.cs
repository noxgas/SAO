using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the loot table for each dungeon boss.
/// Bosses drop better rewards than regular enemies.
/// 
/// REWARD STRUCTURE:
/// - Guaranteed Drops: All guild members get these
/// - Guild Base Rewards: All guild members get these
/// - Final Hit Drop: Only the player who gets the killing blow
/// - Random Drops: Distributed to party members
/// </summary>
[System.Serializable]
public class BossLootTable
{
    public string bossId;
    public string bossName;
    public int floorNumber;
    
    [Header("Guaranteed Drops (Everyone Gets)")]
    [SerializeField] private List<BossDrop> guaranteedDrops = new List<BossDrop>();
    
    [Header("Guild Base Rewards (All Members Get)")]
    [SerializeField] private int guildBaseGoldReward = 500;
    [SerializeField] private int guildBaseExperienceReward = 2000;
    [SerializeField] private List<BossDrop> guildBaseDrops = new List<BossDrop>();
    
    [Header("Final Hit Bonus (Only Killing Blow)")]
    [SerializeField] private BossDrop finalHitDrop;
    [SerializeField] private int finalHitBonusGold = 2000;
    [SerializeField] private int finalHitBonusExperience = 3000;
    
    [Header("Party Exclusive Drops (1-2 Per Party Member)")]
    [SerializeField] private List<BossDrop> randomPartyDrops = new List<BossDrop>();
    
    [Header("Base Rewards (Top Damage Dealers)")]
    [SerializeField] private int partyMemberGoldReward = 1000;
    [SerializeField] private int partyMemberExperienceReward = 5000;

    // Getters
    public List<BossDrop> GetGuaranteedDrops() => guaranteedDrops;
    public List<BossDrop> GetGuildBaseDrops() => guildBaseDrops;
    public List<BossDrop> GetRandomPartyDrops() => randomPartyDrops;
    public BossDrop GetFinalHitDrop() => finalHitDrop;
    
    public int GuildBaseGoldReward => guildBaseGoldReward;
    public int GuildBaseExperienceReward => guildBaseExperienceReward;
    
    public int FinalHitBonusGold => finalHitBonusGold;
    public int FinalHitBonusExperience => finalHitBonusExperience;
    
    public int PartyMemberGoldReward => partyMemberGoldReward;
    public int PartyMemberExperienceReward => partyMemberExperienceReward;

    public BossLootTable(string id, string name, int floor)
    {
        bossId = id;
        bossName = name;
        floorNumber = floor;
    }

    // Add to guaranteed drops (everyone gets)
    public void AddGuaranteedDrop(BossDrop drop)
    {
        guaranteedDrops.Add(drop);
    }

    // Add to guild base drops (all guild members get)
    public void AddGuildBaseDrop(BossDrop drop)
    {
        guildBaseDrops.Add(drop);
    }

    // Add to party exclusive drops (only party members get, 1-2 items)
    public void AddRandomPartyDrop(BossDrop drop)
    {
        randomPartyDrops.Add(drop);
    }

    // Set the final hit special drop (only killer gets)
    public void SetFinalHitDrop(BossDrop drop)
    {
        finalHitDrop = drop;
    }
}