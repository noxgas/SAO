using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Database of dual-wield compatible weapons that drop from dungeons.
/// </summary>
public static class DualWieldWeaponDatabase
{
    private static Dictionary<string, DualWieldSystem.DualWieldWeapon> weaponDatabase = 
        new Dictionary<string, DualWieldSystem.DualWieldWeapon>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;

        // Common Dual Wield Weapons (Starter)
        weaponDatabase["iron_short_sword"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "iron_short_sword",
            weaponName = "Iron Short Sword",
            baseDamage = 12f,
            critChance = 0.08f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Common
        };

        weaponDatabase["steel_short_sword"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "steel_short_sword",
            weaponName = "Steel Short Sword",
            baseDamage = 15f,
            critChance = 0.10f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Uncommon
        };

        // Uncommon Dual Wield Weapons (Floor 1-10)
        weaponDatabase["mithril_blade"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "mithril_blade",
            weaponName = "Mithril Blade",
            baseDamage = 18f,
            critChance = 0.12f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Uncommon
        };

        weaponDatabase["wind_cutter"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "wind_cutter",
            weaponName = "Wind Cutter",
            baseDamage = 16f,
            critChance = 0.15f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Uncommon
        };

        // Rare Dual Wield Weapons (Floor 10-30)
        weaponDatabase["flame_fang"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "flame_fang",
            weaponName = "Flame Fang",
            baseDamage = 22f,
            critChance = 0.16f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Rare
        };

        weaponDatabase["frost_edge"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "frost_edge",
            weaponName = "Frost Edge",
            baseDamage = 20f,
            critChance = 0.18f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Rare
        };

        weaponDatabase["shadow_blade"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "shadow_blade",
            weaponName = "Shadow Blade",
            baseDamage = 19f,
            critChance = 0.22f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Rare
        };

        // Epic Dual Wield Weapons (Floor 30-60)
        weaponDatabase["excalibur_twin"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "excalibur_twin",
            weaponName = "Excalibur Twin",
            baseDamage = 28f,
            critChance = 0.20f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Epic
        };

        weaponDatabase["void_ripper"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "void_ripper",
            weaponName = "Void Ripper",
            baseDamage = 26f,
            critChance = 0.25f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Epic
        };

        // Legendary Dual Wield Weapons (Floor 60-100)
        weaponDatabase["celestial_twin_blades"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "celestial_twin_blades",
            weaponName = "Celestial Twin Blades",
            baseDamage = 35f,
            critChance = 0.28f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Legendary
        };

        weaponDatabase["demon_slayer_pair"] = new DualWieldSystem.DualWieldWeapon
        {
            weaponId = "demon_slayer_pair",
            weaponName = "Demon Slayer Pair",
            baseDamage = 33f,
            critChance = 0.30f,
            isDualWieldCompatible = true,
            rarity = ItemRarity.Legendary
        };

        initialized = true;
    }

    public static DualWieldSystem.DualWieldWeapon GetWeapon(string weaponId)
    {
        Initialize();
        return weaponDatabase.ContainsKey(weaponId) ? weaponDatabase[weaponId] : null;
    }

    public static List<DualWieldSystem.DualWieldWeapon> GetWeaponsByRarity(ItemRarity rarity)
    {
        Initialize();
        List<DualWieldSystem.DualWieldWeapon> result = new List<DualWieldSystem.DualWieldWeapon>();
        
        foreach (var weapon in weaponDatabase.Values)
        {
            if (weapon.rarity == rarity)
                result.Add(weapon);
        }
        
        return result;
    }

    public static List<DualWieldSystem.DualWieldWeapon> GetAllDualWieldWeapons()
    {
        Initialize();
        return new List<DualWieldSystem.DualWieldWeapon>(weaponDatabase.Values);
    }
}