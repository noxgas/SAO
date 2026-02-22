using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Advanced main player controller integrating all game systems.
/// Handles class initialization, progression, skill management, and state tracking.
/// Features: Multi-system integration, achievement tracking, statistics, and progression systems.
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Player Identity")]
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;
    [SerializeField] private int experienceToNextLevel = 1000;
    [SerializeField] private string playerName = "Player";
    [SerializeField] private string playerClassName = "";
    [SerializeField] private string playerSubclassName = "";

    [Header("Statistics")]
    [SerializeField] private int totalDamageDealt = 0;
    [SerializeField] private int totalDamageTaken = 0;
    [SerializeField] private int enemiesDefeated = 0;
    [SerializeField] private int bossesDefeated = 0;
    [SerializeField] private float playTime = 0f;

    // System References
    private ICharacterClass characterClass;
    private PlayerCombat combat;
    private Equipment equipment;
    private SkillTree skillTree;
    private SkillManager skillManager;
    private ComboSystem comboSystem;
    private DualWieldSystem dualWieldSystem;
    private Guild playerGuild;

    // State Management
    private PlayerState playerState = PlayerState.Idle;
    private Dictionary<string, float> statModifiers = new Dictionary<string, float>();
    private List<Achievement> unlockedAchievements = new List<Achievement>();

    public int Level => level;
    public int Experience => experience;
    public ICharacterClass CharacterClass => characterClass;
    public string ClassName => playerClassName;
    public string SubclassName => playerSubclassName;
    public SkillTree SkillTree => skillTree;
    public SkillManager SkillManager => skillManager;
    public ComboSystem ComboSystem => comboSystem;
    public PlayerState CurrentState => playerState;

    private void Awake()
    {
        GetAllComponents();
    }

    private void Start()
    {
        playTime = 0f;
    }

    private void Update()
    {
        playTime += Time.deltaTime;
        UpdatePlayerState();
    }

    /// <summary>
    /// Get or create all necessary components.
    /// </summary>
    private void GetAllComponents()
    {
        combat = GetComponent<PlayerCombat>();
        if (combat == null)
            combat = gameObject.AddComponent<PlayerCombat>();

        equipment = GetComponent<Equipment>();
        if (equipment == null)
            equipment = gameObject.AddComponent<Equipment>();

        skillTree = GetComponent<SkillTree>();
        if (skillTree == null)
            skillTree = gameObject.AddComponent<SkillTree>();

        skillManager = GetComponent<SkillManager>();
        if (skillManager == null)
            skillManager = gameObject.AddComponent<SkillManager>();

        comboSystem = GetComponent<ComboSystem>();
        if (comboSystem == null)
            comboSystem = gameObject.AddComponent<ComboSystem>();

        dualWieldSystem = GetComponent<DualWieldSystem>();
        if (dualWieldSystem == null)
            dualWieldSystem = gameObject.AddComponent<DualWieldSystem>();
    }

    /// <summary>
    /// Initialize the player with a specific class.
    /// </summary>
    public void InitializePlayer(string className)
    {
        playerClassName = className;
        characterClass = ClassFactory.CreateClass(className);

        if (characterClass == null)
        {
            Debug.LogError($"❌ Failed to create character class: {className}");
            return;
        }

        playerSubclassName = characterClass.SubclassName;

        if (combat != null)
            combat.Initialize(this);

        if (comboSystem != null)
        {
            comboSystem.SetCurrentFloor(1);
            comboSystem.SetPlayerGuild(playerGuild);
        }

        characterClass.Initialize(this);
        playerState = PlayerState.Idle;

        LogPlayerInitialization();
    }

    /// <summary>
    /// Initialize player with class and subclass.
    /// </summary>
    public void InitializePlayer(string className, string subclassName)
    {
        playerClassName = className;
        playerSubclassName = subclassName;

        characterClass = ClassFactory.CreateClass(className);

        if (characterClass == null)
        {
            Debug.LogError($"❌ Failed to create character class: {className}");
            return;
        }

        GetAllComponents();

        if (combat != null)
            combat.Initialize(this);

        if (comboSystem != null)
        {
            comboSystem.SetCurrentFloor(1);
            comboSystem.SetPlayerGuild(playerGuild);
        }

        characterClass.Initialize(this);
        playerState = PlayerState.Idle;

        LogPlayerInitialization();
    }

    /// <summary>
    /// Upgrade Swordsman to Swordmaster upon acquiring dual wield blade.
    /// </summary>
    public void UpgradeToSwordmaster()
    {
        if (playerClassName != "Swordsman")
        {
            Debug.LogError("❌ Only Swordsmen can become Swordmasters!");
            return;
        }

        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║   ⚔️  CLASS UPGRADE: SWORDMASTER!  ⚔️   ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");

        playerSubclassName = "Swordmaster";
        characterClass = ClassFactory.CreateClass("Swordsman");
        characterClass.Initialize(this);

        if (dualWieldSystem != null)
            dualWieldSystem.EnableDualWield();

        UnlockAchievement("Swordmaster", "Upgraded to Swordmaster class");
        LogPlayerInitialization();
    }

    /// <summary>
    /// Gain experience and handle level ups.
    /// </summary>
    public void GainExperience(int amount)
    {
        experience += amount;
        Debug.Log($"✨ +{amount} XP | Total: {experience}/{experienceToNextLevel}");

        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Level up the player.
    /// </summary>
    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = (int)(experienceToNextLevel * 1.1f);

        characterClass?.OnLevelUp(level);
        comboSystem?.SetCurrentFloor(level);

        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║           🎉 LEVEL UP! 🎉              ║");
        Debug.Log($"║ New Level: {level,30} ║");
        Debug.Log($"║ Next Level XP: {experienceToNextLevel,22} ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");

        CheckLevelAchievements();
    }

    /// <summary>
    /// Deal damage and track statistics.
    /// </summary>
    public void DealDamage(float damage)
    {
        totalDamageDealt += (int)damage;
        characterClass?.OnDealDamage(damage);
    }

    /// <summary>
    /// Take damage and track statistics.
    /// </summary>
    public void TakeDamage(float damage)
    {
        totalDamageTaken += (int)damage;
        characterClass?.OnTakeDamage(damage);
    }

    /// <summary>
    /// Defeat an enemy.
    /// </summary>
    public void DefeatEnemy(BossEnemy enemy = null)
    {
        if (enemy != null)
        {
            bossesDefeated++;
            GainExperience(enemy.GetRewardXP());
            UnlockAchievement("BossSlayer", $"Defeated {enemy.gameObject.name}");
        }
        else
        {
            enemiesDefeated++;
            GainExperience(100);
        }
    }

    /// <summary>
    /// Set player guild.
    /// </summary>
    public void SetGuild(Guild guild)
    {
        playerGuild = guild;
        if (comboSystem != null)
            comboSystem.SetPlayerGuild(guild);

        Debug.Log($"✓ Joined guild: {guild.name}");
    }

    /// <summary>
    /// Update player state based on actions.
    /// </summary>
    private void UpdatePlayerState()
    {
        if (comboSystem != null && comboSystem.IsComboActive())
        {
            playerState = PlayerState.Comboing;
        }
        else if (combat != null && combat.IsInCombat)
        {
            playerState = PlayerState.InCombat;
        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    /// <summary>
    /// Unlock an achievement.
    /// </summary>
    public void UnlockAchievement(string achievementId, string description)
    {
        var achievement = new Achievement
        {
            id = achievementId,
            description = description,
            unlockedTime = DateTime.Now
        };
        unlockedAchievements.Add(achievement);
        Debug.Log($"🏆 Achievement Unlocked: {description}");
    }

    /// <summary>
    /// Check level-based achievements.
    /// </summary>
    private void CheckLevelAchievements()
    {
        if (level == 10)
            UnlockAchievement("Level10", "Reached level 10");
        if (level == 25)
            UnlockAchievement("Level25", "Reached level 25");
        if (level == 50)
            UnlockAchievement("Level50", "Reached level 50");
    }

    /// <summary>
    /// Get comprehensive player statistics.
    /// </summary>
    public string GetPlayerStats()
    {
        return $"╔════════════════════════════════════════╗\n" +
               $"║          PLAYER STATISTICS             ║\n" +
               $"╠════════════════════════════════════════╣\n" +
               $"║ Name: {playerName,-32} ║\n" +
               $"║ Class: {playerClassName,-30} ║\n" +
               $"║ Subclass: {playerSubclassName,-28} ║\n" +
               $"║ Level: {level,-31} ║\n" +
               $"║ Experience: {experience,-27} ║\n" +
               $"║ Total Damage: {totalDamageDealt,-24} ║\n" +
               $"║ Damage Taken: {totalDamageTaken,-24} ║\n" +
               $"║ Enemies Defeated: {enemiesDefeated,-19} ║\n" +
               $"║ Bosses Defeated: {bossesDefeated,-20} ║\n" +
               $"║ Play Time: {playTime / 3600:F1} hours{"",-19} ║\n" +
               $"║ Achievements: {unlockedAchievements.Count,-23} ║\n" +
               $"╚════════════════════════════════════��═══╝";
    }

    /// <summary>
    /// Log player initialization.
    /// </summary>
    private void LogPlayerInitialization()
    {
        Debug.Log($"\n╔════════════════════════════════════════╗");
        Debug.Log($"║        ✓ PLAYER INITIALIZED            ║");
        Debug.Log($"╠════════════════════════════════════════╣");
        Debug.Log($"║ Player: {playerName,-32} ║");
        Debug.Log($"║ Class: {playerClassName,-30} ║");
        Debug.Log($"║ Subclass: {playerSubclassName,-28} ║");
        Debug.Log($"║ Level: {level,-31} ║");
        Debug.Log($"╚════════════════════════════════════════╝\n");
    }

    public int GetTotalDamageDealt() => totalDamageDealt;
    public int GetEnemiesDefeated() => enemiesDefeated;
    public int GetBossesDefeated() => bossesDefeated;
    public float GetPlayTime() => playTime;
    public List<Achievement> GetAchievements() => unlockedAchievements;
}

/// <summary>
/// Player state enumeration.
/// </summary>
public enum PlayerState
{
    Idle,
    Moving,
    InCombat,
    Comboing,
    Dead,
    Respawning
}

/// <summary>
/// Achievement structure.
/// </summary>
[System.Serializable]
public class Achievement
{
    public string id;
    public string description;
    public DateTime unlockedTime;
    public int points = 10;
}