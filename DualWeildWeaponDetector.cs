using UnityEngine;

/// <summary>
/// Detects when a player obtains a dual wield capable weapon.
/// Automatically enables dual wield mode if conditions are met.
/// </summary>
public static class DualWieldWeaponDetector
{
    /// <summary>
    /// Check if an item is a dual wield weapon and enable dual wield if so.
    /// </summary>
    public static void CheckAndEnableDualWield(Player player, BossDrop drop)
    {
        if (!drop.isDualWieldCapable)
        {
            Debug.Log($"❌ {drop.dropName} is not dual wield capable");
            return;
        }

        Debug.Log($"⚡ DETECTED DUAL WIELD WEAPON: {drop.dropName}!");

        Equipment equipment = player.GetComponent<Equipment>();
        DualWieldSystem dualWield = player.GetComponent<DualWieldSystem>();

        if (equipment == null || dualWield == null)
        {
            Debug.LogWarning("❌ Player missing Equipment or DualWieldSystem component!");
            return;
        }

        // Create equipment item from boss drop
        Equipment.EquipmentItem dualWieldWeapon = new Equipment.EquipmentItem
        {
            itemId = drop.dropId,
            itemName = drop.dropName,
            description = drop.description,
            rarity = drop.rarity,
            damage = drop.statBonus,
            isDualWieldSword = true
        };

        // Try to equip to right hand (main hand)
        if (equipment.RightHandWeapon == null)
        {
            equipment.EquipRightHand(dualWieldWeapon);
        }
        else if (equipment.LeftHandWeapon == null)
        {
            // If right hand already has a weapon, equip to left hand
            bool equipped = equipment.EquipLeftHand(dualWieldWeapon);

            if (equipped)
            {
                Debug.Log($"✨ Dual Wield ACTIVATED with {drop.dropName}!");
                dualWield.EnableDualWield();
                LogDualWieldStatus(player, dualWieldWeapon);
            }
            else
            {
                Debug.LogWarning($"❌ Failed to equip {drop.dropName} to left hand");
            }

            return;
        }

        // If we got here, equip to main hand
        Debug.Log($"✨ Main hand equipped with {drop.dropName}!");
        LogDualWieldStatus(player, dualWieldWeapon);
    }

    /// <summary>
    /// Log the current dual wield status.
    /// </summary>
    private static void LogDualWieldStatus(Player player, Equipment.EquipmentItem weapon)
    {
        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║        ⚔️  DUAL WIELD ACTIVATED  ⚔️        ║");
        Debug.Log($"╠════════════════════════════════════════╣");
        Debug.Log($"║ Player: {player.name,32} ║");
        Debug.Log($"║ Weapon: {weapon.itemName,31} ║");
        Debug.Log($"║ Rarity: {weapon.rarity,31} ║");
        Debug.Log($"║ Damage: {weapon.damage,31:F1} ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");
    }

    /// <summary>
    /// Check if player has dual wield weapons equipped.
    /// </summary>
    public static bool HasDualWieldWeapons(Player player)
    {
        Equipment equipment = player.GetComponent<Equipment>();
        if (equipment == null) return false;

        bool hasMainHand = equipment.RightHandWeapon != null && equipment.RightHandWeapon.isDualWieldSword;
        bool hasOffHand = equipment.LeftHandWeapon != null && equipment.LeftHandWeapon.isDualWieldSword;

        return hasMainHand && hasOffHand;
    }
}