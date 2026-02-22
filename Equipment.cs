using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Equipment and inventory system with rarity tiers.
/// </summary>
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class Equipment : MonoBehaviour
{
    [System.Serializable]
    public class EquipmentItem
    {
        public string itemId;
        public string itemName;
        public ItemRarity rarity;
        public Dictionary<string, float> statBonuses = new Dictionary<string, float>();
    }

    private Dictionary<string, EquipmentItem> equippedItems = new Dictionary<string, EquipmentItem>();
    private List<EquipmentItem> inventory = new List<EquipmentItem>();

    public void EquipItem(EquipmentItem item, string slot)
    {
        if (equippedItems.ContainsKey(slot))
        {
            inventory.Add(equippedItems[slot]);
        }

        equippedItems[slot] = item;
        inventory.Remove(item);
        Debug.Log($"Equipped {item.itemName} to {slot}");
    }

    public EquipmentItem GetEquippedItem(string slot)
    {
        return equippedItems.ContainsKey(slot) ? equippedItems[slot] : null;
    }

    public void AddItemToInventory(EquipmentItem item)
    {
        inventory.Add(item);
    }
}