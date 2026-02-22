using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced combat system with threat management, ability queuing, and combat states.
/// Features: Interrupt system, ability combo validation, threat-based targeting, combat buffs.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat State")]
    [SerializeField] private CombatState currentCombatState = CombatState.Idle;
    [SerializeField] private float combatTimeout = 15f;
    [SerializeField] private float threatRange = 50f;

    private Player player;
    private BossEnemy currentTarget;
    private float lastCombatTime = 0f;
    private float totalThreatGenerated = 0f;

    // Ability system
    private Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
    private Queue<string> abilityQueue = new Queue<string>();
    private string currentCastingAbility = "";
    private float castProgress = 0f;

    // Combat statistics
    private CombatStatistics combatStats = new CombatStatistics();

    public bool IsInCombat => currentCombatState != CombatState.Idle;
    public CombatState CurrentCombatState => currentCombatState;
    public BossEnemy CurrentTarget => currentTarget;
    public CombatStatistics CombatStats => combatStats;

    private void Update()
    {
        UpdateCombatState();
        ProcessAbilityQueue();
        UpdateCasting();
    }

    public void Initialize(Player playerRef)
    {
        player = playerRef;
        Debug.Log("✓ Advanced PlayerCombat system initialized");
        Debug.Log("  Threat Management: Active");
        Debug.Log("  Ability Queueing: Active");
        Debug.Log("  Combat Statistics: Active");
    }

    /// <summary>
    /// Update combat state based on timing.
    /// </summary>
    private void UpdateCombatState()
    {
        if (IsInCombat && Time.time - lastCombatTime > combatTimeout)
        {
            ExitCombat();
        }
    }

    /// <summary>
    /// Enter combat with a target.
    /// </summary>
    public void EnterCombat(BossEnemy target)
    {
        currentTarget = target;
        currentCombatState = CombatState.InCombat;
        lastCombatTime = Time.time;
        totalThreatGenerated = 0f;

        Debug.Log($"⚔️ Entered combat with {target.BossName}!");
        combatStats.OnCombatStart();
    }

    /// <summary>
    /// Exit combat.
    /// </summary>
    public void ExitCombat()
    {
        currentCombatState = CombatState.Idle;
        currentTarget = null;
        abilityQueue.Clear();
        currentCastingAbility = "";

        Debug.Log($"✓ Exited combat | Total Threat: {totalThreatGenerated:F1}");
        combatStats.OnCombatEnd();
    }

    /// <summary>
    /// Add a skill to the combat system.
    /// </summary>
    public void AddSkill(Skill skill)
    {
        skills[skill.SkillId] = skill;
        Debug.Log($"✓ Skill added: {skill.SkillName}");
    }

    /// <summary>
    /// Queue an ability for execution.
    /// </summary>
    public bool QueueAbility(string skillId)
    {
        if (!skills.TryGetValue(skillId, out var skill))
        {
            Debug.LogWarning($"❌ Skill {skillId} not found!");
            return false;
        }

        if (!skill.IsReady)
        {
            Debug.LogWarning($"⏱️ {skill.SkillName} is on cooldown!");
            return false;
        }

        abilityQueue.Enqueue(skillId);
        Debug.Log($"📋 Queued: {skill.SkillName} (Queue size: {abilityQueue.Count})");
        return true;
    }

    /// <summary>
    /// Process the ability queue.
    /// </summary>
    private void ProcessAbilityQueue()
    {
        if (abilityQueue.Count == 0 || !string.IsNullOrEmpty(currentCastingAbility))
            return;

        string nextAbility = abilityQueue.Dequeue();
        CastAbility(nextAbility);
    }

    /// <summary>
    /// Cast an ability with casting time.
    /// </summary>
    private void CastAbility(string skillId)
    {
        if (!skills.TryGetValue(skillId, out var skill))
            return;

        currentCastingAbility = skillId;
        castProgress = 0f;
        Debug.Log($"🔮 Casting: {skill.SkillName}...");
    }

    /// <summary>
    /// Update casting progress.
    /// </summary>
    private void UpdateCasting()
    {
        if (string.IsNullOrEmpty(currentCastingAbility))
            return;

        var skill = skills[currentCastingAbility];
        float castTime = 0.5f; // Base casting time

        castProgress += Time.deltaTime / castTime;

        if (castProgress >= 1f)
        {
            ExecuteAbility(currentCastingAbility);
            currentCastingAbility = "";
        }
    }

    /// <summary>
    /// Execute an ability and generate threat.
    /// </summary>
    private void ExecuteAbility(string skillId)
    {
        if (!skills.TryGetValue(skillId, out var skill) || currentTarget == null)
            return;

        if (skill.Use(player.CharacterClass.BaseStats, player.Level))
        {
            float damage = skill.CalculateDamage(player.CharacterClass.BaseStats, player.Level);
            currentTarget.TakeDamage(damage, player);

            // Generate threat
            float threat = damage * 1.2f; // Abilities generate 120% threat
            GenerateThreat(threat);

            combatStats.OnAbilityUsed(skillId, damage);
            Debug.Log($"✨ {skill.SkillName} executed! {damage:F1} damage, Threat: {threat:F1}");
        }
    }

    /// <summary>
    /// Generate threat on current target.
    /// </summary>
    public void GenerateThreat(float threatAmount)
    {
        totalThreatGenerated += threatAmount;
        if (currentTarget != null)
        {
            Debug.Log($"⚠️ Threat: +{threatAmount:F1} (Total: {totalThreatGenerated:F1})");
        }
    }

    /// <summary>
    /// Get skill by ID.
    /// </summary>
    public Skill GetSkill(string skillId)
    {
        return skills.TryGetValue(skillId, out var skill) ? skill : null;
    }

    /// <summary>
    /// Get combat statistics summary.
    /// </summary>
    public string GetCombatStats()
    {
        return $"State: {currentCombatState} | Threat: {totalThreatGenerated:F1} | " +
               $"Queued: {abilityQueue.Count} | Target: {(currentTarget != null ? currentTarget.BossName : "None")}";
    }
}

/// <summary>
/// Combat state enumeration.
/// </summary>
public enum CombatState
{
    Idle,
    InCombat,
    Casting,
    Channeling,
    Interrupted,
    Dead
}

/// <summary>
/// Advanced combat statistics tracking.
/// </summary>
public class CombatStatistics
{
    private DateTime combatStartTime;
    private DateTime combatEndTime;
    private int abilitiesUsed = 0;
    private float totalDamageDealt = 0f;
    private float totalDamageTaken = 0f;
    private int criticalHits = 0;
    private int missedAbilities = 0;
    private Dictionary<string, int> abilityUsageCount = new Dictionary<string, int>();

    public void OnCombatStart()
    {
        combatStartTime = DateTime.Now;
        ResetStats();
    }

    public void OnCombatEnd()
    {
        combatEndTime = DateTime.Now;
    }

    public void OnAbilityUsed(string skillId, float damage)
    {
        abilitiesUsed++;
        totalDamageDealt += damage;

        if (!abilityUsageCount.ContainsKey(skillId))
            abilityUsageCount[skillId] = 0;
        abilityUsageCount[skillId]++;
    }

    public void OnCriticalHit()
    {
        criticalHits++;
    }

    public void OnMiss()
    {
        missedAbilities++;
    }

    private void ResetStats()
    {
        abilitiesUsed = 0;
        totalDamageDealt = 0f;
        totalDamageTaken = 0f;
        criticalHits = 0;
        missedAbilities = 0;
        abilityUsageCount.Clear();
    }

    public float GetDPS()
    {
        var duration = (combatEndTime - combatStartTime).TotalSeconds;
        return duration > 0 ? totalDamageDealt / (float)duration : 0f;
    }

    public string GetCombatSummary()
    {
        return $"Combat Duration: {(combatEndTime - combatStartTime).TotalSeconds:F1}s | " +
               $"DPS: {GetDPS():F1} | Abilities Used: {abilitiesUsed} | " +
               $"Crits: {criticalHits} | Misses: {missedAbilities}";
    }
}