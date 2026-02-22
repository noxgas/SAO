using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// VR Character panel showing player stats and class info.
/// </summary>
public class VRCharacterPanel : VRUIPanel
{
    [Header("Character Info")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Navigation")]
    [SerializeField] private VRButton backButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private Player currentPlayer;

    private void Start()
    {
        titleText.text = "👤 CHARACTER 👤";
        titleText.color = VRUIManager.Instance.GetAccentColor(AccentType.Green);
        
        backButton.OnClicked += OnBackClicked;
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

        // Get stats
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

    private void OnBackClicked()
    {
        Hide();
    }
}