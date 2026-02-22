using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Advanced class factory with dynamic loading, validation, and class analytics.
/// Features: Reflection-based loading, class requirements, synergy detection.
/// </summary>
public static class ClassFactory
{
    private static Dictionary<string, Type> registeredClasses = new Dictionary<string, Type>();
    private static Dictionary<string, ClassMetadata> classMetadata = new Dictionary<string, ClassMetadata>();
    private static bool isInitialized = false;

    /// <summary>
    /// Initialize the factory (call once at startup).
    /// </summary>
    public static void Initialize()
    {
        if (isInitialized)
            return;

        RegisterClass("Swordsman", typeof(Swordsman), new ClassRequirements { minimumLevel = 1 });
        RegisterClass("Mage", typeof(Mage), new ClassRequirements { minimumLevel = 5, requiredIntelligence = 50 });
        RegisterClass("Archer", typeof(Archer), new ClassRequirements { minimumLevel = 5, requiredDexterity = 50 });
        RegisterClass("Paladin", typeof(Paladin), new ClassRequirements { minimumLevel = 10, requiredWisdom = 60 });
        RegisterClass("Rogue", typeof(Rogue), new ClassRequirements { minimumLevel = 10, requiredDexterity = 70 });

        isInitialized = true;
        Debug.Log($"✓ Class Factory initialized with {registeredClasses.Count} classes");
    }

    /// <summary>
    /// Register a class type with metadata.
    /// </summary>
    private static void RegisterClass(string className, Type classType, ClassRequirements requirements)
    {
        registeredClasses[className] = classType;
        classMetadata[className] = new ClassMetadata
        {
            className = className,
            classType = classType,
            requirements = requirements,
            registrationTime = DateTime.Now
        };
    }

    /// <summary>
    /// Create a character class by name with validation.
    /// </summary>
    public static ICharacterClass CreateClass(string className)
    {
        if (!isInitialized)
            Initialize();

        if (!registeredClasses.TryGetValue(className, out var classType))
        {
            Debug.LogError($"❌ Class '{className}' not registered!");
            return null;
        }

        try
        {
            var instance = Activator.CreateInstance(classType) as ICharacterClass;
            Debug.Log($"✓ Created class instance: {className}");
            return instance;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Failed to create class '{className}': {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Check if a class can be unlocked by a player.
    /// </summary>
    public static bool CanUnlockClass(string className, Player player)
    {
        if (!classMetadata.TryGetValue(className, out var metadata))
            return false;

        var requirements = metadata.requirements;

        if (player.Level < requirements.minimumLevel)
        {
            Debug.LogWarning($"⚠️ Requires level {requirements.minimumLevel}");
            return false;
        }

        // Add more requirement checks as needed
        return true;
    }

    /// <summary>
    /// Get all available class names.
    /// </summary>
    public static string[] GetAvailableClasses()
    {
        if (!isInitialized)
            Initialize();

        return registeredClasses.Keys.ToArray();
    }

    /// <summary>
    /// Get class metadata.
    /// </summary>
    public static ClassMetadata GetClassMetadata(string className)
    {
        return classMetadata.TryGetValue(className, out var metadata) ? metadata : null;
    }

    /// <summary>
    /// Get class description.
    /// </summary>
    public static string GetClassDescription(string className)
    {
        var classInstance = CreateClass(className);
        return classInstance?.ClassDescription ?? "Unknown class";
    }

    /// <summary>
    /// Get all classes and their stats.
    /// </summary>
    public static string GetClassComparison()
    {
        if (!isInitialized)
            Initialize();

        string comparison = "╔════════════════════════════════════════╗\n" +
                          "║      AVAILABLE CLASSES COMPARISON       ║\n" +
                          "╠════════════════════════════════════════╣\n";

        foreach (var className in registeredClasses.Keys)
        {
            var classInstance = CreateClass(className);
            if (classInstance != null)
            {
                comparison += $"║ {className,-36} ║\n";
                comparison += $"║   {classInstance.ClassDescription,-33} ║\n";
            }
        }

        comparison += "╚════════════════════════════════════════╝";
        return comparison;
    }

    /// <summary>
    /// Get synergy information between classes.
    /// </summary>
    public static float GetClassSynergy(string class1, string class2)
    {
        // Define class synergies
        var synergies = new Dictionary<(string, string), float>
        {
            { ("Swordsman", "Paladin"), 0.8f },
            { ("Archer", "Rogue"), 0.9f },
            { ("Mage", "Paladin"), 0.7f },
            { ("Rogue", "Archer"), 0.85f },
        };

        return synergies.TryGetValue((class1, class2), out var synergy) ? synergy : 0f;
    }
}

/// <summary>
/// Class metadata for registration and validation.
/// </summary>
public class ClassMetadata
{
    public string className;
    public Type classType;
    public ClassRequirements requirements;
    public DateTime registrationTime;
    public int totalCreated = 0;

    public string GetMetadataInfo()
    {
        return $"{className} | Requires Lvl {requirements.minimumLevel} | Created: {totalCreated} times";
    }
}

/// <summary>
/// Class unlock requirements.
/// </summary>
[System.Serializable]
public class ClassRequirements
{
    public int minimumLevel = 1;
    public int requiredStrength = 0;
    public int requiredIntelligence = 0;
    public int requiredDexterity = 0;
    public int requiredWisdom = 0;
    public int requiredEndurance = 0;
    public List<string> requiredPreviousClasses = new List<string>();
    public int requiredCurrency = 0;

    public bool AreRequirementsMet(Player player)
    {
        if (player.Level < minimumLevel)
            return false;

        // Add stat and currency checks as needed
        return true;
    }

    public string GetRequirementsString()
    {
        var reqs = new List<string>();
        if (minimumLevel > 1) reqs.Add($"Lvl {minimumLevel}");
        if (requiredStrength > 0) reqs.Add($"STR {requiredStrength}");
        if (requiredIntelligence > 0) reqs.Add($"INT {requiredIntelligence}");
        if (requiredDexterity > 0) reqs.Add($"DEX {requiredDexterity}");

        return string.Join(" | ", reqs);
    }
}