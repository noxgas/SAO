using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central VR UI Manager for the entire game.
/// Handles all menus, panels, and UI state.
/// SAO-inspired design with holographic feel.
/// </summary>
public class VRMenuSystem : MonoBehaviour
{
    private static VRMenuSystem instance;

    [Header("Main Menus")]
    [SerializeField] private VRMainMenuPanel mainMenuPanel;
    [SerializeField] private VRSettingsPanel settingsMenuPanel;
    [SerializeField] private VRPlayerHUDPanel playerHUDPanel;
    [SerializeField] private VRInventoryPanel inventoryPanel;
    [SerializeField] private VRCharacterPanel characterPanel;
    [SerializeField] private VRPauseMenuPanel pauseMenuPanel;
    [SerializeField] private SkillTreePanel skillTreePanel;
    [SerializeField] private VRSkillsPanel skillsPanel;
    [SerializeField] private VRMapPanel mapPanel;
    [SerializeField] private VRPartyPanel partyPanel;
    [SerializeField] private VRQuestLogPanel questLogPanel;

    [Header("UI Settings")]
    [SerializeField] private Color saoBlueAccent = new Color(0, 1, 1);
    [SerializeField] private Color saoRedAccent = new Color(1, 0.2f, 0.2f);
    [SerializeField] private Color saoGreenAccent = new Color(0, 1, 0.5f);
    [SerializeField] private float panelTransitionSpeed = 0.3f;

    [Header("VR Settings")]
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private float menuDistanceFromPlayer = 2f;

    private Canvas mainCanvas;
    private VRMenuPanel currentPanel;
    private bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        mainCanvas = GetComponent<Canvas>();
        InitializePanels();
    }

    private void Start()
    {
        PositionMenuInFrontOfPlayer();
    }

    public static VRMenuSystem Instance => instance;

    private void InitializePanels()
    {
        mainMenuPanel = GetOrCreatePanel<VRMainMenuPanel>("VRMainMenuPanel");
        settingsMenuPanel = GetOrCreatePanel<VRSettingsPanel>("VRSettingsPanel");
        playerHUDPanel = GetOrCreatePanel<VRPlayerHUDPanel>("VRPlayerHUDPanel");
        inventoryPanel = GetOrCreatePanel<VRInventoryPanel>("VRInventoryPanel");
        characterPanel = GetOrCreatePanel<VRCharacterPanel>("VRCharacterPanel");
        pauseMenuPanel = GetOrCreatePanel<VRPauseMenuPanel>("VRPauseMenuPanel");
        skillTreePanel = GetOrCreatePanel<SkillTreePanel>("SkillTreePanel");
        skillsPanel = GetOrCreatePanel<VRSkillsPanel>("VRSkillsPanel");
        mapPanel = GetOrCreatePanel<VRMapPanel>("VRMapPanel");
        partyPanel = GetOrCreatePanel<VRPartyPanel>("VRPartyPanel");
        questLogPanel = GetOrCreatePanel<VRQuestLogPanel>("VRQuestLogPanel");

        HideAllPanels();
        ShowPlayerHUD();
    }

    private T GetOrCreatePanel<T>(string panelName) where T : VRMenuPanel
    {
        Transform panelTransform = transform.Find(panelName);
        if (panelTransform != null)
        {
            T panel = panelTransform.GetComponent<T>();
            if (panel != null) return panel;
        }

        GameObject panelGO = new GameObject(panelName);
        panelGO.transform.SetParent(transform);
        T newPanel = panelGO.AddComponent<T>();
        return newPanel;
    }

    private void PositionMenuInFrontOfPlayer()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 menuPosition = cameraTransform.position + cameraTransform.forward * menuDistanceFromPlayer;
        mainCanvas.transform.position = menuPosition;
        mainCanvas.transform.rotation = Quaternion.LookRotation(mainCanvas.transform.position - cameraTransform.position);
    }

    public void ShowMainMenu() => ShowPanel(mainMenuPanel);
    public void ShowSettings() => ShowPanel(settingsMenuPanel);
    public void ShowInventory() => ShowPanel(inventoryPanel);
    public void ShowCharacter() => ShowPanel(characterPanel);
    public void ShowSkillTree() => ShowPanel(skillTreePanel);
    public void ShowSkills() => ShowPanel(skillsPanel);
    public void ShowMap() => ShowPanel(mapPanel);
    public void ShowParty() => ShowPanel(partyPanel);
    public void ShowQuestLog() => ShowPanel(questLogPanel);
    public void ShowPlayerHUD() => playerHUDPanel.Show();

    public void ShowPauseMenu()
    {
        ShowPanel(pauseMenuPanel);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void HidePauseMenu()
    {
        HidePanel(pauseMenuPanel);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void ShowPanel(VRMenuPanel panel)
    {
        HideAllPanels();
        if (panel != null)
        {
            panel.Show();
            currentPanel = panel;
            PositionMenuInFrontOfPlayer();
        }
    }

    public void HidePanel(VRMenuPanel panel)
    {
        if (panel != null)
            panel.Hide();
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.Hide();
        if (settingsMenuPanel != null) settingsMenuPanel.Hide();
        if (inventoryPanel != null) inventoryPanel.Hide();
        if (characterPanel != null) characterPanel.Hide();
        if (pauseMenuPanel != null) pauseMenuPanel.Hide();
        if (skillTreePanel != null) skillTreePanel.Hide();
        if (skillsPanel != null) skillsPanel.Hide();
        if (mapPanel != null) mapPanel.Hide();
        if (partyPanel != null) partyPanel.Hide();
        if (questLogPanel != null) questLogPanel.Hide();
    }

    public Color GetAccentColor(AccentType type)
    {
        return type switch
        {
            AccentType.Blue => saoBlueAccent,
            AccentType.Red => saoRedAccent,
            AccentType.Green => saoGreenAccent,
            _ => saoBlueAccent
        };
    }

    private void Update()
    {
        if (currentPanel != null && currentPanel.isVisible)
        {
            PositionMenuInFrontOfPlayer();
        }
    }

    public Player GetCurrentPlayer() => FindObjectOfType<Player>();
}

public enum AccentType
{
    Blue,
    Red,
    Green
}