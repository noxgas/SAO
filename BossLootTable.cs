using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Loot table for a specific boss.
/// Defines guaranteed drops, guild drops, party drops, and final hit drops.
/// </summary>
public class BossLootTable
{
    public string bossId;
    public string bossName;
    public int floor;

    private List<BossDrop> guaranteedDrops = new List<BossDrop>();
    private List<BossDrop> guildBaseDrops = new List<BossDrop>();
    private List<BossDrop> randomPartyDrops = new List<BossDrop>();
    private BossDrop finalHitDrop;

    public int GuildBaseGoldReward = 100;
    public int GuildBaseExperienceReward = 50;
    public int PartyMemberGoldReward = 200;
    public int PartyMemberExperienceReward = 150;
    public int FinalHitBonusGold = 500;
    public int FinalHitBonusExperience = 500;

    public BossLootTable(string id, string name, int floorNumber)
    {
        bossId = id;
        bossName = name;
        floor = floorNumber;
    }

    public void AddGuaranteedDrop(BossDrop drop)
    {
        guaranteedDrops.Add(drop);
    }

    public void AddGuildBaseDrop(BossDrop drop)
    {
        guildBaseDrops.Add(drop);
    }

    public void AddRandomPartyDrop(BossDrop drop)
    {
        randomPartyDrops.Add(drop);
    }

    public void SetFinalHitDrop(BossDrop drop)
    {
        finalHitDrop = drop;
    }

    public List<BossDrop> GetGuaranteedDrops() => guaranteedDrops;
    public List<BossDrop> GetGuildBaseDrops() => guildBaseDrops;
    public List<BossDrop> GetRandomPartyDrops() => randomPartyDrops;
    public BossDrop GetFinalHitDrop() => finalHitDrop;
}