using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Character panel showing player stats and class info.
/// </summary>
public class CharacterPanel : UIPanel
{
    [Header("Character Info")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private Player currentPlayer;

    private void Start()
    {
        titleText.text = "👤 CHARACTER 👤";
        titleText.color = UIManager.Instance.GetAccentColor(AccentType.Green);
    }

    protected override void OnShow()
    {
        currentPlayer = FindObjectOfType<Player>();
        UpdateCharacterInfo();
    }

    private void UpdateCharacterInfo()
    {
        if (currentPlayer == null) return;

        characterNameText.text = currentPlayer.name;
        classText.text = $"Class: {currentPlayer.CharacterClass.ClassName}";
        levelText.text = $"Level: {currentPlayer.Level}";
        experienceText.text = $"EXP: {currentPlayer.Experience}";

        // Get stats
        CharacterStats stats = currentPlayer.CharacterClass.BaseStats;
        statsText.text = $"═══════════════════════════════\n" +
                        $"HP:        {stats.MaxHealth}\n" +
                        $"Stamina:   {stats.MaxStamina}\n" +
                        $"Mana:      {stats.MaxMana}\n" +
                        $"Damage:    {stats.PhysicalDamage}\n" +
                        $"Defense:   {stats.Defense}\n" +
                        $"Crit:      {(stats.CritChance * 100):F1}%\n" +
                        $"Move Spd:  {stats.MoveSpeed}\n" +
                        $"═══════════════════════════════";
    }
}