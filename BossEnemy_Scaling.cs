using UnityEngine;
using System.Collections.Generic;

public enum BossPhase
{
    Phase1,
    Phase2,
    Phase3
}

/// <summary>
/// Boss enemy with scaling health based on floor.
/// Base health: 1,000 HP
/// Scales by 8% per floor
/// </summary>
public class BossEnemy : MonoBehaviour
{
    [SerializeField] private string bossId;
    [SerializeField] private string bossName;
    [SerializeField] private float baseHealth = 1000f;
    [SerializeField] private int floorNumber = 1;
    
    private float maxHealth;
    private float currentHealth;
    
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
        DamageScalingSystem.Instance.SetFloor(floorNumber);
        maxHealth = DamageScalingSystem.Instance.GetBossHealth(baseHealth);
        currentHealth = maxHealth;

        Debug.Log($"\n╔═══════════��══════════════════════════╗");
        Debug.Log($"║  BOSS: {bossName,20}             ║");
        Debug.Log($"║  Floor: {floorNumber,2}                       ║");
        Debug.Log($"║  Health: {maxHealth:F0} HP                  ║");
        Debug.Log($"║  Recommended Party Size: 4-8     ║");
        Debug.Log($"╚══════════════════════════════════════╝\n");
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
        Debug.Log($"🔥 Boss {bossName} entering {newPhase}!");
    }

    private void Die()
    {
        Debug.Log($"\n╔══════════════════════════════════════╗");
        Debug.Log($"║  🎉 {bossName,23} DEFEATED! 🎉  ║");
        Debug.Log($"║  Defeated by: {finalHitPlayer.name,20}        ║");
        Debug.Log($"║  Party Size: {damageContributions.Count,21}              ║");
        Debug.Log($"╚══════════════════════════════════════╝\n");

        // Get defeating party (top damage dealers)
        List<Player> defeatersParty = GetDefeatersParty();

        // Process boss defeat and distribute rewards
        BossDefeatRewardSystem rewardSystem = BossDefeatRewardSystem.Instance;
        if (rewardSystem != null)
        {
            rewardSystem.ProcessBossDefeat(bossId, finalHitPlayer, defeatersParty, defeatersGuild);
        }

        // Record defeat in guild stats
        if (defeatersGuild != null)
        {
            defeatersGuild.RecordBossDefeat(0);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Get top damage dealers (excluding the final hit player).
    /// </summary>
    private List<Player> GetDefeatersParty()
    {
        List<Player> defeaters = new List<Player>();

        // Sort by damage dealt
        damageContributions.Sort((a, b) => b.damageDealt.CompareTo(a.damageDealt));

        // Take top 4 contributors (or all if less than 4)
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
}