using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Guild system for player organizations.
/// </summary>
public class Guild : MonoBehaviour
{
    public string guildName;
    public int guildLevel = 1;
    public List<Player> members = new List<Player>();

    public void AddMember(Player player)
    {
        if (!members.Contains(player))
        {
            members.Add(player);
            Debug.Log($"✓ {player.name} joined {guildName}");
        }
    }

    public void RemoveMember(Player player)
    {
        members.Remove(player);
        Debug.Log($"✗ {player.name} left {guildName}");
    }

    public void RecordBossDefeat(int bossId)
    {
        Debug.Log($"🏆 Guild {guildName} defeated a boss!");
        guildLevel++;
    }
}