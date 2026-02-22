using UnityEngine;
using Valve.VR;
using System.Collections.Generic;

/// <summary>
/// Main VR Menu System - SAO-inspired holographic interface.
/// Handles all menu panels, interactions, and animations.
/// </summary>
public class VRMenuSystem : MonoBehaviour
{
    private static VRMenuSystem instance;

    [Header("Menu Canvas")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private float menuSpawnDistance = 0.5f;
    [SerializeField] private Vector3 menuSpawnOffset = Vector3.zero;

    [Header("Panel Prefabs")]
    [SerializeField] private VRMenuPanel statusPanelPrefab;
    [SerializeField] private VRMenuPanel inventoryPanelPrefab;
    [SerializeField] private VRMenuPanel equipmentPanelPrefab;
    [SerializeField] private VRMenuPanel skillsPanelPrefab;
    [SerializeField] private VRMenuPanel mapPanelPrefab;
    [SerializeField] private VRMenuPanel partyPanelPrefab;
    [SerializeField] private VRMenuPanel guildPanelPrefab;
    [SerializeField] private VRMenuPanel questLogPanelPrefab;
    [SerializeField] private VRMenuPanel settingsPanelPrefab;

    [Header("Menu Settings")]
    [SerializeField] private float panelFadeInDuration = 0.2f;
    [SerializeField] private float panelScaleAnimationDuration = 0.3f;
    [SerializeField] private Color holographicBlue = new Color(0.227f, 0.627f, 1f); // #3aa0ff
    [SerializeField] private float holographicGlowIntensity = 1.2f;

    [Header("VR Input")]
    [SerializeField] private SteamVR_Input_Sources controllerHand = SteamVR_Input_Sources.RightHand;

    private Dictionary<string, VRMenuPanel> openPanels = new Dictionary<string, VRMenuPanel>();
    private VRMenuPanel mainMenuPanel;
    private bool isMenuOpen = false;

    // SteamVR Actions
    private SteamVR_Action_Boolean menuToggleAction;
    private SteamVR_Action_Pose poseAction;

    private Camera mainCamera;
    private Player currentPlayer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        mainCamera = Camera.main;
    }

    private void Start()
    {
        InitializeSteamVRActions();
        currentPlayer = FindObjectOfType<Player>();
        InitializeMenuCanvas();
    }

    public static VRMenuSystem Instance => instance;

    private void InitializeSteamVRActions()
    {
        menuToggleAction = SteamVR_Actions.default_Grab; // Grip button
        poseAction = SteamVR_Actions.default_Pose;
    }

    private void InitializeMenuCanvas()
    {
        if (menuCanvas == null)
        {
            GameObject canvasGO = new GameObject("VRMenuCanvas");
            canvasGO.transform.SetParent(transform);
            menuCanvas = canvasGO.AddComponent<Canvas>();
        }

        menuCanvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = menuCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1920, 1080);
    }

    private void Update()
    {
        // Toggle menu with grip button
        if (SteamVR_Input.GetStateDown("GrabGrip", controllerHand))
        {
            ToggleMenu();
        }

        // Update menu position to follow player view
        if (isMenuOpen)
        {
            UpdateMenuPosition();
        }
    }

    public void ToggleMenu()
    {
        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    public void OpenMenu()
    {
        if (isMenuOpen) return;

        isMenuOpen = true;
        Time.timeScale = 0f; // Pause game

        // Create or show main menu panel
        if (mainMenuPanel == null)
        {
            mainMenuPanel = CreateMenuPanel("MainMenu", Vector3.zero);
        }

        mainMenuPanel.Show();
    }

    public void CloseMenu()
    {
        if (!isMenuOpen) return;

        isMenuOpen = false;
        Time.timeScale = 1f; // Resume game

        // Close all panels
        foreach (var panel in openPanels.Values)
        {
            panel.Hide();
        }
        openPanels.Clear();
    }

    private void UpdateMenuPosition()
    {
        // Position menu 0.5m in front of player
        Vector3 menuPosition = mainCamera.transform.position + mainCamera.transform.forward * menuSpawnDistance + menuSpawnOffset;
        menuCanvas.transform.position = menuPosition;
        menuCanvas.transform.rotation = Quaternion.LookRotation(menuCanvas.transform.position - mainCamera.transform.position);
    }

    public VRMenuPanel CreateMenuPanel(string panelType, Vector3 offset)
    {
        VRMenuPanel prefab = panelType switch
        {
            "Status" => statusPanelPrefab,
            "Inventory" => inventoryPanelPrefab,
            "Equipment" => equipmentPanelPrefab,
            "Skills" => skillsPanelPrefab,
            "Map" => mapPanelPrefab,
            "Party" => partyPanelPrefab,
            "Guild" => guildPanelPrefab,
            "QuestLog" => questLogPanelPrefab,
            "Settings" => settingsPanelPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"Panel prefab not found: {panelType}");
            return null;
        }

        VRMenuPanel panel = Instantiate(prefab, menuCanvas.transform);
        panel.Initialize(this, panelType, offset);

        if (!openPanels.ContainsKey(panelType))
        {
            openPanels[panelType] = panel;
        }

        return panel;
    }

    public void ClosePanel(string panelType)
    {
        if (openPanels.TryGetValue(panelType, out var panel))
        {
            panel.Hide();
            openPanels.Remove(panelType);
        }
    }

    public Player GetCurrentPlayer() => currentPlayer;
}