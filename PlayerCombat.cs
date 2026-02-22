using UnityEngine;
using System.Collections.Generic;

public enum CombatStyle
{
    MeleeStamina,
    MagicMana
}

/// <summary>
/// Handles player combat logic, skill execution, and resource management.
/// Now integrates with Dual Wield System for combo tracking.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentStamina;
    [SerializeField] private float currentMana;
    
    private CharacterStats stats;
    private Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
    private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();
    private CombatStyle combatStyle;
    private DualWieldSystem dualWieldSystem;

    private float staminaRegenRate = 30f; // per second
    private float manaRegenRate = 25f; // per second

    public float HealthPercent => stats != null ? currentHealth / stats.MaxHealth : 0f;
    public float StaminaPercent => stats != null ? currentStamina / stats.MaxStamina : 0f;
    public float ManaPercent => stats != null ? currentMana / stats.MaxMana : 0f;

    public void Initialize(CharacterStats characterStats)
    {
        stats = characterStats.Clone();
        currentHealth = stats.MaxHealth;
        currentStamina = stats.MaxStamina;
        currentMana = stats.MaxMana;

        dualWieldSystem = GetComponent<DualWieldSystem>();
    }

    public void SetCombatStyle(CombatStyle style)
    {
        combatStyle = style;
    }

    public void AddSkill(Skill skill)
    {
        var skillCopy = skill.Clone();
        skills[skill.skillId] = skillCopy;
        skillCooldowns[skill.skillId] = 0f;
    }

    public bool TryExecuteSkill(string skillId, Vector3 targetPosition = default)
    {
        if (!skills.TryGetValue(skillId, out var skill))
        {
            Debug.LogWarning($"Skill not found: {skillId}");
            return false;
        }

        // Check cooldown
        if (skillCooldowns.TryGetValue(skillId, out var cooldown) && cooldown > 0)
        {
            Debug.LogWarning($"Skill on cooldown: {skillId}");
            return false;
        }

        // Check resources
        if (skill.staminaCost > 0 && currentStamina < skill.staminaCost)
        {
            Debug.LogWarning("Insufficient stamina");
            return false;
        }

        if (skill.manaCost > 0 && currentMana < skill.manaCost)
        {
            Debug.LogWarning("Insufficient mana");
            return false;
        }

        // Consume resources
        currentStamina -= skill.staminaCost;
        currentMana -= skill.manaCost;

        // Apply cooldown
        skillCooldowns[skillId] = skill.cooldownSeconds;

        // Increase proficiency
        skill.IncreaseProficiency();

        // Execute skill
        OnSkillExecuted(skill, targetPosition);

        return true;
    }

    private void OnSkillExecuted(Skill skill, Vector3 targetPosition)
    {
        Debug.Log($"Skill executed: {skill.skillName} at {targetPosition}");

        // If dual wield system exists and this is a dual wield skill, register hit
        if (dualWieldSystem != null && skill.skillId.Contains("dual"))
        {
            dualWieldSystem.RegisterHit();

            // Check if final strike is ready
            if (dualWieldSystem.CanUseFinalStrike && skill.skillId == "final_strike")
            {
                float finalStrikeDamage = dualWieldSystem.ExecuteFinalStrike();
                // Apply this damage to enemies
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(stats.MaxHealth, currentHealth + healAmount);
    }

    private void OnDeath()
    {
        Debug.Log("Player defeated!");
    }

    private void Update()
    {
        if (stats == null) return;

        // Regenerate resources
        if (currentStamina < stats.MaxStamina)
            currentStamina += staminaRegenRate * Time.deltaTime;

        if (currentMana < stats.MaxMana)
            currentMana += manaRegenRate * Time.deltaTime;

        // Cap resources
        currentStamina = Mathf.Min(stats.MaxStamina, currentStamina);
        currentMana = Mathf.Min(stats.MaxMana, currentMana);

        // Update cooldowns
        foreach (var key in new List<string>(skillCooldowns.Keys))
        {
            if (skillCooldowns[key] > 0)
                skillCooldowns[key] -= Time.deltaTime;
        }
    }
}