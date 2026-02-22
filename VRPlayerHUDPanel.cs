using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRPlayerHUDPanel : VRMenuPanel
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

    [Header("Combo Info")]
    [SerializeField] private TextMeshProUGUI comboCountText;

    private Player currentPlayer;
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        scaleAnimationDuration = 0.1f;
    }

    public override void Show()
    {
        base.Show();
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
        playerNameText.text = currentPlayer.name;
        levelText.text = $"Lvl {currentPlayer.Level}";
        floorText.text = $"Floor {WorldManager.Instance.GetCurrentFloor()}";

        PlayerCombat combat = currentPlayer.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            healthBar.fillAmount = combat.HealthPercent;
            staminaBar.fillAmount = combat.StaminaPercent;
            manaBar.fillAmount = combat.ManaPercent;

            healthText.text = $"HP: {Mathf.RoundToInt(combat.HealthPercent * 100)}%";
            staminaText.text = $"STA: {Mathf.RoundToInt(combat.StaminaPercent * 100)}%";
            manaText.text = $"MANA: {Mathf.RoundToInt(combat.ManaPercent * 100)}%";

            if (combat.HealthPercent > 0.5f)
                healthBar.color = new Color(0, 1, 0.5f);
            else if (combat.HealthPercent > 0.25f)
                healthBar.color = new Color(1, 1, 0);
            else
                healthBar.color = new Color(1, 0.2f, 0.2f);

            DualWieldSystem dualWield = currentPlayer.GetComponent<DualWieldSystem>();
            if (dualWield != null && dualWield.IsComboActive())
            {
                comboCountText.text = $"COMBO: {dualWield.GetCurrentComboCount()}/5";
                if (dualWield.GetCurrentComboCount() >= 5)
                    comboCountText.color = new Color(1, 1, 0);
                else
                    comboCountText.color = new Color(0, 1, 1);
            }
        }
    }
}