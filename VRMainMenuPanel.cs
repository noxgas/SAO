using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main menu panel - displays player status and menu options.
/// Top section: Status panel
/// Center section: Vertical menu list
/// </summary>
public class VRMainMenuPanel : VRMenuPanel
{
    [Header("Status Panel")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image spBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private TextMeshProUGUI colText;
    [SerializeField] private TextMeshProUGUI guildNameText;

    [Header("Status Labels")]
    [SerializeField] private TextMeshProUGUI hpLabel;
    [SerializeField] private TextMeshProUGUI spLabel;
    [SerializeField] private TextMeshProUGUI mpLabel;

    [Header("Menu Options")]
    [SerializeField] private Transform menuOptionsContainer;
    [SerializeField] private VRMenuButton menuButtonPrefab;

    [Header("Menu Layout")]
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    private List<VRMenuButton> menuButtons = new List<VRMenuButton>();
    private Player currentPlayer;
    private PlayerCombat playerCombat;

    private string[] menuOptions = new string[]
    {
        "Inventory",
        "Equipment",
        "Skills",
        "Map",
        "Party",
        "Guild",
        "Trade",
        "Messages",
        "Quest Log",
        "Settings",
        "Log Out"
    };

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 500f;
        panelHeight = 1000f;
    }

    private void Start()
    {
        CreateMenuButtons();
    }

    private void CreateMenuButtons()
    {
        currentPlayer = VRMenuSystem.Instance.GetCurrentPlayer();
        playerCombat = currentPlayer.GetComponent<PlayerCombat>();

        foreach (string option in menuOptions)
        {
            VRMenuButton button = Instantiate(menuButtonPrefab, menuOptionsContainer);
            button.Initialize(option, this);
            menuButtons.Add(button);
        }
    }

    private void Update()
    {
        if (isVisible && currentPlayer != null)
        {
            UpdateStatusPanel();
        }
    }

    private void UpdateStatusPanel()
    {
        // Update character info
        playerNameText.text = currentPlayer.name;
        levelText.text = $"Lvl {currentPlayer.Level}";
        floorText.text = $"Floor: 1"; // TODO: Get from world manager
        colText.text = $"Col: 0"; // TODO: Get from inventory
        guildNameText.text = "Guild: None"; // TODO: Get from guild manager

        // Update bars
        if (playerCombat != null)
        {
            hpBar.fillAmount = playerCombat.HealthPercent;
            spBar.fillAmount = playerCombat.StaminaPercent;
            mpBar.fillAmount = playerCombat.ManaPercent;

            // Update colors
            hpBar.color = GetHealthColor(playerCombat.HealthPercent);

            // Update labels
            hpLabel.text = $"HP: {Mathf.RoundToInt(playerCombat.HealthPercent * 100)}%";
            spLabel.text = $"SP: {Mathf.RoundToInt(playerCombat.StaminaPercent * 100)}%";
            mpLabel.text = $"MP: {Mathf.RoundToInt(playerCombat.ManaPercent * 100)}%";
        }
    }

    private Color GetHealthColor(float healthPercent)
    {
        if (healthPercent > 0.5f)
            return new Color(0, 1, 0.5f); // Green
        else if (healthPercent > 0.25f)
            return new Color(1, 1, 0); // Yellow
        else
            return new Color(1, 0.2f, 0.2f); // Red
    }

    public void OnMenuOptionSelected(string option)
    {
        if (option == "Log Out")
        {
            // TODO: Handle logout
            Debug.Log("Logging out...");
            return;
        }

        // Create submenu panel
        VRMenuPanel panel = menuSystem.CreateMenuPanel(option, new Vector3(panelWidth + 50f, 0, 0));
        panel.Show();
    }
}