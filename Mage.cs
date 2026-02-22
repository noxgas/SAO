using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Mage class - magic user.
/// </summary>
public class Mage : ICharacterClass
{
    public string ClassName => "Mage";
    public string SubclassName => "Wizard";

    public CharacterStats BaseStats
    {
        get
        {
            CharacterStats stats = ScriptableObject.CreateInstance<CharacterStats>();
            stats.baseHealth = 60f;
            stats.baseStamina = 80f;
            stats.baseMana = 200f;
            stats.baseMagicalDamage = 30f;
            stats.baseDefense = 3f;
            stats.baseMoveSpeed = 6f;
            stats.baseCritChance = 0.10f;
            return stats;
        }
    }

    public List<string> StartingSkills => new List<string> { "Fireball", "Ice Spike", "Mana Shield" };

    public void Initialize(Player player)
    {
        Debug.Log($"✓ {ClassName} class initialized for {player.name}");
    }

    public void OnLevelUp(int newLevel)
    {
        Debug.Log($"Mage leveled up to {newLevel}");
    }
}