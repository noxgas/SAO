using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// In-game HUD showing player stats during gameplay.
/// SAO-style holographic health bars and status display.
/// </summary>
public class PlayerHUDPanel : UIPanel
{
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI floorText;

    [Header("Health & Stamina")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI manaText;

    [Header("Buffs/Debuffs")]
    [SerializeField] private Transform buffContainer;
    [SerializeField] private Image buffPrefab;

    [Header("Combat Info")]
    [SerializeField] private TextMeshProUGUI comboCountText;
    [SerializeField] private TextMeshProUGUI damageNumberPrefab;

    private Player currentPlayer;
    private RectTransform healthBarRect;
    private RectTransform staminaBarRect;
    private RectTransform manaBarRect;

    protected override void Awake()
    {
        base.Awake();
        healthBarRect = healthBar.GetComponent<RectTransform>();
        staminaBarRect = staminaBar.GetComponent<RectTransform>();
        manaBarRect = manaBar.GetComponent<RectTransform>();
    }

    private void Start()
    {
        currentPlayer = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (isVisible && currentPlayer != null)
        {
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        // Update player info
        playerNameText.text = currentPlayer.name;
        levelText.text = $"Level {currentPlayer.Level}";

        // Update bars
        PlayerCombat combat = currentPlayer.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            healthBar.fillAmount = combat.HealthPercent;
            staminaBar.fillAmount = combat.StaminaPercent;
            manaBar.fillAmount = combat.ManaPercent;

            healthText.text = $"HP: {Mathf.RoundToInt(combat.HealthPercent * 100)}%";
            staminaText.text = $"STA: {Mathf.RoundToInt(combat.StaminaPercent * 100)}%";
            manaText.text = $"MANA: {Mathf.RoundToInt(combat.ManaPercent * 100)}%";
        }

        // Update colors based on health
        if (combat.HealthPercent > 0.5f)
            healthBar.color = new Color(0, 1, 0.5f); // Green
        else if (combat.HealthPercent > 0.25f)
            healthBar.color = new Color(1, 1, 0); // Yellow
        else
            healthBar.color = new Color(1, 0.2f, 0.2f); // Red
    }

    protected override void OnShow()
    {
        currentPlayer = FindObjectOfType<Player>();
    }
}