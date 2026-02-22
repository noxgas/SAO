using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dual Wield System for Swordsman class.
/// ONLY accepts boss-dropped weapons that are flagged as dual wield capable.
/// Regular dungeon drops cannot be used for dual wielding.
/// </summary>
public class DualWieldSystem : MonoBehaviour
{
    [SerializeField] private DualWieldWeapon leftWeapon;
    [SerializeField] private DualWieldWeapon rightWeapon;

    private int currentComboHits = 0;
    private float comboResetTimer = 0f;
    private const float COMBO_RESET_TIME = 3f; // Combo resets after 3 seconds of no hits

    [Header("Final Hit Bonus")]
    [SerializeField] private int hitsRequiredForFinalStrike = 5;
    [SerializeField] private float finalStrikeBonusMultiplier = 1.8f; // 80% damage bonus
    
    private bool canUseFinalStrike = false;

    public DualWieldWeapon LeftWeapon => leftWeapon;
    public DualWieldWeapon RightWeapon => rightWeapon;
    public int CurrentComboHits => currentComboHits;
    public bool CanUseFinalStrike => canUseFinalStrike;

    public void Initialize()
    {
        currentComboHits = 0;
        canUseFinalStrike = false;
        Debug.Log("Dual Wield System initialized. Equip two boss-dropped dual wield weapons!");
    }

    /// <summary>
    /// Equip a weapon to the specified hand.
    /// ONLY boss-dropped dual wield capable weapons can be equipped.
    /// </summary>
    public bool EquipWeapon(DualWieldWeapon weapon, bool isRightHand)
    {
        // Check 1: Must be flagged as dual wield capable
        if (!weapon.isDualWieldCapable)
        {
            Debug.LogWarning($"❌ {weapon.weaponName} cannot be dual wielded!");
            return false;
        }

        // Check 2: Must be a boss drop
        if (!weapon.isBossDrop)
        {
            Debug.LogWarning($"❌ {weapon.weaponName} must be a boss drop to dual wield!");
            return false;
        }

        // Check 3: Must be the exact same weapon or compatible pair
        if (isRightHand)
        {
            // If left weapon exists, check compatibility
            if (leftWeapon != null && leftWeapon.weaponId != weapon.weaponId)
            {
                Debug.LogWarning($"⚠️ Weapons must be the same type for dual wielding!");
                return false;
            }

            rightWeapon = weapon.Clone();
            Debug.Log($"✓ Equipped {weapon.weaponName} to RIGHT hand");
        }
        else
        {
            // If right weapon exists, check compatibility
            if (rightWeapon != null && rightWeapon.weaponId != weapon.weaponId)
            {
                Debug.LogWarning($"⚠️ Weapons must be the same type for dual wielding!");
                return false;
            }

            leftWeapon = weapon.Clone();
            Debug.Log($"✓ Equipped {weapon.weaponName} to LEFT hand");
        }

        return true;
    }

    /// <summary>
    /// Check if both weapons are equipped and are boss-dropped dual wield weapons.
    /// </summary>
    public bool HasBothWeaponsEquipped()
    {
        return leftWeapon != null && rightWeapon != null && 
               leftWeapon.isDualWieldCapable && rightWeapon.isDualWieldCapable &&
               leftWeapon.isBossDrop && rightWeapon.isBossDrop &&
               leftWeapon.weaponId == rightWeapon.weaponId; // Same weapon type
    }

    /// <summary>
    /// Called when a successful attack/skill hits.
    /// Increments combo counter and checks for final strike readiness.
    /// </summary>
    public void RegisterHit()
    {
        if (!HasBothWeaponsEquipped())
        {
            Debug.LogWarning("Cannot register hit - both boss-dropped dual wield weapons not equipped!");
            return;
        }

        currentComboHits++;
        comboResetTimer = COMBO_RESET_TIME;

        Debug.Log($"🗡️ Combo Hit: {currentComboHits}/{hitsRequiredForFinalStrike}");

        // Check if final strike is ready
        if (currentComboHits >= hitsRequiredForFinalStrike)
        {
            canUseFinalStrike = true;
            Debug.Log("⚡ FINAL STRIKE READY!");
        }
    }

    /// <summary>
    /// Execute the final strike with bonus damage.
    /// Resets combo counter.
    /// </summary>
    public float ExecuteFinalStrike()
    {
        if (!canUseFinalStrike)
        {
            Debug.LogWarning("Final strike not ready!");
            return 0f;
        }

        float baseDamage = (leftWeapon.baseDamage + rightWeapon.baseDamage) / 2f;
        float finalStrikeDamage = baseDamage * finalStrikeBonusMultiplier;

        Debug.Log($"⚡ FINAL STRIKE! Damage: {finalStrikeDamage}");

        ResetCombo();
        return finalStrikeDamage;
    }

    /// <summary>
    /// Reset the combo counter and final strike status.
    /// </summary>
    public void ResetCombo()
    {
        currentComboHits = 0;
        canUseFinalStrike = false;
        comboResetTimer = 0f;
        Debug.Log("Combo reset!");
    }

    /// <summary>
    /// Get total damage from both weapons (before multipliers).
    /// </summary>
    public float GetCombinedWeaponDamage()
    {
        if (!HasBothWeaponsEquipped())
            return 0f;

        return leftWeapon.baseDamage + rightWeapon.baseDamage;
    }

    /// <summary>
    /// Get the average crit chance of both weapons.
    /// </summary>
    public float GetCombinedCritChance()
    {
        if (!HasBothWeaponsEquipped())
            return 0f;

        return (leftWeapon.critChance + rightWeapon.critChance) / 2f;
    }

    private void Update()
    {
        // Update combo reset timer
        if (comboResetTimer > 0)
        {
            comboResetTimer -= Time.deltaTime;
            
            if (comboResetTimer <= 0)
            {
                ResetCombo();
            }
        }
    }
}