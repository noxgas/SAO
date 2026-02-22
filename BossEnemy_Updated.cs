using UnityEngine;
using System.Collections.Generic;

public enum BossPhase
{
    Phase1,
    Phase2,
    Phase3
}

/// <summary>
/// Boss enemy with multiple phases and loot rewards.
/// Tracks the final hit for special bonus rewards.
/// </summary>
public class BossEnemy : MonoBehaviour
{
    [SerializeField] private string bossId;
    [SerializeField] private string bossName;
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float currentHealth;
    
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
    private Player finalHitPlayer; // The player who deals the killing blow

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Register damage dealt by a player.
    /// </summary>
    public void TakeDamage(float damage, Player damageDealer)
    {
        currentHealth -= damage;
        finalHitPlayer = damageDealer; // Track who dealt this damage (might be the final hit)

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
        Debug.Log($"Boss {bossName} entering {newPhase}!");
    }

    private void Die()
    {
        Debug.Log($"Boss {bossName} defeated by {finalHitPlayer.name}!");

        // Get party members (top 4 damage dealers, excluding final hit player)
        List<Player> partyMembers = GetTopDamageDealers(4);

        // Process boss defeat with proper reward distribution
        BossDefeatRewardSystem rewardSystem = BossDefeatRewardSystem.Instance;
        if (rewardSystem != null)
        {
            rewardSystem.ProcessBossDefeat(bossId, finalHitPlayer, partyMembers, defeatersGuild);
        }

        // Record defeat in guild stats
        if (defeatersGuild != null)
        {
            defeatersGuild.RecordBossDefeat(0); // Gold is handled by reward system
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Get top damage dealers (excluding the final hit player).
    /// </summary>
    private List<Player> GetTopDamageDealers(int count)
    {
        List<Player> topDealers = new List<Player>();

        // Sort by damage dealt (descending)
        damageContributions.Sort((a, b) => b.damageDealt.CompareTo(a.damageDealt));

        // Get top contributors, but exclude final hit player from this list
        foreach (var contribution in damageContributions)
        {
            if (contribution.player != finalHitPlayer && topDealers.Count < count)
            {
                topDealers.Add(contribution.player);
            }
        }

        return topDealers;
    }

    public void SetDefeatersGuild(Guild guild)
    {
        defeatersGuild = guild;
    }
}