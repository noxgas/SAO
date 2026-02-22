using UnityEngine;
using System;

/// <summary>
/// Core stats system that scales with level.
/// Used by all classes and subclasses.
/// </summary>
[System.Serializable]
public class CharacterStats
{
    [SerializeField] public float baseHealth;
    [SerializeField] public float baseStamina;
    [SerializeField] public float baseMana;
    [SerializeField] public float basePhysicalDamage;
    [SerializeField] public float baseMagicalDamage;
    [SerializeField] public float baseDefense;
    [SerializeField] public float baseMagicResist;
    [SerializeField] public float baseCritChance;
    [SerializeField] public float baseMoveSpeed;

    [SerializeField] public float healthPerLevel = 15f;
    [SerializeField] public float staminaPerLevel = 5f;
    [SerializeField] public float manaPerLevel = 10f;
    [SerializeField] public float damagePerLevel = 2f;
    [SerializeField] public float defensePerLevel = 1f;

    private int currentLevel = 1;

    public float MaxHealth => baseHealth + (healthPerLevel * currentLevel);
    public float MaxStamina => baseStamina + (staminaPerLevel * currentLevel);
    public float MaxMana => baseMana + (manaPerLevel * currentLevel);
    public float PhysicalDamage => basePhysicalDamage + (damagePerLevel * currentLevel);
    public float MagicalDamage => baseMagicalDamage + (damagePerLevel * currentLevel);
    public float Defense => baseDefense + (defensePerLevel * currentLevel);
    public float MagicResist => baseMagicResist;
    public float CritChance => baseCritChance;
    public float MoveSpeed => baseMoveSpeed;

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Max(1, level);
    }

    public CharacterStats Clone()
    {
        return (CharacterStats)this.MemberwiseClone();
    }
}