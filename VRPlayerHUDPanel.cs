using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// VR In-game HUD showing player stats during gameplay.
/// Always visible, positioned near player's head.
/// </summary>
public class VRPlayerHUDPanel : VRUIPanel
{
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Health & Stamina")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI manaText;

    [Header("Combo Info")]
    [SerializeField] private TextMeshProUGUI comboCountText;

    private Player currentPlayer;
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        // HUD should not fade in/out
        animationDuration = 0.1f;
    }

    private void Start()
    {
        currentPlayer = FindObjectOfType<Player>();
        isInitialized = true;
    }

    private void Update()
    {
        if (isVisible && isInitialized && currentPlayer != null)
        {
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        // Update player info
        playerNameText.text = currentPlayer.name;
        levelText.text = $"Lvl {currentPlayer.Level}";

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

            // Update health bar color
            if (combat.HealthPercent > 0.5f)
                healthBar.color = new Color(0, 1, 0.5f); // Green
            else if (combat.HealthPercent > 0.25f)
                healthBar.color = new Color(1, 1, 0); // Yellow
            else
                healthBar.color = new Color(1, 0.2f, 0.2f); // Red

            // Update combo if dual wield active
            DualWieldSystem dualWield = currentPlayer.GetComponent<DualWieldSystem>();
            if (dualWield != null && dualWield.IsDualWieldActive)
            {
                comboCountText.text = $"COMBO: {dualWield.CurrentComboHits}/5";
                if (dualWield.CanUseFinalStrike)
                    comboCountText.color = new Color(1, 1, 0); // Yellow - ready!
                else
                    comboCountText.color = new Color(0, 1, 1); // Cyan
            }
        }
    }

    protected override void OnShow()
    {
        currentPlayer = FindObjectOfType<Player>();
    }
}