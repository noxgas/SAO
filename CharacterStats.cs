using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced character statistics system with scaling, bonuses, and progression tracking.
/// Handles all stat calculations, modifiers, and attribute management.
/// </summary>
public class CharacterStats
{
    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseStamina = 100f;
    public float baseMana = 100f;
    public float basePhysicalDamage = 10f;
    public float baseMagicalDamage = 10f;
    public float baseDefense = 5f;
    public float baseMoveSpeed = 6f;
    public float baseCritChance = 0.1f;

    [Header("Current Stats")]
    public float currentHealth = 100f;
    public float currentStamina = 100f;
    public float currentMana = 100f;

    [Header("Scaling")]
    private float levelScaling = 1.1f;
    private float equipmentBonus = 1f;

    // Stat modifiers
    private Dictionary<string, float> statModifiers = new Dictionary<string, float>();
    private Dictionary<string, float> percentModifiers = new Dictionary<string, float>();

    // Properties with calculations
    public float Health => baseHealth * GetModifier("Health");
    public float Stamina => baseStamina * GetModifier("Stamina");
    public float Mana => baseMana * GetModifier("Mana");
    public float PhysicalDamage => basePhysicalDamage * GetModifier("PhysicalDamage");
    public float MagicalDamage => baseMagicalDamage * GetModifier("MagicalDamage");
    public float Defense => baseDefense * GetModifier("Defense");
    public float MoveSpeed => baseMoveSpeed * GetModifier("MoveSpeed");
    public float CritChance => Mathf.Clamp01(baseCritChance + GetPercentModifier("CritChance"));

    /// <summary>
    /// Constructor - initialize stats.
    /// </summary>
    public CharacterStats()
    {
        currentHealth = baseHealth;
        currentStamina = baseStamina;
        currentMana = baseMana;
    }

    /// <summary>
    /// Apply a flat modifier to a stat.
    /// </summary>
    public void AddModifier(string statName, float amount)
    {
        if (!statModifiers.ContainsKey(statName))
            statModifiers[statName] = 0f;

        statModifiers[statName] += amount;
        Debug.Log($"✓ Added modifier to {statName}: +{amount}");
    }

    /// <summary>
    /// Remove a flat modifier from a stat.
    /// </summary>
    public void RemoveModifier(string statName, float amount)
    {
        if (statModifiers.ContainsKey(statName))
        {
            statModifiers[statName] -= amount;
        }
    }

    /// <summary>
    /// Apply a percentage modifier to a stat.
    /// </summary>
    public void AddPercentModifier(string statName, float percent)
    {
        if (!percentModifiers.ContainsKey(statName))
            percentModifiers[statName] = 0f;

        percentModifiers[statName] += percent;
        Debug.Log($"✓ Added {percent * 100}% modifier to {statName}");
    }

    /// <summary>
    /// Get the combined modifier for a stat.
    /// </summary>
    private float GetModifier(string statName)
    {
        float baseModifier = 1f;
        if (statModifiers.TryGetValue(statName, out var flatMod))
            baseModifier += flatMod;
        if (percentModifiers.TryGetValue(statName, out var percentMod))
            baseModifier *= (1f + percentMod);
        return baseModifier;
    }

    /// <summary>
    /// Get percentage modifier.
    /// </summary>
    private float GetPercentModifier(string statName)
    {
        return percentModifiers.TryGetValue(statName, out var mod) ? mod : 0f;
    }

    /// <summary>
    /// Apply level scaling to stats.
    /// </summary>
    public void ApplyLevelScaling(int level)
    {
        float scaling = Mathf.Pow(levelScaling, level - 1);
        baseHealth *= scaling;
        baseStamina *= scaling;
        baseMana *= scaling;
        basePhysicalDamage *= scaling;
        baseMagicalDamage *= scaling;
        baseDefense *= scaling;

        Debug.Log($"✓ Applied level {level} scaling: {scaling:F2}x");
    }

    /// <summary>
    /// Apply equipment bonuses.
    /// </summary>
    public void ApplyEquipmentBonus(float bonus)
    {
        equipmentBonus = bonus;
        Debug.Log($"✓ Applied equipment bonus: {bonus:F2}x");
    }

    /// <summary>
    /// Reset all modifiers.
    /// </summary>
    public void ResetModifiers()
    {
        statModifiers.Clear();
        percentModifiers.Clear();
        Debug.Log("✓ All stat modifiers reset");
    }

    /// <summary>
    /// Get comprehensive stats display.
    /// </summary>
    public string GetStatsDisplay()
    {
        return $"╔════════════════════════════════════════╗\n" +
               $"║          CHARACTER STATISTICS          ║\n" +
               $"╠════════════════════════════════════════╣\n" +
               $"║ Health: {Health,31:F1} ║\n" +
               $"║ Stamina: {Stamina,29:F1} ║\n" +
               $"║ Mana: {Mana,32:F1} ║\n" +
               $"║ Phys DMG: {PhysicalDamage,28:F1} ║\n" +
               $"║ Mag DMG: {MagicalDamage,29:F1} ║\n" +
               $"║ Defense: {Defense,29:F1} ║\n" +
               $"║ Move Speed: {MoveSpeed,26:F1} ║\n" +
               $"║ Crit Chance: {CritChance * 100,23:F1}% ║\n" +
               $"╚════════════════════════════════════════╝";
    }

    /// <summary>
    /// Clone stats for a new instance.
    /// </summary>
    public CharacterStats Clone()
    {
        var clone = new CharacterStats
        {
            baseHealth = this.baseHealth,
            baseStamina = this.baseStamina,
            baseMana = this.baseMana,
            basePhysicalDamage = this.basePhysicalDamage,
            baseMagicalDamage = this.baseMagicalDamage,
            baseDefense = this.baseDefense,
            baseMoveSpeed = this.baseMoveSpeed,
            baseCritChance = this.baseCritChance
        };
        return clone;
    }

    /// <summary>
    /// Get total stats summary.
    /// </summary>
    public string GetSummary()
    {
        return $"HP:{Health:F0} | STM:{Stamina:F0} | MNA:{Mana:F0} | " +
               $"PDMG:{PhysicalDamage:F1} | MDMG:{MagicalDamage:F1} | DEF:{Defense:F1} | SPD:{MoveSpeed:F1}";
    }
}