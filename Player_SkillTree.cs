// Add this to Player.cs

private SkillTreeSystem skillTreeSystem;

private void InitializeSkillTree()
{
    skillTreeSystem = GetComponent<SkillTreeSystem>();
    if (skillTreeSystem == null)
        skillTreeSystem = gameObject.AddComponent<SkillTreeSystem>();
}

// In InitializePlayer method:
public void InitializePlayer(string className, string subclassName)
{
    // ... existing code ...

    // Initialize skill tree
    InitializeSkillTree();
}

// Update GainExperience to notify skill tree:
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
    
    // Award skill points
    if (skillTreeSystem != null)
    {
        skillTreeSystem.OnPlayerLevelUp(level);
    }

    Debug.Log($"Level up! Now level {level}");
}