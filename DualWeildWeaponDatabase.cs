using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Database of dual wield weapons available from bosses.
/// </summary>
public static class DualWieldWeaponDatabase
{
    private static Dictionary<string, DualWieldWeapon> weaponDatabase = new Dictionary<string, DualWieldWeapon>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;

        weaponDatabase["twin_iron_blades_boss"] = new DualWieldWeapon
        {
            weaponId = "twin_iron_blades_boss",
            weaponName = "Twin Iron Blades",
            baseDamage = 14f,
            critChance = 0.12f,
            isDualWieldCapable = true,
            isBossDrop = true,
            bossThatDroppedFrom = "goblin_king",
            rarity = ItemRarity.Rare,
            isBoundToCharacter = true
        };

        weaponDatabase["frost_edge_twin_boss"] = new DualWieldWeapon
        {
            weaponId = "frost_edge_twin_boss",
            weaponName = "Twin Frost Edges",
            baseDamage = 20f,
            critChance = 0.15f,
            isDualWieldCapable = true,
            isBossDrop = true,
            bossThatDroppedFrom = "frost_dragon",
            rarity = ItemRarity.Epic,
            isBoundToCharacter = true
        };

        weaponDatabase["shadow_twin_blades_boss"] = new DualWieldWeapon
        {
            weaponId = "shadow_twin_blades_boss",
            weaponName = "Twin Shadow Blades",
            baseDamage = 26f,
            critChance = 0.18f,
            isDualWieldCapable = true,
            isBossDrop = true,
            bossThatDroppedFrom = "shadow_monarch",
            rarity = ItemRarity.Legendary,
            isBoundToCharacter = true
        };

        initialized = true;
    }

    public static DualWieldWeapon GetWeapon(string weaponId)
    {
        Initialize();
        if (weaponDatabase.ContainsKey(weaponId))
            return weaponDatabase[weaponId].Clone();

        Debug.LogWarning($"Dual wield weapon not found: {weaponId}");
        return null;
    }

    public static List<DualWieldWeapon> GetAllWeapons()
    {
        Initialize();
        return new List<DualWieldWeapon>(weaponDatabase.Values);
    }
}