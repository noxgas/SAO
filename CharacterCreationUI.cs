using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles character class and subclass selection during game startup.
/// Displays options for players to choose their class and build.
/// </summary>
public class CharacterCreationUI : MonoBehaviour
{
    [System.Serializable]
    public class ClassOption
    {
        public string className;
        public string description;
        public Sprite icon;
    }

    [System.Serializable]
    public class SubclassOption
    {
        public string subclassName;
        public string description;
        public string playstyleInfo;
        public Sprite icon;
    }

    [Header("Swordsman Subclasses")]
    [SerializeField] private List<SubclassOption> swordsmanSubclasses = new List<SubclassOption>();

    [Header("Tank Subclasses")]
    [SerializeField] private List<SubclassOption> tankSubclasses = new List<SubclassOption>();

    [Header("Mage Subclasses")]
    [SerializeField] private List<SubclassOption> mageSubclasses = new List<SubclassOption>();

    private string selectedClass;
    private string selectedSubclass;
    private Player playerInstance;

    private void Awake()
    {
        InitializeSubclassOptions();
    }

    /// <summary>
    /// Initialize all subclass options with descriptions.
    /// </summary>
    private void InitializeSubclassOptions()
    {
        // Swordsman subclasses
        swordsmanSubclasses.Add(new SubclassOption
        {
            subclassName = "Duelist",
            description = "Master of precision strikes",
            playstyleInfo = "High Crit Chance | Fast Attacks | Low Defense\nUse rapiers and light piercing weapons",
            icon = null // Add sprite in inspector
        });

        swordsmanSubclasses.Add(new SubclassOption
        {
            subclassName = "Vanguard",
            description = "Wielder of massive greatswords",
            playstyleInfo = "High Damage | AoE | High Defense\nUse heavy greatswords and armor-breaking techniques",
            icon = null // Add sprite in inspector
        });

        swordsmanSubclasses.Add(new SubclassOption
        {
            subclassName = "Swordmaster",
            description = "Dual wield master of combo chains",
            playstyleInfo = "Combo-Based | Final Hit Bonus | High Stamina\nDual wield locked trait - only uses dual weapons",
            icon = null // Add sprite in inspector
        });

        // Tank subclasses
        tankSubclasses.Add(new SubclassOption
        {
            subclassName = "Shield Tank",
            description = "Guardian with sword and shield",
            playstyleInfo = "Block | Taunt | Damage Mitigation",
            icon = null
        });

        tankSubclasses.Add(new SubclassOption
        {
            subclassName = "Heavy Weapon Tank",
            description = "Crowd controller with massive weapons",
            playstyleInfo = "Crowd Control | Stagger | High Damage",
            icon = null
        });

        // Mage subclasses
        mageSubclasses.Add(new SubclassOption
        {
            subclassName = "Attack Mage",
            description = "Elemental damage dealer",
            playstyleInfo = "AoE Damage | High Burst | Low Defense",
            icon = null
        });

        mageSubclasses.Add(new SubclassOption
        {
            subclassName = "Support Mage",
            description = "Healer and buff provider",
            playstyleInfo = "Healing | Buffs | Debuff Removal",
            icon = null
        });
    }

    /// <summary>
    /// Display main class selection menu.
    /// </summary>
    public void ShowClassSelection()
    {
        Debug.Log("=== CLASS SELECTION ===");
        Debug.Log("1. Swordsman - Fast melee DPS");
        Debug.Log("2. Tank - Defense and aggro control");
        Debug.Log("3. Mage - Ranged damage and support");
    }

    /// <summary>
    /// Select a class and show subclass options.
    /// </summary>
    public void SelectClass(string className)
    {
        selectedClass = className;
        Debug.Log($"Selected class: {className}");
        ShowSubclassSelection(className);
    }

    /// <summary>
    /// Show available subclasses for selected class.
    /// </summary>
    private void ShowSubclassSelection(string className)
    {
        List<SubclassOption> options = className switch
        {
            "Swordsman" => swordsmanSubclasses,
            "Tank" => tankSubclasses,
            "Mage" => mageSubclasses,
            _ => new List<SubclassOption>()
        };

        Debug.Log($"\n=== {className.ToUpper()} SUBCLASSES ===");
        for (int i = 0; i < options.Count; i++)
        {
            Debug.Log($"{i + 1}. {options[i].subclassName} - {options[i].description}");
            Debug.Log($"   {options[i].playstyleInfo}\n");
        }
    }

    /// <summary>
    /// Select a subclass and create the character.
    /// </summary>
    public void SelectSubclass(string subclassName)
    {
        selectedSubclass = subclassName;
        Debug.Log($"Selected subclass: {subclassName}");
        CreateCharacter();
    }

    /// <summary>
    /// Create the player with selected class and subclass.
    /// </summary>
    private void CreateCharacter()
    {
        Debug.Log($"\n✓ Creating {selectedClass} - {selectedSubclass}...\n");

        // Find or create player object
        playerInstance = FindObjectOfType<Player>();
        if (playerInstance == null)
        {
            GameObject playerGO = new GameObject("Player");
            playerInstance = playerGO.AddComponent<Player>();
        }

        // Initialize with selected class and subclass
        playerInstance.InitializePlayer(selectedClass, selectedSubclass);

        Debug.Log($"✓ Character created successfully!");
        Debug.Log($"Class: {playerInstance.CharacterClass.ClassName}");
        Debug.Log($"Subclass: {playerInstance.CharacterClass.SubclassName}");
    }

    /// <summary>
    /// Quick test method to create a Swordmaster (Dual Wield).
    /// </summary>
    public void QuickCreateSwordmaster()
    {
        SelectClass("Swordsman");
        SelectSubclass("Swordmaster");
    }

    /// <summary>
    /// Quick test method to create a Duelist.
    /// </summary>
    public void QuickCreateDuelist()
    {
        SelectClass("Swordsman");
        SelectSubclass("Duelist");
    }

    /// <summary>
    /// Quick test method to create a Vanguard.
    /// </summary>
    public void QuickCreateVanguard()
    {
        SelectClass("Swordsman");
        SelectSubclass("Vanguard");
    }
}