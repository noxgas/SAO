using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dual Wield System for players with dual wield capable weapons.
/// Handles combo attacks and synchronized blade strikes.
/// </summary>
public class DualWieldSystem : MonoBehaviour
{
    [Header("Dual Wield Settings")]
    [SerializeField] private bool isDualWieldActive = false;
    [SerializeField] private float comboWindowTime = 1.5f;
    [SerializeField] private float synchronizationBonus = 1.2f;

    private Equipment equipment;
    private PlayerCombat playerCombat;
    private ComboSystem comboSystem;
    private Player player;

    private bool isComboActive = false;
    private float comboTimer = 0f;
    private int currentComboCount = 0;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        equipment = GetComponent<Equipment>();
        playerCombat = GetComponent<PlayerCombat>();
        comboSystem = GetComponent<ComboSystem>();
        player = GetComponent<Player>();
    }

    /// <summary>
    /// Initialize the dual wield system.
    /// </summary>
    public void Initialize(Player playerRef)
    {
        player = playerRef;
        equipment = GetComponent<Equipment>();
        playerCombat = GetComponent<PlayerCombat>();
        comboSystem = GetComponent<ComboSystem>();

        Debug.Log("✓ Dual Wield System initialized");
    }

    private void Update()
    {
        if (isDualWieldActive)
        {
            UpdateComboTimer();
        }
    }

    /// <summary>
    /// Enable dual wield mode.
    /// </summary>
    public void EnableDualWield()
    {
        if (equipment == null)
        {
            Debug.LogWarning("❌ Equipment component not found!");
            return;
        }

        if (equipment.RightHandWeapon == null || equipment.LeftHandWeapon == null)
        {
            Debug.LogWarning("❌ Both hands must have weapons equipped for dual wield!");
            return;
        }

        if (!equipment.RightHandWeapon.isDualWieldSword || !equipment.LeftHandWeapon.isDualWieldSword)
        {
            Debug.LogWarning("❌ Both weapons must be dual wield capable!");
            return;
        }

        isDualWieldActive = true;
        Debug.Log($"✨ DUAL WIELD ENABLED!");
        Debug.Log($"Left: {equipment.LeftHandWeapon.itemName}");
        Debug.Log($"Right: {equipment.RightHandWeapon.itemName}");
    }

    /// <summary>
    /// Disable dual wield mode.
    /// </summary>
    public void DisableDualWield()
    {
        isDualWieldActive = false;
        isComboActive = false;
        currentComboCount = 0;
        Debug.Log("⚔️ Dual wield disabled");
    }

    /// <summary>
    /// Start a combo attack.
    /// </summary>
    public void StartCombo()
    {
        if (!isDualWieldActive)
        {
            Debug.LogWarning("❌ Dual wield not active!");
            return;
        }

        isComboActive = true;
        currentComboCount = 0;
        comboTimer = comboWindowTime;

        Debug.Log("⚡ COMBO STARTED!");
    }

    /// <summary>
    /// Register a strike in the combo.
    /// </summary>
    public bool RegisterStrike()
    {
        if (!isComboActive)
        {
            StartCombo();
            return true;
        }

        if (comboTimer <= 0)
        {
            Debug.Log("❌ Combo window expired!");
            EndCombo();
            return false;
        }

        currentComboCount++;
        comboTimer = comboWindowTime;
        lastAttackTime = Time.time;

        float damageMod = 1f + (currentComboCount * 0.1f) * synchronizationBonus;
        Debug.Log($"⚡ STRIKE {currentComboCount} | Damage Mod: {damageMod:F2}x");

        if (currentComboCount >= 5)
        {
            ExecuteMaxCombo();
            return true;
        }

        return true;
    }

    /// <summary>
    /// Update combo timer.
    /// </summary>
    private void UpdateComboTimer()
    {
        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0)
            {
                EndCombo();
            }
        }
    }

    /// <summary>
    /// Execute maximum combo (5 strikes).
    /// </summary>
    private void ExecuteMaxCombo()
    {
        Debug.Log("\n╔════════════════════════════════════════╗");
        Debug.Log($"║         🎉 5-HIT COMBO! 🎉              ║");
        Debug.Log($"║  Damage Multiplier: {(1f + (currentComboCount * 0.1f) * synchronizationBonus):F2}x       ║");
        Debug.Log("╚════════════════════════════════════════╝\n");

        // Apply bonus damage
        if (playerCombat != null)
        {
            float bonusDamage = equipment.RightHandWeapon.damage + equipment.LeftHandWeapon.damage;
            bonusDamage *= (1f + (currentComboCount * 0.1f) * synchronizationBonus);
            // Apply bonus damage to enemy here
        }

        EndCombo();
    }

    /// <summary>
    /// End the combo.
    /// </summary>
    private void EndCombo()
    {
        if (currentComboCount > 0)
        {
            Debug.Log($"✓ Combo ended at {currentComboCount} strikes");
        }

        isComboActive = false;
        currentComboCount = 0;
        comboTimer = 0f;
    }

    public bool IsDualWieldActive() => isDualWieldActive;
    public bool IsComboActive() => isComboActive;
    public int GetCurrentComboCount() => currentComboCount;
    public float GetComboProgress() => comboTimer / comboWindowTime;
    public Equipment.EquipmentItem GetLeftWeapon() => equipment?.LeftHandWeapon;
    public Equipment.EquipmentItem GetRightWeapon() => equipment?.RightHandWeapon;
}