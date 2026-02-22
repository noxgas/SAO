using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Guild system for player grouping and benefits.
/// </summary>
public class Guild
{
    public string guildName;
    public Player guildLeader;
    public List<Player> members = new List<Player>();
    
    [System.Serializable]
    public class GuildStats
    {
        public int totalBossesDefeated = 0;
        public int totalGoldEarned = 0;
        public long foundedTime;
    }

    public GuildStats stats = new GuildStats();

    public Guild(string name)
    {
        guildName = name;
        stats.foundedTime = System.DateTime.Now.Ticks;
    }

    public void AddMember(Player player)
    {
        if (!members.Contains(player))
            members.Add(player);
    }

    public void RemoveMember(Player player)
    {
        if (members.Contains(player))
            members.Remove(player);
    }

    public void RecordBossDefeat(int goldEarned)
    {
        stats.totalBossesDefeated++;
        stats.totalGoldEarned += goldEarned;
    }

    public int GetMemberCount() => members.Count;
}