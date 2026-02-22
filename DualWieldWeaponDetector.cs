using UnityEngine;

/// <summary>
/// Detects when a player obtains a dual wield capable weapon from a boss.
/// Automatically upgrades Swordsman to Swordmaster.
/// </summary>
public class DualWieldWeaponDetector : MonoBehaviour
{
    /// <summary>
    /// Check if a player obtains a dual wield capable weapon.
    /// Automatically converts them to Swordmaster if they're a Swordsman.
    /// </summary>
    public static void CheckAndEnableDualWield(Player player, BossDrop weaponDropped)
    {
        // Only Swordsmen can dual wield
        if (player.CharacterClass.ClassName != "Swordsman")
            return;

        // Only if the weapon is dual wield capable
        if (!weaponDropped.isDualWieldCapable)
            return;

        Debug.Log($"\n🗡️ {player.name} obtained a dual wield capable weapon: {weaponDropped.dropName}");

        // UPGRADE SWORDSMAN TO SWORDMASTER
        player.UpgradeToSwordmaster();

        // Check if player already has the dual wield system
        DualWieldSystem dualWield = player.GetComponent<DualWieldSystem>();
        if (dualWield == null)
        {
            dualWield = player.gameObject.AddComponent<DualWieldSystem>();
            dualWield.Initialize();
        }

        // Try to equip the weapon
        ConvertBossDropToDualWieldWeapon(weaponDropped, dualWield, player);
    }

    /// <summary>
    /// Convert a BossDrop to a DualWieldWeapon and equip it.
    /// </summary>
    private static void ConvertBossDropToDualWieldWeapon(BossDrop bossDrop, DualWieldSystem dualWield, Player player)
    {
        DualWieldWeapon weapon = new DualWieldWeapon
        {
            weaponId = bossDrop.dropId,
            weaponName = bossDrop.dropName,
            baseDamage = bossDrop.statBonus,
            critChance = 0.12f,
            isDualWieldCapable = true,
            isBossDrop = true,
            bossThatDroppedFrom = bossDrop.bossThatDropsIt,
            rarity = bossDrop.rarity,
            isBoundToCharacter = true
        };

        // Equip to left hand
        if (dualWield.LeftHandWeapon == null)
        {
            dualWield.EquipLeftHandWeapon(weapon);
            Debug.Log($"⚔️ Equipped {weapon.weaponName} to LEFT hand");
        }
    }
}