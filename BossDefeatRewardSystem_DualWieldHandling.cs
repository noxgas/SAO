// Add this to the ApplyRewardToPlayer method:

private void ApplyRewardToPlayer(Player player, DefeatReward reward)
{
    // Award experience
    player.GainExperience(reward.experienceEarned);

    // Add loot to inventory
    Equipment equipment = player.GetComponent<Equipment>();
    if (equipment != null)
    {
        foreach (var drop in reward.earnedLoot)
        {
            // Check if this is a dual wield unlock item
            if (drop.isDualWieldUnlock && player.CharacterClass.ClassName == "Swordsman")
            {
                // Apply dual wield unlock immediately
                DualWieldUnlockSystem.Instance.UnlockDualWield(player);
            }
            else
            {
                AddBossDropToInventory(player, drop);
            }
        }
    }
}