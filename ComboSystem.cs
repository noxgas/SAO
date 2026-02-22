using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Advanced combo system with full game integration.
/// Handles: dual wield, boss mechanics, floor scaling, skill trees, guilds, and VR input.
/// </summary>
public class ComboSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    [SerializeField] private float comboWindow = 2f;
    [SerializeField] private float comboResetDelay = 3f;
    [SerializeField] private int maxComboCount = 10;
    [SerializeField] private float damagePerComboHit = 1.1f;
    [SerializeField] private float criticalHitChance = 0.15f;
    [SerializeField] private float criticalHitMultiplier = 1.5f;

    [Header("Floor Scaling")]
    [SerializeField] private int currentFloor = 1;
    [SerializeField] private float floorDamageMultiplier = 1.2f;
    [SerializeField] private float floorHealthScaling = 1.1f;

    [Header("Dual Wield Bonuses")]
    [SerializeField] private float dualWieldComboBonus = 0.5f;
    [SerializeField] private float dualWieldCritBonus = 0.05f;
    [SerializeField] private float dualWieldSpeedBonus = 1.2f;

    [Header("Boss Mechanics")]
    [SerializeField] private float bossComboThreshold = 5f;
    [SerializeField] private float bossComboRewardMultiplier = 2f;

    [Header("Guild Bonuses")]
    [SerializeField] private float guildComboBonus = 0.1f;
    [SerializeField] private float guildDamageBonus = 0.05f;

    // Combo State
    private int comboCounter = 0;
    private float comboTimer = 0f;
    private float lastHitTime = 0f;
    private bool isComboActive = false;
    private bool isBossComboActive = false;
    private int consecutiveBossHits = 0;

    // Component References
    private Player player;
    private PlayerCombat playerCombat;
    private DualWieldSystem dualWieldSystem;
    private BossEnemy currentBoss;
    private Guild playerGuild;

    // Tracking
    private List<float> comboHitTimes = new List<float>();
    private List<float> comboDamages = new List<float>();
    private List<bool> comboCrits = new List<bool>();
    private float totalComboMultiplier = 1f;
    private float totalComboDamage = 0f;
    private int totalComboCrits = 0;

    // Performance Metrics
    private float bestComboMultiplier = 1f;
    private int longestComboStreak = 0;
    private float totalComboXpEarned = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerCombat = GetComponent<PlayerCombat>();
        dualWieldSystem = GetComponent<DualWieldSystem>();
    }

    private void Update()
    {
        UpdateComboTimer();
        MonitorBossCombo();
    }

    /// <summary>
    /// Set the current floor level for damage scaling.
    /// </summary>
    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
        Debug.Log($"🏢 Floor {floor} - Damage: {GetFloorDamageMultiplier():F2}x");
    }

    public int GetCurrentFloor() => currentFloor;

    /// <summary>
    /// Set the current boss being fought.
    /// </summary>
    public void SetCurrentBoss(BossEnemy boss)
    {
        currentBoss = boss;
        consecutiveBossHits = 0;
        isBossComboActive = false;
        Debug.Log($"⚔️ Boss set: {boss.gameObject.name}");
    }

    /// <summary>
    /// Set player guild for guild bonuses.
    /// </summary>
    public void SetPlayerGuild(Guild guild)
    {
        playerGuild = guild;
        if (guild != null)
            Debug.Log($"Guild bonus: +{(guildComboBonus * 100):F0}% combo, +{(guildDamageBonus * 100):F0}% damage");
    }

    /// <summary>
    /// Start a new combo.
    /// </summary>
    public void StartCombo()
    {
        if (isComboActive)
            return;

        isComboActive = true;
        comboCounter = 0;
        comboTimer = comboWindow;
        comboHitTimes.Clear();
        comboDamages.Clear();
        comboCrits.Clear();
        totalComboMultiplier = 1f;
        totalComboDamage = 0f;
        totalComboCrits = 0;

        Debug.Log("⚡ COMBO STARTED!");
    }

    /// <summary>
    /// Register a hit in the combo with full system integration.
    /// </summary>
    public bool RegisterHit(float baseDamage, BossEnemy target = null)
    {
        if (!isComboActive)
            StartCombo();

        if (comboTimer <= 0)
        {
            Debug.Log("❌ Combo window expired!");
            EndCombo();
            return false;
        }

        comboCounter++;
        comboHitTimes.Add(Time.time);
        lastHitTime = Time.time;
        comboTimer = comboWindow;

        // Calculate final damage multiplier
        totalComboMultiplier = CalculateTotalMultiplier();

        // Check for critical hit
        bool isCritical = Random.value < GetCriticalChance();
        if (isCritical)
        {
            totalComboCrits++;
            totalComboMultiplier *= criticalHitMultiplier;
        }

        comboCrits.Add(isCritical);

        // Apply all multipliers
        float finalDamage = baseDamage * totalComboMultiplier;
        comboDamages.Add(finalDamage);
        totalComboDamage += finalDamage;

        // Apply damage to boss
        if (target != null)
        {
            target.TakeDamage(finalDamage, player);
            consecutiveBossHits++;
        }

        // Log hit with all details
        string critText = isCritical ? " 💥 CRIT!" : "";
        Debug.Log($"⚡ HIT #{comboCounter} | DMG: {finalDamage:F1} | Multiplier: {totalComboMultiplier:F2}x | Floor: {currentFloor}{critText}");

        // Check achievements
        if (comboCounter >= maxComboCount)
            ExecuteMaxCombo();

        if (currentBoss != null && comboCounter >= (int)bossComboThreshold)
            ActivateBossCombo();

        return true;
    }

    /// <summary>
    /// Calculate total damage multiplier from all sources.
    /// </summary>
    private float CalculateTotalMultiplier()
    {
        float multiplier = 1f;

        // Combo hits multiplier
        multiplier += (comboCounter * (damagePerComboHit - 1f));

        // Floor scaling
        multiplier *= (1f + ((currentFloor - 1) * 0.1f));

        // Dual wield bonus
        if (dualWieldSystem != null && dualWieldSystem.IsDualWieldActive())
            multiplier *= (1f + dualWieldComboBonus);

        // Guild bonus
        if (playerGuild != null)
            multiplier *= (1f + guildComboBonus);

        // Skill tree bonuses (if available)
        if (player != null && player.SkillTree != null)
            multiplier *= player.SkillTree.GetComboMultiplierBonus();

        return multiplier;
    }

    /// <summary>
    /// Get critical hit chance with all bonuses.
    /// </summary>
    private float GetCriticalChance()
    {
        float chance = criticalHitChance;

        if (dualWieldSystem != null && dualWieldSystem.IsDualWieldActive())
            chance += dualWieldCritBonus;

        if (playerGuild != null)
            chance += 0.02f; // Small guild bonus

        return Mathf.Clamp01(chance);
    }

    /// <summary>
    /// Get floor damage multiplier.
    /// </summary>
    public float GetFloorDamageMultiplier()
    {
        return 1f + ((currentFloor - 1) * 0.1f);
    }

    /// <summary>
    /// Activate special boss combo mode.
    /// </summary>
    private void ActivateBossCombo()
    {
        if (isBossComboActive)
            return;

        isBossComboActive = true;
        Debug.Log("\n╔════════════════════════════════════════╗");
        Debug.Log($"║     ⚡ BOSS COMBO ACTIVATED! ⚡         ║");
        Debug.Log($"║ Hits: {comboCounter,32} ║");
        Debug.Log($"║ Damage Multiplier: {bossComboRewardMultiplier,21:F2}x ║");
        Debug.Log("╚════════════════════════════════════════╝\n");
    }

    /// <summary>
    /// Monitor boss combo state.
    /// </summary>
    private void MonitorBossCombo()
    {
        if (isBossComboActive && currentBoss != null)
        {
            if (currentBoss.GetHealthPercent() <= 0)
            {
                ExecuteBossComboReward();
            }
        }
    }

    /// <summary>
    /// Execute boss combo reward.
    /// </summary>
    private void ExecuteBossComboReward()
    {
        if (!isBossComboActive)
            return;

        float rewardDamage = totalComboDamage * bossComboRewardMultiplier;
        float rewardXp = comboCounter * 100f;

        Debug.Log("\n╔════════════════════════════════════════╗");
        Debug.Log($"║        🎉 BOSS COMBO COMPLETE! 🎉      ║");
        Debug.Log($"║ Consecutive Hits: {consecutiveBossHits,19} ║");
        Debug.Log($"║ Total Damage: {totalComboDamage,25:F1} ║");
        Debug.Log($"║ Bonus Damage: {rewardDamage,25:F1} ║");
        Debug.Log($"║ XP Reward: {rewardXp,28:F0} ║");
        Debug.Log("╚════════════════════════════════════════╝\n");

        if (player != null)
            player.GainExperience((int)rewardXp);

        totalComboXpEarned += rewardXp;
        isBossComboActive = false;
    }

    /// <summary>
    /// Execute the maximum combo (10 hits).
    /// </summary>
    private void ExecuteMaxCombo()
    {
        Debug.Log("\n╔════════════════════════════════════════╗");
        Debug.Log($"║         🎉 MAX COMBO! ({comboCounter} HITS)       ║");
        Debug.Log($"║ Total Damage: {totalComboDamage,25:F1} ║");
        Debug.Log($"║ Multiplier: {totalComboMultiplier,28:F2}x ║");
        Debug.Log($"║ Critical Hits: {totalComboCrits,24} ║");
        Debug.Log($"║ Floor Bonus: {(currentFloor * 10),26}% ║");
        Debug.Log("╚════════════════════════════════════════╝\n");

        // Track best combo
        if (totalComboMultiplier > bestComboMultiplier)
            bestComboMultiplier = totalComboMultiplier;

        if (comboCounter > longestComboStreak)
            longestComboStreak = comboCounter;

        // Dual wield bonus damage
        if (dualWieldSystem != null && dualWieldSystem.IsDualWieldActive())
        {
            float dualWieldBonus = totalComboDamage * 0.5f;
            totalComboDamage += dualWieldBonus;
            Debug.Log($"✨ Dual Wield Bonus: +{dualWieldBonus:F1} damage!");
        }

        // Grant XP
        float comboXp = comboCounter * 50f;
        if (player != null)
            player.GainExperience((int)comboXp);

        totalComboXpEarned += comboXp;
    }

    /// <summary>
    /// Update the combo timer each frame.
    /// </summary>
    private void UpdateComboTimer()
    {
        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
                EndCombo();
        }
    }

    /// <summary>
    /// End the current combo.
    /// </summary>
    public void EndCombo()
    {
        if (comboCounter > 0)
        {
            Debug.Log($"✓ Combo ended! {comboCounter} hits, {totalComboDamage:F1} total damage, {totalComboCrits} crits");
        }

        isComboActive = false;
        isBossComboActive = false;
        comboCounter = 0;
        comboTimer = 0f;
        totalComboMultiplier = 1f;
        consecutiveBossHits = 0;
        comboHitTimes.Clear();
        comboDamages.Clear();
        comboCrits.Clear();
    }

    // Getters
    public int GetComboCount() => comboCounter;
    public float GetComboMultiplier() => totalComboMultiplier;
    public float GetComboProgress() => Mathf.Clamp01(1f - (comboTimer / comboWindow));
    public bool IsComboActive() => isComboActive;
    public bool IsBossComboActive() => isBossComboActive;
    public float GetTotalComboDamage() => totalComboDamage;
    public int GetTotalComboCrits() => totalComboCrits;
    public float GetBestComboMultiplier() => bestComboMultiplier;
    public int GetLongestComboStreak() => longestComboStreak;
    public float GetTotalComboXpEarned() => totalComboXpEarned;
    public int GetConsecutiveBossHits() => consecutiveBossHits;
    public List<float> GetComboHitTimes() => comboHitTimes;
    public List<float> GetComboDamages() => comboDamages;
    public List<bool> GetComboCrits() => comboCrits;

    /// <summary>
    /// Get comprehensive combo statistics.
    /// </summary>
    public string GetComboStats()
    {
        return $"Combo: {comboCounter}/{maxComboCount} | Multiplier: {totalComboMultiplier:F2}x | " +
               $"Crits: {totalComboCrits} | DMG: {totalComboDamage:F1} | Floor: {currentFloor} | " +
               $"Guild: {(playerGuild != null ? "✓" : "✗")} | DualWield: {(dualWieldSystem?.IsDualWieldActive() ?? false ? "✓" : "✗")}";
    }

    /// <summary>
    /// Reset combo system (for level/floor changes).
    /// </summary>
    public void ResetComboSystem()
    {
        EndCombo();
        comboHitTimes.Clear();
        comboDamages.Clear();
        comboCrits.Clear();
        Debug.Log("✓ Combo system reset");
    }
}