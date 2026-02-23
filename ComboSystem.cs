#if STEAMVR_PRESENT
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

/// <summary>
/// Combo system for Swordsman classes.
/// Hold trigger to perform slashes in sequence.
/// Reward precision and speed with damage multipliers.
/// 
/// Damage Multipliers:
/// 2-Hit: Sloppy 20% | Precise 50%
/// 3-Hit: Sloppy 40% | Precise 100%
/// 4-Hit: Sloppy 60% | Precise 150%
/// 5-Hit: Sloppy 80% | Precise 200%
/// 
/// Damage Scaling:
/// Floor 1: Base 30-50 per swing
/// Floor 50: ~150-250 per swing
/// Floor 100: ~300-500+ per swing
/// </summary>
public class ComboSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    [SerializeField] private float maxComboWindow = 1.5f; // Time between slashes
    [SerializeField] private float comboTimeoutDuration = 2f; // Reset combo after this time
    
    [Header("Precision Settings")]
    [SerializeField] private float precisionThresholdDistance = 0.3f; // How close slashes need to be
    [SerializeField] private float precisionThresholdTiming = 0.2f; // Timing window for precision
    
    [Header("Damage Scaling")]
    [SerializeField] private float baseMinDamage = 30f; // Floor 1 minimum
    [SerializeField] private float baseMaxDamage = 50f; // Floor 1 maximum
    [SerializeField] private float damageScalePerFloor = 1.08f; // 8% increase per floor
    
    [Header("References")]
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private SteamVR_Input_Sources controllerHand = SteamVR_Input_Sources.RightHand;

    private Player player;
    private Swordsman swordsmanClass;
    private List<SlashData> currentCombo = new List<SlashData>();
    private float comboTimeout;
    private bool isComboActive = false;
    private Vector3 lastSlashPosition;
    private float lastSlashTime;
    private int currentFloor = 1;

    [System.Serializable]
    private class SlashData
    {
        public int slashNumber;
        public Vector3 position;
        public float timestamp;
        public bool isPrecise;
    }

    private ComboPreset[] comboPresets;

    [System.Serializable]
    public class ComboPreset
    {
        public int hits;
        public float sloppyDamageMultiplier;
        public float preciseDamageMultiplier;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        playerCombat = GetComponent<PlayerCombat>();
        if (rightHandTransform == null)
            rightHandTransform = transform;

        InitializeComboPresets();
    }

    private void InitializeComboPresets()
    {
        comboPresets = new ComboPreset[]
        {
            new ComboPreset { hits = 2, sloppyDamageMultiplier = 0.2f, preciseDamageMultiplier = 0.5f },
            new ComboPreset { hits = 3, sloppyDamageMultiplier = 0.4f, preciseDamageMultiplier = 1.0f },
            new ComboPreset { hits = 4, sloppyDamageMultiplier = 0.6f, preciseDamageMultiplier = 1.5f },
            new ComboPreset { hits = 5, sloppyDamageMultiplier = 0.8f, preciseDamageMultiplier = 2.0f }
        };
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = Mathf.Max(1, floor);
    }

    /// <summary>
    /// Calculate base damage for current floor.
    /// Scales exponentially with floor level.
    /// Floor 1: 30-50
    /// Floor 10: ~65-108
    /// Floor 50: ~155-258
    /// Floor 100: ~405-675
    /// </summary>
    private float GetFloorScaledDamage(float baseDamage)
    {
        float floorMultiplier = Mathf.Pow(damageScalePerFloor, currentFloor - 1);
        return baseDamage * floorMultiplier;
    }

    private void Update()
    {
        if (playerCombat == null || player == null)
            return;

        // Check if player is Swordsman class
        if (player.CharacterClass.ClassName != "Swordsman")
            return;

        HandleComboInput();
        UpdateComboTimeout();
    }

    private void HandleComboInput()
    {
        // Trigger held - start/continue combo
        if (SteamVR_Input.GetState("InteractUI", controllerHand))
        {
            if (!isComboActive)
            {
                StartCombo();
            }
        }
        // Trigger released - register slash
        else if (SteamVR_Input.GetStateUp("InteractUI", controllerHand))
        {
            RegisterSlash();
        }
    }

    private void StartCombo()
    {
        if (isComboActive)
            return;

        isComboActive = true;
        currentCombo.Clear();
        comboTimeout = comboTimeoutDuration;

        Debug.Log("🗡️ Combo started - hold trigger to perform slashes");
    }

    private void RegisterSlash()
    {
        if (!isComboActive)
            return;

        int slashNumber = currentCombo.Count + 1;
        
        // Check if this is a valid combo continuation
        if (slashNumber > 1)
        {
            float timeSinceLastSlash = Time.time - lastSlashTime;
            
            // Check if slash is within combo window
            if (timeSinceLastSlash > maxComboWindow)
            {
                Debug.LogWarning($"❌ Combo broken! Slash took {timeSinceLastSlash}s (max: {maxComboWindow}s)");
                EndCombo(false);
                return;
            }
        }

        // Get current hand position
        Vector3 currentPosition = rightHandTransform.position;
        float currentTime = Time.time;

        // Determine if this slash is precise
        bool isPrecise = IsPreciseSlash(slashNumber, currentPosition, currentTime);

        // Create slash data
        SlashData slash = new SlashData
        {
            slashNumber = slashNumber,
            position = currentPosition,
            timestamp = currentTime,
            isPrecise = isPrecise
        };

        currentCombo.Add(slash);
        lastSlashPosition = currentPosition;
        lastSlashTime = currentTime;

        Debug.Log($"✓ Slash {slashNumber} registered - {(isPrecise ? "PRECISE" : "sloppy")}");

        // Check if combo is complete (max 5 hits)
        if (slashNumber >= 5)
        {
            Debug.Log("🎉 5-Hit Combo Complete!");
            EndCombo(true);
        }
        else
        {
            comboTimeout = comboTimeoutDuration;
        }
    }

    private bool IsPreciseSlash(int slashNumber, Vector3 currentPosition, float currentTime)
    {
        if (slashNumber == 1)
            return true; // First slash is always considered precise

        // Check timing precision
        float timeSinceLastSlash = currentTime - lastSlashTime;
        float expectedTiming = maxComboWindow / 3f; // Optimal timing
        float timingDeviation = Mathf.Abs(timeSinceLastSlash - expectedTiming);
        bool isTimingPrecise = timingDeviation <= precisionThresholdTiming;

        // Check spatial precision (slashes in similar area)
        float distanceFromLastSlash = Vector3.Distance(currentPosition, lastSlashPosition);
        bool isPositionPrecise = distanceFromLastSlash <= precisionThresholdDistance;

        bool isPrecise = isTimingPrecise && isPositionPrecise;

        Debug.Log($"  Timing: {timeSinceLastSlash:F2}s (deviation: {timingDeviation:F2}s) - {(isTimingPrecise ? "✓" : "✗")}");
        Debug.Log($"  Distance: {distanceFromLastSlash:F2}m - {(isPositionPrecise ? "✓" : "✗")}");

        return isPrecise;
    }

    private void UpdateComboTimeout()
    {
        if (!isComboActive)
            return;

        comboTimeout -= Time.deltaTime;

        if (comboTimeout <= 0)
        {
            Debug.Log("⏱️ Combo timeout - sequence completed");
            EndCombo(true);
        }
    }

    private void EndCombo(bool executeCombo)
    {
        if (!isComboActive)
            return;

        isComboActive = false;

        if (currentCombo.Count == 0)
            return;

        if (executeCombo)
        {
            ExecuteCombo();
        }
        else
        {
            Debug.Log("❌ Combo failed");
        }

        currentCombo.Clear();
    }

    private void ExecuteCombo()
    {
        int comboHits = currentCombo.Count;
        
        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║         COMBO EXECUTED: {comboHits}-HIT          ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");

        // Count precise slashes
        int preciseCount = 0;
        foreach (var slash in currentCombo)
        {
            if (slash.isPrecise)
                preciseCount++;
        }

        bool isOverallPrecise = preciseCount >= (comboHits * 0.7f); // 70% precise = overall precise

        // Get damage multiplier
        float damageMultiplier = GetComboMultiplier(comboHits, isOverallPrecise);

        // Get base damage (scaled by floor)
        float baseDamage = GetBaseComboSlashDamage();
        float totalDamage = baseDamage * (1f + damageMultiplier);

        // Display results
        Debug.Log($"Floor: {currentFloor}");
        Debug.Log($"Base Damage: {baseDamage:F1}");
        Debug.Log($"Precision: {preciseCount}/{comboHits} slashes precise");
        Debug.Log($"Multiplier: {(isOverallPrecise ? "PRECISE" : "SLOPPY")} - {damageMultiplier * 100f:F0}% bonus");
        Debug.Log($"Total Damage: {totalDamage:F1}");

        // Apply damage to nearby enemies
        ApplyComboDamage(totalDamage, comboHits);

        // Reward player
        RewardCombo(comboHits, isOverallPrecise);
    }

    private float GetComboMultiplier(int hits, bool isPrecise)
    {
        foreach (var preset in comboPresets)
        {
            if (preset.hits == hits)
            {
                return isPrecise ? preset.preciseDamageMultiplier : preset.sloppyDamageMultiplier;
            }
        }

        return 0f;
    }

    /// <summary>
    /// Get base damage per slash (scaled by floor).
    /// Floor 1: 15-25
    /// Floor 10: ~32-54
    /// Floor 50: ~77-129
    /// Floor 100: ~202-337
    /// </summary>
    private float GetBaseComboSlashDamage()
    {
        float scaledMinDamage = GetFloorScaledDamage(baseMinDamage);
        float scaledMaxDamage = GetFloorScaledDamage(baseMaxDamage);
        float damage = Random.Range(scaledMinDamage, scaledMaxDamage);

        return damage;
    }

    private void ApplyComboDamage(float damage, int hits)
    {
        // Find all enemies in range
        Collider[] enemies = Physics.OverlapSphere(transform.position, 15f);

        int hitCount = 0;
        foreach (var enemyCollider in enemies)
        {
            AIEnemy enemy = enemyCollider.GetComponent<AIEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hitCount++;
                Debug.Log($"💥 Enemy hit for {damage:F1} damage");
            }
        }

        if (hitCount == 0)
        {
            Debug.Log("⚠️ No enemies hit by combo");
        }
    }

    private void RewardCombo(int hits, bool isPrecise)
    {
        // Bonus experience for precise combos (scales with floor)
        int baseXP = hits * 100;
        int floorBonus = (currentFloor - 1) * 50; // 50 XP per floor
        int bonusXP = baseXP + floorBonus;

        if (isPrecise)
            bonusXP = (int)(bonusXP * 1.5f); // 50% bonus for precise

        player.GainExperience(bonusXP);

        // VFX/SFX feedback
        DisplayComboFeedback(hits, isPrecise);
    }

    private void DisplayComboFeedback(int hits, bool isPrecise)
    {
        string precisionText = isPrecise ? "✨ PRECISE" : "HIT";
        string hitText = hits switch
        {
            2 => "Double Strike",
            3 => "Triple Strike",
            4 => "Quad Strike",
            5 => "Ultimate Combo",
            _ => "Unknown"
        };

        Debug.Log($"\n🎉 {hitText} - {precisionText} 🎉\n");
    }

    /// <summary>
    /// Get current combo status (for UI display).
    /// </summary>
    public int GetCurrentComboCount()
    {
        return currentCombo.Count;
    }

    public bool IsComboActive()
    {
        return isComboActive;
    }

    public float GetComboProgress()
    {
        return 1f - (comboTimeout / comboTimeoutDuration);
    }

    public float GetFloorScaledBaseDamage()
    {
        return GetFloorScaledDamage(baseMinDamage);
    }
}
#endif // STEAMVR_PRESENT
