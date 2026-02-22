using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages dual wield unlock acquisition and application.
/// Tracks which players have unlocked dual wielding.
/// </summary>
public class DualWieldUnlockSystem : MonoBehaviour
{
    private static DualWieldUnlockSystem instance;
    private HashSet<string> playersWithDualWield = new HashSet<string>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static DualWieldUnlockSystem Instance => instance;

    /// <summary>
    /// Check if a player has dual wield unlocked.
    /// </summary>
    public bool HasDualWieldUnlocked(string playerName)
    {
        return playersWithDualWield.Contains(playerName);
    }

    /// <summary>
    /// Unlock dual wield for a player.
    /// </summary>
    public void UnlockDualWield(Player player)
    {
        // Must be Swordsman
        if (player.CharacterClass.ClassName != "Swordsman")
        {
            Debug.LogError($"Only Swordsmen can unlock dual wielding!");
            return;
        }

        // Must be level 20+
        if (player.Level < 20)
        {
            Debug.LogError($"Must be level 20 to unlock dual wield!");
            return;
        }

        // Add to unlocked players
        playersWithDualWield.Add(player.name);

        // Reinitialize as dual wield
        player.InitializePlayer("Swordsman", "DualWield");

        Debug.Log($"🎉 {player.name} has UNLOCKED DUAL WIELD!");
    }

    /// <summary>
    /// Get count of players with dual wield unlocked (for stats).
    /// </summary>
    public int GetDualWieldPlayerCount()
    {
        return playersWithDualWield.Count;
    }
}