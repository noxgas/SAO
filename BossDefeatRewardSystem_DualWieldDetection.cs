private void AddBossDropToInventory(Player player, BossDrop drop)
{
    Equipment equipment = player.GetComponent<Equipment>();
    if (equipment == null) return;

    Equipment.EquipmentItem item = new Equipment.EquipmentItem
    {
        itemId = drop.dropId,
        itemName = drop.dropName,
        description = drop.description,
        rarity = drop.rarity,
        damage = drop.statBonus,
        isDualWieldSword = drop.isDualWieldCapable  // Mark if it's a dual wield sword
    };

    equipment.AddItemToInventory(item);
    Debug.Log($"✓ Added {drop.dropName} ({drop.rarity}) to {player.name}'s inventory");

    // If it's a dual wield sword, add a hint that it's special
    if (drop.isDualWieldCapable)
    {
        Debug.Log($"💡 [{drop.dropName}] - {drop.description}");
    }
}