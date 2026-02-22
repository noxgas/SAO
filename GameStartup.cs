using UnityEngine;

/// <summary>
/// Game startup script that shows character creation or loads existing character.
/// </summary>
public class GameStartup : MonoBehaviour
{
    [SerializeField] private CharacterCreationUI characterCreationUI;

    private void Start()
    {
        if (characterCreationUI == null)
            characterCreationUI = GetComponent<CharacterCreationUI>();

        // Show character creation menu
        characterCreationUI.ShowClassSelection();

        // For testing: Uncomment one of these to auto-create a character
        // characterCreationUI.QuickCreateDuelist();
        // characterCreationUI.QuickCreateVanguard();
        // characterCreationUI.QuickCreateSwordmaster();
    }

    /// <summary>
    /// Public method to call from UI button: Create Duelist
    /// </summary>
    public void CreateDuelist()
    {
        characterCreationUI.QuickCreateDuelist();
    }

    /// <summary>
    /// Public method to call from UI button: Create Vanguard
    /// </summary>
    public void CreateVanguard()
    {
        characterCreationUI.QuickCreateVanguard();
    }

    /// <summary>
    /// Public method to call from UI button: Create Swordmaster
    /// </summary>
    public void CreateSwordmaster()
    {
        characterCreationUI.QuickCreateSwordmaster();
    }
}