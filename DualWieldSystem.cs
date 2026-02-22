using UnityEngine;

/// <summary>
/// Dual Wield System for Swordsman class.
/// Hidden feature - not advertised to players.
/// Only works when a dual wield capable sword is equipped in LEFT hand.
/// Right hand always has the main sword.
/// </summary>
public class DualWieldSystem : MonoBehaviour
{
    [SerializeField] private DualWieldWeapon rightHandWeapon;   // Main sword (always equipped)
    [SerializeField] private DualWieldWeapon leftHandWeapon;    // Secondary sword (optional - enables dual wield)

    private int currentComboHits = 0;
    private float comboResetTimer = 0f;
    private const float COMBO_RESET_TIME = 3f;

    [Header("Final Hit Bonus")]
    [SerializeField] private int hitsRequiredForFinalStrike = 5;
    [SerializeField] private float finalStrikeBonusMultiplier = 1.8f;
    
    private bool canUseFinalStrike = false;
    private bool isDualWieldActive = false;

    public DualWieldWeapon RightHandWeapon => rightHandWeapon;
    public DualWieldWeapon LeftHandWeapon => leftHandWeapon;
    public int CurrentComboHits => currentComboHits;
    public bool CanUseFinalStrike => canUseFinalStrike;
    public bool IsDualWieldActive => isDualWieldActive;

    public void Initialize()
    {
        currentComboHits = 0;
        canUseFinalStrike = false;
        isDualWieldActive = false;
        Debug.Log("Dual Wield System initialized (hidden feature)");
    }

    /// <summary>
    /// Equip a weapon to right hand (main sword).
    /// </summary>
    public bool EquipRightHandWeapon(DualWieldWeapon weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Cannot equip null weapon to right hand");
            return false;
        }

        rightHandWeapon = weapon.Clone();
        Debug.Log($"⚔️ Equipped {weapon.weaponName} to RIGHT hand (main sword)");
        return true;
    }

    /// <summary>
    /// Equip a weapon to left hand (secondary sword).
    /// This is where dual wield swords go!
    /// </summary>
    public bool EquipLeftHandWeapon(DualWieldWeapon weapon)
    {
        // Check 1: Must be dual wield capable
        if (!weapon.isDualWieldCapable)
        {
            Debug.LogWarning($"❌ {weapon.weaponName} is not a dual wield sword!");
            return false;
        }

        // Check 2: Must be a boss drop
        if (!weapon.isBossDrop)
        {
            Debug.LogWarning($"❌ Only boss-dropped swords can be dual wielded!");
            return false;
        }

        // Check 3: Must have a main sword equipped
        if (rightHandWeapon == null)
        {
            Debug.LogWarning($"❌ Must have a main sword equipped first!");
            return false;
        }

        leftHandWeapon = weapon.Clone();
        isDualWieldActive = true;
        
        Debug.Log($"✨ DUAL WIELD ACTIVATED! ✨");
        Debug.Log($"⚔️ Equipped {weapon.weaponName} to LEFT hand (dual wield sword)");
        Debug.Log($"🎉 {weapon.weaponName} is revealed to be a DUAL WIELD SWORD!");
        
        return true;
    }

    /// <summary>
    /// Unequip left hand weapon (deactivates dual wield).
    /// </summary>
    public bool UnequipLeftHandWeapon()
    {
        if (leftHandWeapon == null)
        {
            Debug.LogWarning("No left hand weapon to unequip");
            return false;
        }

        Debug.Log($"Unequipped {leftHandWeapon.weaponName} from LEFT hand");
        leftHandWeapon = null;
        isDualWieldActive = false;
        ResetCombo();
        
        return true;
    }

    /// <summary>
    /// Called when a successful attack/skill hits.
    /// Only works if dual wield is active (left hand weapon equipped).
    /// </summary>
    public void RegisterHit()
    {
        if (!isDualWieldActive || leftHandWeapon == null)
        {
            return; // Silent - dual wield not active
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
    /// Only available when dual wield is active.
    /// </summary>
    public float ExecuteFinalStrike()
    {
        if (!canUseFinalStrike || !isDualWieldActive)
        {
            Debug.LogWarning("Final strike not ready!");
            return 0f;
        }

        float baseDamage = (rightHandWeapon.baseDamage + leftHandWeapon.baseDamage) / 2f;
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
    }

    /// <summary>
    /// Get total damage (if dual wielding, combines both swords).
    /// </summary>
    public float GetDamageOutput()
    {
        if (!isDualWieldActive || leftHandWeapon == null)
            return rightHandWeapon.baseDamage;

        return rightHandWeapon.baseDamage + leftHandWeapon.baseDamage;
    }

    /// <summary>
    /// Get combined crit chance (if dual wielding).
    /// </summary>
    public float GetCombinedCritChance()
    {
        if (!isDualWieldActive || leftHandWeapon == null)
            return rightHandWeapon.critChance;

        return (rightHandWeapon.critChance + leftHandWeapon.critChance) / 2f;
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