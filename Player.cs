using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main player controller that ties together all systems.
/// Handles class initialization and Swordmaster conversion.
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int experience;
    [SerializeField] private int experienceToNextLevel = 1000;
    [SerializeField] private string playerClassName;
    [SerializeField] private string playerSubclassName;

    private ICharacterClass characterClass;
    private PlayerCombat combat;
    private Equipment equipment;

    public int Level => level;
    public int Experience => experience;
    public ICharacterClass CharacterClass => characterClass;
    public string ClassName => playerClassName;
    public string SubclassName => playerSubclassName;

    public void InitializePlayer(string className, string subclassName)
    {
        playerClassName = className;
        playerSubclassName = subclassName;

        characterClass = ClassFactory.CreateClass(className, subclassName);
        
        if (characterClass == null)
        {
            Debug.LogError("Failed to create character class");
            return;
        }

        // Setup combat system
        combat = GetComponent<PlayerCombat>();
        if (combat == null)
            combat = gameObject.AddComponent<PlayerCombat>();

        combat.Initialize(characterClass.BaseStats);

        // Add starting skills
        SkillDatabase.Initialize();
        foreach (var skillId in characterClass.StartingSkills)
        {
            var skillData = SkillDatabase.GetSkill(skillId);
            if (skillData != null)
                combat.AddSkill(skillData);
        }

        // Setup equipment
        equipment = GetComponent<Equipment>();
        if (equipment == null)
            equipment = gameObject.AddComponent<Equipment>();

        characterClass.Initialize(this);
        
        Debug.Log($"✓ Player initialized");
        Debug.Log($"  Class: {characterClass.ClassName}");
        Debug.Log($"  Subclass: {characterClass.SubclassName}");
    }

    /// <summary>
    /// Upgrade Swordsman to Swordmaster upon acquiring dual wield blade.
    /// </summary>
    public void UpgradeToSwordmaster()
    {
        if (playerClassName != "Swordsman")
        {
            Debug.LogError("Only Swordsmen can become Swordmasters!");
            return;
        }

        Debug.Log($"\n╔════════════════════════════════════════════════╗");
        Debug.Log($"║        🎉 CLASS UPGRADE: SWORDMASTER! 🎉        ║");
        Debug.Log($"╚════════════════════════════════════════════════╝\n");

        // Reinitialize as Swordmaster
        InitializePlayer("Swordsman", "Swordmaster");

        Debug.Log($"✓ You are now a SWORDMASTER!");
        Debug.Log($"✓ Dual Wield System activated!");
    }

    public void GainExperience(int amount)
    {
        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = (int)(experienceToNextLevel * 1.1f);

        characterClass.OnLevelUp(level);
        Debug.Log($"Level up! Now level {level}");
    }
}