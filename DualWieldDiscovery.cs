using UnityEngine;

/// <summary>
/// Handles the discovery of dual wielding as a hidden feature.
/// Shows special message when player equips a dual wield sword.
/// </summary>
public class DualWieldDiscovery : MonoBehaviour
{
    /// <summary>
    /// Called when a player equips a dual wield sword to left hand.
    /// Reveals the hidden feature.
    /// </summary>
    public static void DiscoverDualWield(Player player, Equipment.EquipmentItem sword)
    {
        Debug.Log($"\n╔════════════════════════════════════════════════╗");
        Debug.Log($"║           🎉 HIDDEN FEATURE UNLOCKED! 🎉         ║");
        Debug.Log($"╚════════════════════════════════════════════════╝\n");
        
        Debug.Log($"✨ You have discovered DUAL WIELDING!");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"The {sword.itemName} revealed its true purpose.");
        Debug.Log($"\n{sword.description}\n");
        Debug.Log($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"NEW ABILITIES UNLOCKED:");
        Debug.Log($"  🗡️  Dual Slash - Basic dual attack");
        Debug.Log($"  🗡️  Dual Combo - Rapid combo strikes");
        Debug.Log($"  ⚡ Final Strike - Ultimate finishing move (5 hit combo)");
        Debug.Log($"\nCombo chains reset after 3 seconds of no attacks.");
        Debug.Log($"Final Strike deals 1.8x damage!\n");
        Debug.Log($"╚════════════════════════════════════════════════╝\n");
    }
}