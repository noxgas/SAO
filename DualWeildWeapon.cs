using UnityEngine;

/// <summary>
/// A boss-dropped weapon that can be used for dual wielding.
/// This is a hidden feature - players only discover it when they equip the sword.
/// </summary>
[System.Serializable]
public class DualWieldWeapon
{
    public string weaponId;
    public string weaponName;
    public float baseDamage;
    public float critChance;

    [Header("Dual Wield Properties")]
    public bool isDualWieldCapable;     // Can this be dual wielded (HIDDEN from UI)
    public bool isBossDrop;             // Only boss drops qualify
    public string bossThatDroppedFrom;  // Which boss dropped this

    public ItemRarity rarity;
    public bool isBoundToCharacter;

    public DualWieldWeapon Clone()
    {
        return (DualWieldWeapon)this.MemberwiseClone();
    }
}