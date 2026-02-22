using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Equipment and inventory system.
/// Manages equipped items and inventory slots.
/// </summary>
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

    private EquipmentItem rightHandWeapon;
    private EquipmentItem leftHandWeapon;
    private List<EquipmentItem> inventory = new List<EquipmentItem>();

    public EquipmentItem RightHandWeapon => rightHandWeapon;
    public EquipmentItem LeftHandWeapon => leftHandWeapon;

    public void EquipRightHand(EquipmentItem weapon)
    {
        rightHandWeapon = weapon;
        inventory.Remove(weapon);
        Debug.Log($"⚔️ Equipped {weapon.itemName} to RIGHT hand");
    }

    public bool EquipLeftHand(EquipmentItem weapon)
    {
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

    public void UnequipLeftHand()
    {
        if (leftHandWeapon != null)
        {
            inventory.Add(leftHandWeapon);
            Debug.Log($"Unequipped {leftHandWeapon.itemName}");
            leftHandWeapon = null;
        }
    }

    public void AddItemToInventory(EquipmentItem item)
    {
        inventory.Add(item);
        Debug.Log($"✓ Added {item.itemName} to inventory");
    }

    public List<EquipmentItem> GetInventory() => inventory;
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}