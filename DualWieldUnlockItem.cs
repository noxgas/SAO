using UnityEngine;

/// <summary>
/// The ultra-rare item that unlocks dual wielding for Swordsmen.
/// Drop chance: < 1% (only from high-level bosses)
/// Bound to character once obtained.
/// </summary>
[System.Serializable]
public class DualWieldUnlockItem : MonoBehaviour
{
    public string itemId = "dual_wield_unlock";
    public string itemName = "Dual Wield Codex";
    public string description = "An ancient codex that teaches the art of dual wielding. " +
                               "Only Swordsmen can learn this legendary technique.";
    
    public ItemRarity rarity = ItemRarity.Legendary;
    public bool isBoundToCharacter = true;
    public float dropChance = 0.005f; // 0.5% drop chance (less than 1%)
    public int requiredPlayerLevel = 20; // Can only be obtained after level 20
    
    /// <summary>
    /// Apply dual wield unlock to a Swordsman player.
    /// </summary>
    public void UnlockDualWield(Player player)
    {
        // Check if player is Swordsman
        if (player.CharacterClass.ClassName != "Swordsman")
        {
            Debug.LogError($"Only Swordsmen can unlock dual wielding! {player.name} is {player.CharacterClass.ClassName}");
            return;
        }

        // Check if player is high enough level
        if (player.Level < requiredPlayerLevel)
        {
            Debug.LogWarning($"Player must be level {requiredPlayerLevel} or higher to unlock dual wield!");
            return;
        }

        Debug.Log($"⚡ {player.name} has unlocked DUAL WIELD!");
        
        // Reinitialize player as Dual Wield Swordsman
        player.InitializePlayer("Swordsman", "DualWield");
        
        // Show special UI/notification
        OnDualWieldUnlocked(player);
    }

    /// <summary>
    /// Called when dual wield is successfully unlocked.
    /// </summary>
    private void OnDualWieldUnlocked(Player player)
    {
        Debug.Log($"🎉 === LEGENDARY UNLOCK === 🎉");
        Debug.Log($"{player.name} has become a DUAL WIELD MASTER!");
        Debug.Log($"New skills unlocked:");
        Debug.Log($"  - Dual Slash");
        Debug.Log($"  - Dual Combo");
        Debug.Log($"  - Final Strike");
        // Here you'd trigger UI notifications, achievements, etc.
    }
}