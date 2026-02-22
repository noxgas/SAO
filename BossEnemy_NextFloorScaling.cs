using UnityEngine;
using System.Collections.Generic;

public enum BossPhase
{
    Phase1,
    Phase2,
    Phase3
}

/// <summary>
/// Boss enemy with health scaled to NEXT floor's median damage.
/// This creates a "challenge zone" mechanic where players can
/// voluntarily fight harder bosses for rare drops.
/// 
/// Example:
/// Floor 1 Boss HP: Scaled to Floor 2 median damage
/// Lvl 10 players can attempt Floor 1 boss = normal difficulty
/// Lvl 1 players can attempt Floor 1 boss = HARD MODE (rare drops!)
/// </summary>
public class BossEnemy : MonoBehaviour
{
    [SerializeField] private string bossId;
    [SerializeField] private string bossName;
    [SerializeField] private float baseHealth = 1000f;
    [SerializeField] private int bossFloor = 1;
    
    private float maxHealth;
    private float currentHealth;
    private float scaledHealthMultiplier; // For informational purposes
    
    private BossPhase currentPhase = BossPhase.Phase1;
    private float phase2Threshold = 0.66f;
    private float phase3Threshold = 0.33f;

    [System.Serializable]
    private class DamageContribution
    {
        public Player player;
        public float damageDealt;
    }

    private List<DamageContribution> damageContributions = new List<DamageContribution>();
    private Guild defeatersGuild;
    private Player finalHitPlayer;

    private void Start()
    {
        // Get scaled health from DamageScalingSystem
        // Boss health scales to NEXT floor's median damage
        DamageScalingSystem.Instance.SetFloor(bossFloor);
        maxHealth = DamageScalingSystem.Instance.GetBossHealth(bossFloor, baseHealth);
        currentHealth = maxHealth;

        // Calculate multiplier for display
        float baseMedian = DamageScalingSystem.Instance.GetMedianWeaponDamage(bossFloor);
        float nextMedian = DamageScalingSystem.Instance.GetMedianWeaponDamage(bossFloor + 1);
        scaledHealthMultiplier = nextMedian / baseMedian;

        Debug.Log($"\n╔════════════════════════════════════════════════════╗");
        Debug.Log($"║  BOSS ENCOUNTER INITIATED".PadRight(53) + "║");
        Debug.Log($"╠════════════════════════════════════════════════════╣");
        Debug.Log($"║  Boss: {bossName}".PadRight(53) + "║");
        Debug.Log($"║  Floor: {bossFloor}".PadRight(53) + "║");
        Debug.Log($"║  Base Health: {baseHealth:F0} HP".PadRight(53) + "║");
        Debug.Log($"║  Scaled Health: {maxHealth:F0} HP ({scaledHealthMultiplier:F2}x)".PadRight(53) + "║");
        Debug.Log($"║  Scaling Reference: Floor {bossFloor + 1} median damage".PadRight(53) + "║");
        Debug.Log($"║  Recommended Party: 4-8 players".PadRight(53) + "║");
        Debug.Log($"╚════════════════════════════════════════════════════╝\n");
    }

    /// <summary>
    /// Register damage dealt by a player.
    /// </summary>
    public void TakeDamage(float damage, Player damageDealer)
    {
        currentHealth -= damage;
        finalHitPlayer = damageDealer;

        // Track damage contribution
        DamageContribution contribution = damageContributions.Find(d => d.player == damageDealer);
        if (contribution != null)
        {
            contribution.damageDealt += damage;
        }
        else
        {
            damageContributions.Add(new DamageContribution
            {
                player = damageDealer,
                damageDealt = damage
            });
        }

        CheckPhaseTransition();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckPhaseTransition()
    {
        float healthPercent = currentHealth / maxHealth;

        if (healthPercent <= phase3Threshold && currentPhase != BossPhase.Phase3)
        {
            TransitionToPhase(BossPhase.Phase3);
        }
        else if (healthPercent <= phase2Threshold && currentPhase != BossPhase.Phase2)
        {
            TransitionToPhase(BossPhase.Phase2);
        }
    }

    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;
        Debug.Log($"🔥 {bossName} entering {newPhase}!");
    }

    private void Die()
    {
        Debug.Log($"\n╔════════════════════════════════════════════════════╗");
        Debug.Log($"║  🎉 {bossName} DEFEATED! 🎉".PadRight(53) + "║");
        Debug.Log($"║  Defeated by: {finalHitPlayer.name}".PadRight(53) + "║");
        Debug.Log($"║  Party Size: {damageContributions.Count}".PadRight(53) + "║");
        Debug.Log($"║  Difficulty Multiplier: {scaledHealthMultiplier:F2}x".PadRight(53) + "║");
        Debug.Log($"╚════════════════════════════════════════════════════╝\n");

        // Get defeating party
        List<Player> defeatersParty = GetDefeatersParty();

        // Process boss defeat
        BossDefeatRewardSystem rewardSystem = BossDefeatRewardSystem.Instance;
        if (rewardSystem != null)
        {
            rewardSystem.ProcessBossDefeat(bossId, finalHitPlayer, defeatersParty, defeatersGuild);
        }

        if (defeatersGuild != null)
        {
            defeatersGuild.RecordBossDefeat(0);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Get top damage dealers.
    /// </summary>
    private List<Player> GetDefeatersParty()
    {
        List<Player> defeaters = new List<Player>();
        damageContributions.Sort((a, b) => b.damageDealt.CompareTo(a.damageDealt));

        int partySize = Mathf.Min(4, damageContributions.Count);
        for (int i = 0; i < partySize; i++)
        {
            defeaters.Add(damageContributions[i].player);
        }

        return defeaters;
    }

    public void SetDefeatersGuild(Guild guild)
    {
        defeatersGuild = guild;
    }

    public float GetCurrentHealthPercent() => currentHealth / maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public int GetBossFloor() => bossFloor;
    public float GetDifficultyMultiplier() => scaledHealthMultiplier;
}