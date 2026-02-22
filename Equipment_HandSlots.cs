using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Equipment system with left/right hand slots.
/// Allows Swordsmen to equip dual wield swords in left hand.
/// </summary>
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public class Equipment : MonoBehaviour
{
    [System.Serializable]
    public class EquipmentItem
    {
        public string itemId;
        public string itemName;
        public string description;
        public ItemRarity rarity;
        public float damage;
        public bool isDualWieldSword;
    }

    private EquipmentItem rightHandWeapon;   // Main sword
    private EquipmentItem leftHandWeapon;    // Secondary sword (if dual wield)
    
    private List<EquipmentItem> inventory = new List<EquipmentItem>();

    public EquipmentItem RightHandWeapon => rightHandWeapon;
    public EquipmentItem LeftHandWeapon => leftHandWeapon;

    /// <summary>
    /// Equip a weapon to right hand (main sword).
    /// </summary>
    public void EquipRightHand(EquipmentItem weapon)
    {
        rightHandWeapon = weapon;
        inventory.Remove(weapon);
        Debug.Log($"⚔️ Equipped {weapon.itemName} to RIGHT hand");
    }

    /// <summary>
    /// Equip a weapon to left hand (dual wield sword).
    /// </summary>
    public bool EquipLeftHand(EquipmentItem weapon)
    {
        // Check if it's a dual wield sword
        if (!weapon.isDualWieldSword)
        {
            Debug.LogWarning($"❌ {weapon.itemName} is not a dual wield sword!");
            return false;
        }

        leftHandWeapon = weapon;
        inventory.Remove(weapon);
        
        Debug.Log($"✨ Equipped {weapon.itemName} to LEFT hand");
        Debug.Log($"🎉 DUAL WIELD ACTIVATED!");
        
        return true;
    }

    /// <summary>
    /// Unequip left hand weapon.
    /// </summary>
    public void UnequipLeftHand()
    {
        if (leftHandWeapon != null)
        {
            inventory.Add(leftHandWeapon);
            Debug.Log($"Unequipped {leftHandWeapon.itemName}");
            leftHandWeapon = null;
        }
    }

    /// <summary>
    /// Add item to inventory.
    /// </summary>
    public void AddItemToInventory(EquipmentItem item)
    {
        inventory.Add(item);
        Debug.Log($"✓ Added {item.itemName} to inventory");
        
        // If it's a dual wield sword, hint at its capability
        if (item.isDualWieldSword)
        {
            Debug.Log($"   💡 {item.itemName}: \"{item.description}\"");
        }
    }

    /// <summary>
    /// Get all items in inventory.
    /// </summary>
    public List<EquipmentItem> GetInventory() => inventory;
}