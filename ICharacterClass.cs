/// <summary>
/// Base interface for all character classes.
/// Allows modular subclass implementation.
/// </summary>
public interface ICharacterClass
{
    string ClassName { get; }
    string SubclassName { get; }
    CharacterStats BaseStats { get; }
    List<string> StartingSkills { get; }
    
    void Initialize(Player player);
    void OnLevelUp(int newLevel);
}