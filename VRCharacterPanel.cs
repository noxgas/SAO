using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRCharacterPanel : VRMenuPanel
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

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 800f;
        panelHeight = 900f;
    }

    public override void Show()
    {
        base.Show();
        titleText.text = "👤 CHARACTER 👤";
        titleText.color = VRMenuSystem.Instance.GetAccentColor(AccentType.Green);

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

        CharacterStats stats = currentPlayer.CharacterClass.BaseStats;
        statsText.text = $"╔════════════════════════╗\n" +
                        $"║ HP:        {stats.MaxHealth,8:F0} ║\n" +
                        $"║ Stamina:   {stats.MaxStamina,8:F0} ║\n" +
                        $"║ Mana:      {stats.MaxMana,8:F0} ║\n" +
                        $"║ Damage:    {stats.PhysicalDamage,8:F1} ║\n" +
                        $"║ Defense:   {stats.Defense,8:F1} ║\n" +
                        $"║ Crit:      {(stats.CritChance * 100),7:F1}% ║\n" +
                        $"║ Move Spd:  {stats.MoveSpeed,8:F1} ║\n" +
                        $"╚════════════════════════╝";
    }
}