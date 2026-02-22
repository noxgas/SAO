using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all guilds in the game.
/// Handles guild creation, member management, and statistics.
/// </summary>
public class GuildManager : MonoBehaviour
{
    private static GuildManager instance;
    private Dictionary<string, Guild> allGuilds = new Dictionary<string, Guild>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static GuildManager Instance => instance;

    /// <summary>
    /// Create a new guild.
    /// </summary>
    public Guild CreateGuild(string guildName, Player founder)
    {
        if (allGuilds.ContainsKey(guildName))
        {
            Debug.LogWarning($"Guild {guildName} already exists!");
            return null;
        }

        Guild newGuild = new Guild(guildName);
        newGuild.AddMember(founder);
        allGuilds[guildName] = newGuild;

        Debug.Log($"Guild '{guildName}' created by {founder.name}");
        return newGuild;
    }

    /// <summary>
    /// Get a guild by name.
    /// </summary>
    public Guild GetGuild(string guildName)
    {
        return allGuilds.ContainsKey(guildName) ? allGuilds[guildName] : null;
    }

    /// <summary>
    /// Get all guilds.
    /// </summary>
    public List<Guild> GetAllGuilds()
    {
        return new List<Guild>(allGuilds.Values);
    }
}