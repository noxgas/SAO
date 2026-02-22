// Update the GetBaseComboSlashDamage method in ComboSystem.cs

/// <summary>
/// Get base damage per slash (scaled by current floor).
/// Floor 1: 15-25 per slash
/// Floor 10: 32-54 per slash
/// Floor 50: 77-129 per slash
/// Floor 100: 202-337 per slash
/// </summary>
private float GetBaseComboSlashDamage()
{
    float scaledMinDamage = DamageScalingSystem.Instance.GetWeaponDamageMin(currentFloor, 15f);
    float scaledMaxDamage = DamageScalingSystem.Instance.GetWeaponDamageMax(currentFloor, 25f);
    float damage = Random.Range(scaledMinDamage, scaledMaxDamage);

    return damage;
}