// Add this to your Player.cs Initialize method:

public void InitializePlayer(string className, string playstyleName = "")
{
    characterClass = ClassFactory.CreateClass(className, playstyleName);
    
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
    
    Debug.Log($"Player initialized as {characterClass.ClassName}");
    if (!string.IsNullOrEmpty(playstyleName))
        Debug.Log($"Playstyle: {playstyleName}");
}