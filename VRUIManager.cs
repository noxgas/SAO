#if STEAMVR_PRESENT
using UnityEngine;
using Valve.VR;

/// <summary>
/// VR-specific UI Manager for the entire game.
/// Handles all menus, panels, and UI state using SteamVR controllers.
/// SAO-inspired design with holographic feel.
/// </summary>
public class VRUIManager : MonoBehaviour
{
    private static VRUIManager instance;

    [Header("Main Menus")]
    [SerializeField] private VRMainMenuPanel mainMenuPanel;
    [SerializeField] private VRSettingsMenuPanel settingsMenuPanel;
    [SerializeField] private VRPlayerHUDPanel playerHUDPanel;
    [SerializeField] private VRInventoryPanel inventoryPanel;
    [SerializeField] private VRCharacterPanel characterPanel;
    [SerializeField] private VRPauseMenuPanel pauseMenuPanel;

    [Header("UI Settings")]
    [SerializeField] private Color saoBlueAccent = new Color(0, 1, 1); // Cyan
    [SerializeField] private Color saoRedAccent = new Color(1, 0.2f, 0.2f); // Red
    [SerializeField] private Color saoGreenAccent = new Color(0, 1, 0.5f); // Green
    [SerializeField] private float panelTransitionSpeed = 0.3f;

    [Header("VR Settings")]
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private float menuDistanceFromPlayer = 2f;

    private Canvas mainCanvas;
    private VRUIPanel currentPanel;
    private bool isPaused = false;

    // SteamVR Actions
    private SteamVR_Action_Boolean menuActivateAction;
    private SteamVR_Action_Boolean menuSelectAction;
    private SteamVR_Action_Boolean menuBackAction;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        mainCanvas = GetComponent<Canvas>();
        InitializeSteamVRActions();
        InitializePanels();
    }

    private void Start()
    {
        // Position canvas in front of player
        PositionMenuInFrontOfPlayer();
    }

    public static VRUIManager Instance => instance;

    private void InitializeSteamVRActions()
    {
        // Get SteamVR actions
        menuActivateAction = SteamVR_Actions.default_Grab;
        menuSelectAction = SteamVR_Actions.default_InteractUI;
        menuBackAction = SteamVR_Actions.default_Grab;
    }

    private void InitializePanels()
    {
        // Find or create panels
        mainMenuPanel = GetOrCreatePanel<VRMainMenuPanel>("VRMainMenuPanel");
        settingsMenuPanel = GetOrCreatePanel<VRSettingsMenuPanel>("VRSettingsMenuPanel");
        playerHUDPanel = GetOrCreatePanel<VRPlayerHUDPanel>("VRPlayerHUDPanel");
        inventoryPanel = GetOrCreatePanel<VRInventoryPanel>("VRInventoryPanel");
        characterPanel = GetOrCreatePanel<VRCharacterPanel>("VRCharacterPanel");
        pauseMenuPanel = GetOrCreatePanel<VRPauseMenuPanel>("VRPauseMenuPanel");

        // Hide all panels initially
        HideAllPanels();

        // Show HUD by default
        ShowPlayerHUD();
    }

    private T GetOrCreatePanel<T>(string panelName) where T : VRUIPanel
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

    /// <summary>
    /// Position the menu in front of the player's face.
    /// </summary>
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

    public void ShowPanel(VRUIPanel panel)
    {
        HideAllPanels();
        if (panel != null)
        {
            panel.Show();
            currentPanel = panel;
            PositionMenuInFrontOfPlayer();
        }
    }

    public void HidePanel(VRUIPanel panel)
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
        // Check for menu activation (right controller grip button)
        if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand))
        {
            if (isPaused)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        // Check for inventory (right controller trigger)
        if (SteamVR_Input.GetStateDown("InteractUI", SteamVR_Input_Sources.RightHand))
        {
            if (currentPanel is VRInventoryPanel)
                HidePanel(inventoryPanel);
            else if (!isPaused)
                ShowInventory();
        }

        // Position menu in front of player
        if (currentPanel != null && currentPanel.isVisible)
        {
            PositionMenuInFrontOfPlayer();
        }
    }
}

public enum AccentType
{
    Blue,
    Red,
    Green
}
#endif // STEAMVR_PRESENT
