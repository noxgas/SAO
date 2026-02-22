using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Boss enemy that can be defeated for rewards.
/// Tracks damage dealt by each player for reward distribution.
/// </summary>
public class BossEnemy : MonoBehaviour
{
    [SerializeField] private string bossId;
    [SerializeField] private string bossName;
    [SerializeField] private int floor;
    [SerializeField] private float maxHealth;
    [SerializeField] private float damageMultiplier = 1f;

    private float currentHealth;
    private Dictionary<Player, float> damageDealt = new Dictionary<Player, float>();
    private Guild defeatersGuild;
    private Player finalHitPlayer;
    private bool isDefeated = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Player damageDealer)
    {
        if (isDefeated) return;

        float actualDamage = damage * damageMultiplier;
        currentHealth -= actualDamage;

        if (!damageDealt.ContainsKey(damageDealer))
            damageDealt[damageDealer] = 0;

        damageDealt[damageDealer] += actualDamage;
        finalHitPlayer = damageDealer;

        Debug.Log($"{bossName} took {actualDamage:F1} damage from {damageDealer.name}. HP: {currentHealth:F1}/{maxHealth:F1}");

        if (currentHealth <= 0)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        isDefeated = true;
        Debug.Log($"⚔️ {bossName} DEFEATED!");

        List<Player> topDealers = GetTopDamageDealers(4);

        List<BossDefeatRewardSystem.DefeatReward> rewards = BossDefeatRewardSystem.Instance.ProcessBossDefeat(
            bossId,
            finalHitPlayer,
            topDealers,
            defeatersGuild
        );

        Destroy(gameObject);
    }

    private List<Player> GetTopDamageDealers(int count)
    {
        List<Player> result = new List<Player>();
        var sortedDamage = new List<(Player, float)>();

        foreach (var kvp in damageDealt)
        {
            sortedDamage.Add((kvp.Key, kvp.Value));
        }

        sortedDamage.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        for (int i = 0; i < Mathf.Min(count, sortedDamage.Count); i++)
        {
            result.Add(sortedDamage[i].Item1);
        }

        return result;
    }

    public void SetGuild(Guild guild)
    {
        defeatersGuild = guild;
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
    public bool IsDefeated => isDefeated;
}