using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Central UI Manager for the entire game.
/// Handles all menus, panels, and UI state.
/// SAO-inspired design with holographic feel.
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("Main Menus")]
    [SerializeField] private MainMenuPanel mainMenuPanel;
    [SerializeField] private SettingsMenuPanel settingsMenuPanel;
    [SerializeField] private PlayerHUDPanel playerHUDPanel;
    [SerializeField] private InventoryPanel inventoryPanel;
    [SerializeField] private CharacterPanel characterPanel;
    [SerializeField] private PauseMenuPanel pauseMenuPanel;

    [Header("UI Settings")]
    [SerializeField] private Color saoBlueAccent = new Color(0, 1, 1); // Cyan
    [SerializeField] private Color saoRedAccent = new Color(1, 0.2f, 0.2f); // Red
    [SerializeField] private Color saoGreenAccent = new Color(0, 1, 0.5f); // Green
    [SerializeField] private float panelTransitionSpeed = 0.3f;

    private Canvas mainCanvas;
    private UIPanel currentPanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        mainCanvas = GetComponent<Canvas>();
        InitializePanels();
    }

    public static UIManager Instance => instance;

    private void InitializePanels()
    {
        // Find or create panels
        mainMenuPanel = GetOrCreatePanel<MainMenuPanel>("MainMenuPanel");
        settingsMenuPanel = GetOrCreatePanel<SettingsMenuPanel>("SettingsMenuPanel");
        playerHUDPanel = GetOrCreatePanel<PlayerHUDPanel>("PlayerHUDPanel");
        inventoryPanel = GetOrCreatePanel<InventoryPanel>("InventoryPanel");
        characterPanel = GetOrCreatePanel<CharacterPanel>("CharacterPanel");
        pauseMenuPanel = GetOrCreatePanel<PauseMenuPanel>("PauseMenuPanel");

        // Hide all panels initially
        HideAllPanels();
    }

    private T GetOrCreatePanel<T>(string panelName) where T : UIPanel
    {
        Transform panelTransform = transform.Find(panelName);
        if (panelTransform != null)
        {
            T panel = panelTransform.GetComponent<T>();
            if (panel != null) return panel;
        }

        // Create new panel if not found
        GameObject panelGO = new GameObject(panelName);
        panelGO.transform.SetParent(transform);
        T newPanel = panelGO.AddComponent<T>();
        return newPanel;
    }

    public void ShowMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    public void ShowSettings()
    {
        ShowPanel(settingsMenuPanel);
    }

    public void ShowInventory()
    {
        ShowPanel(inventoryPanel);
    }

    public void ShowCharacter()
    {
        ShowPanel(characterPanel);
    }

    public void ShowPauseMenu()
    {
        ShowPanel(pauseMenuPanel);
        Time.timeScale = 0f; // Pause game
    }

    public void HidePauseMenu()
    {
        HidePanel(pauseMenuPanel);
        Time.timeScale = 1f; // Resume game
    }

    public void ShowPanel(UIPanel panel)
    {
        HideAllPanels();
        if (panel != null)
        {
            panel.Show();
            currentPanel = panel;
        }
    }

    public void HidePanel(UIPanel panel)
    {
        if (panel != null)
            panel.Hide();
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.Hide();
        if (settingsMenuPanel != null) settingsMenuPanel.Hide();
        if (playerHUDPanel != null) playerHUDPanel.Hide();
        if (inventoryPanel != null) inventoryPanel.Hide();
        if (characterPanel != null) characterPanel.Hide();
        if (pauseMenuPanel != null) pauseMenuPanel.Hide();
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
        // Toggle pause menu with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPanel is PauseMenuPanel)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        // Toggle inventory with I
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (currentPanel is InventoryPanel)
                HidePanel(inventoryPanel);
            else
                ShowInventory();
        }

        // Toggle character with C
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentPanel is CharacterPanel)
                HidePanel(characterPanel);
            else
                ShowCharacter();
        }
    }
}

public enum AccentType
{
    Blue,
    Red,
    Green
}