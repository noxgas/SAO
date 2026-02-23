using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SAO-style in-game menu system.  Builds itself entirely at runtime — no prefabs,
/// no SteamVR SDK required.
///
/// Open / close with Y button (Quest 2) or M key (desktop).
///
/// Menu layout (vertical list, centred, floating in front of player)
/// ─────────────────────────────────────────────────────────────────
/// ╔══════════════════════════╗
/// ║  MENU         [Floor: 1] ║  ← title row
/// ╠══════════════════════════╣
/// ║ ▌ Status                 ║
/// ║ ▌ Inventory              ║
/// ║ ▌ Equipment              ║
/// ║ ▌ Skills                 ║
/// ║ ▌ Map                    ║
/// ║ ▌ Party                  ║
/// ║ ▌ Quest Log              ║
/// ║ ▌ Settings               ║
/// ╠══════════════════════════╣
/// ║ ▌ Log Out                ║
/// ╚══════════════════════════╝
///
/// Selecting an option opens a detail sub-panel to the right.
/// </summary>
public class SAOMenuController : MonoBehaviour
{
    [Header("References (auto-found if empty)")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Player       player;
    [SerializeField] private Camera       headCamera;

    [Header("Menu Positioning")]
    [Tooltip("Distance in front of the camera the menu spawns (metres).")]
    [SerializeField] private float menuDistance  = 1.2f;
    [Tooltip("Vertical offset (metres) relative to camera.")]
    [SerializeField] private float menuVertOffset = -0.05f;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    // ── State ─────────────────────────────────────────────────────────────────
    private bool   menuOpen;
    private Canvas menuCanvas;   // World-space canvas that moves with the player

    // Main menu panel
    private GameObject mainPanel;
    private CanvasGroup mainCG;

    // Sub-panel
    private GameObject subPanel;
    private CanvasGroup subCG;
    private TextMeshProUGUI subTitle;
    private Transform      subContent;

    // Animation
    private Coroutine animCoroutine;

    // Active sub-panel name
    private string activeSubPanel = "";

    // ── Singleton (so Quest2InputHandler can call ToggleMenu) ─────────────────
    private static SAOMenuController _instance;
    public static SAOMenuController Instance => _instance;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        if (playerCombat == null) playerCombat = FindObjectOfType<PlayerCombat>();
        if (player       == null) player       = FindObjectOfType<Player>();
        if (headCamera   == null) headCamera   = Camera.main;

        BuildMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleMenu();

        if (menuOpen && menuCanvas != null)
            PositionMenu();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void ToggleMenu()
    {
        if (menuOpen) CloseMenu();
        else           OpenMenu();
    }

    public void OpenMenu()
    {
        if (menuOpen) return;
        menuOpen = true;
        Time.timeScale = 0f;
        menuCanvas.gameObject.SetActive(true);
        PositionMenu();
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimatePanel(mainCG, mainPanel.transform, show: true));
    }

    public void CloseMenu()
    {
        if (!menuOpen) return;
        menuOpen = false;
        Time.timeScale = 1f;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimatePanel(mainCG, mainPanel.transform, show: false,
            onDone: () => menuCanvas.gameObject.SetActive(false)));

        // Also close sub-panel
        CloseSubPanel();
    }

    // ── Menu construction ─────────────────────────────────────────────────────

    private void BuildMenu()
    {
        // World-space canvas – follows camera each frame when open.
        // This is a single-scene prototype so DontDestroyOnLoad is not needed.
        var canvasGO = new GameObject("SAO_MenuCanvas");
        menuCanvas = canvasGO.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.WorldSpace;
        menuCanvas.sortingOrder = 200;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var cRT = menuCanvas.GetComponent<RectTransform>();
        cRT.sizeDelta = new Vector2(100f, 60f);
        cRT.localScale = new Vector3(0.003f, 0.003f, 0.003f);

        // ── Main panel ───────────────────────────────────────────────────────
        BuildMainPanel();

        // ── Sub-panel (hidden initially) ─────────────────────────────────────
        BuildSubPanel();

        menuCanvas.gameObject.SetActive(false);
    }

    private void BuildMainPanel()
    {
        float pW = 280f;   // pixels in canvas space
        float pH = 520f;

        mainPanel = new GameObject("MainPanel");
        mainPanel.transform.SetParent(menuCanvas.transform, false);
        var bg = mainPanel.AddComponent<Image>();
        bg.color = SAOStyler.BackgroundColor;
        var rt = mainPanel.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-70f, 0f);
        rt.sizeDelta = new Vector2(pW, pH);
        SAOStyler.AddBorder(rt, thickness: 1.5f);

        mainCG = mainPanel.AddComponent<CanvasGroup>();
        mainCG.alpha = 0f;
        mainPanel.transform.localScale = Vector3.one * 0.9f;

        // Title row
        var titleRow = NewRectGO("TitleRow", mainPanel.transform,
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, -50), new Vector2(0, 0));
        var titleBg = titleRow.AddComponent<Image>();
        titleBg.color = new Color(SAOStyler.BorderColor.r, SAOStyler.BorderColor.g,
                                  SAOStyler.BorderColor.b, 0.15f);

        SAOStyler.CreateText(titleRow.transform, "Title", "MENU",
            24f, SAOStyler.TextPrimary,
            new Vector2(0, 0), new Vector2(0.55f, 1),
            new Vector2(14f, 0), new Vector2(0, 0),
            TextAlignmentOptions.MidlineLeft).fontStyle = FontStyles.Bold;

        SAOStyler.CreateText(titleRow.transform, "Floor", "Floor: 1",
            14f, SAOStyler.TextLabel,
            new Vector2(0.55f, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(-10f, 0),
            TextAlignmentOptions.MidlineRight);

        // Status mini-bar (HP inside the title, SAO style)
        var miniHP = SAOStyler.CreateStatBar(mainPanel.transform, "MiniHP", SAOStyler.HPColorHigh,
            "HP", 11f,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(10f, -74f), offsetMax: new Vector2(-10f, -54f));
        // store for later refresh
        _miniHPFill = miniHP;

        // Divider
        CreateHLine(mainPanel.transform, new Vector2(6f, -78f), pW - 12f);

        // Menu buttons
        string[] options =
        {
            "Status", "Inventory", "Equipment", "Skills",
            "Map", "Party", "Quest Log", "Settings"
        };

        float btnH  = 40f;
        float startY = -86f;
        for (int i = 0; i < options.Length; i++)
        {
            string opt = options[i];
            float y = startY - i * (btnH + 2f);
            var btn = SAOStyler.CreateMenuButton(mainPanel.transform, opt, 15f,
                anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
                offsetMin: new Vector2(8f, y - btnH),
                offsetMax: new Vector2(-8f, y));
            btn.onClick.AddListener(() => OpenSubPanel(opt));
        }

        // Divider before Log Out
        float logOutY = startY - options.Length * (btnH + 2f) - 4f;
        CreateHLine(mainPanel.transform, new Vector2(6f, logOutY), pW - 12f);

        var logOutBtn = SAOStyler.CreateMenuButton(mainPanel.transform, "Log Out", 15f,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(8f, logOutY - 6f - btnH),
            offsetMax: new Vector2(-8f, logOutY - 6f));
        logOutBtn.onClick.AddListener(OnLogOut);
        // Make Log Out text red
        var logOutTxt = logOutBtn.GetComponentInChildren<TextMeshProUGUI>();
        if (logOutTxt != null) logOutTxt.color = SAOStyler.HPColorLow;
    }

    private Image _miniHPFill;   // kept alive between frames for the mini HP bar

    private void BuildSubPanel()
    {
        float pW = 320f;
        float pH = 520f;

        subPanel = new GameObject("SubPanel");
        subPanel.transform.SetParent(menuCanvas.transform, false);
        var bg = subPanel.AddComponent<Image>();
        bg.color = SAOStyler.BackgroundColor;
        var rt = subPanel.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(100f, 0f);
        rt.sizeDelta = new Vector2(pW, pH);
        SAOStyler.AddBorder(rt, thickness: 1.5f);

        subCG = subPanel.AddComponent<CanvasGroup>();
        subCG.alpha = 0f;
        subPanel.transform.localScale = Vector3.one * 0.9f;

        // Title row
        var titleRow = NewRectGO("SubTitleRow", subPanel.transform,
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, -50), new Vector2(0, 0));
        var titleBg = titleRow.AddComponent<Image>();
        titleBg.color = new Color(SAOStyler.BorderColor.r, SAOStyler.BorderColor.g,
                                  SAOStyler.BorderColor.b, 0.15f);

        subTitle = SAOStyler.CreateText(titleRow.transform, "SubTitle", "",
            22f, SAOStyler.TextPrimary,
            new Vector2(0, 0), new Vector2(1, 1),
            new Vector2(14f, 0), new Vector2(-10f, 0),
            TextAlignmentOptions.MidlineLeft);
        subTitle.fontStyle = FontStyles.Bold;

        CreateHLine(subPanel.transform, new Vector2(6f, -52f), pW - 12f);

        // Content container
        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(subPanel.transform, false);
        subContent = contentGO.transform;
        var contentRT = contentGO.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 0);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.offsetMin = new Vector2(10f, 10f);
        contentRT.offsetMax = new Vector2(-10f, -56f);

        subPanel.SetActive(false);
    }

    // ── Sub-panel logic ───────────────────────────────────────────────────────

    private void OpenSubPanel(string option)
    {
        if (activeSubPanel == option)
        {
            CloseSubPanel();
            return;
        }

        activeSubPanel = option;
        subTitle.text = option.ToUpper();

        // Clear previous content
        foreach (Transform child in subContent)
            Destroy(child.gameObject);

        PopulateSubPanel(option);

        subPanel.SetActive(true);
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimatePanel(subCG, subPanel.transform, show: true));
    }

    private void CloseSubPanel()
    {
        if (!subPanel.activeSelf) return;
        activeSubPanel = "";
        StartCoroutine(AnimatePanel(subCG, subPanel.transform, show: false,
            onDone: () => subPanel.SetActive(false)));
    }

    private void PopulateSubPanel(string option)
    {
        switch (option)
        {
            case "Status":    PopulateStatus();    break;
            case "Inventory": PopulateInventory(); break;
            case "Equipment": PopulateEquipment(); break;
            case "Skills":    PopulateSkills();    break;
            case "Map":       PopulateMap();       break;
            case "Party":     PopulateParty();     break;
            case "Quest Log": PopulateQuestLog();  break;
            case "Settings":  PopulateSettings();  break;
            default:          PopulateComingSoon(option); break;
        }
    }

    // ── Sub-panel content ─────────────────────────────────────────────────────

    private void PopulateStatus()
    {
        float lh = 28f;
        float y  = 0f;

        AddSubRow("NAME",   player != null ? player.name : "Player", ref y, lh);
        AddSubRow("LEVEL",  player != null ? player.Level.ToString() : "1",  ref y, lh);
        AddSubRow("HP",     playerCombat != null ? $"{Mathf.RoundToInt(playerCombat.HealthPercent*100)}%" : "—", ref y, lh);
        AddSubRow("SP",     playerCombat != null ? $"{Mathf.RoundToInt(playerCombat.StaminaPercent*100)}%" : "—", ref y, lh);
        AddSubRow("FLOOR",  "1",         ref y, lh);
        AddSubRow("COL",    "0",         ref y, lh);
        AddSubRow("GUILD",  "None",      ref y, lh);
    }

    private void PopulateInventory()
    {
        AddSubLabel("(No items yet — pick up items to see them here.)", 13f);
    }

    private void PopulateEquipment()
    {
        float lh = 28f; float y = 0f;
        string[] slots = { "WEAPON", "OFF-HAND", "HEAD", "CHEST", "LEGS", "BOOTS", "ACCESSORY" };
        foreach (var s in slots)
            AddSubRow(s, "—", ref y, lh);
    }

    private void PopulateSkills()
    {
        float lh = 28f; float y = 0f;
        AddSubRow("SWORD",     "—",  ref y, lh);
        AddSubRow("ONE-HAND",  "—",  ref y, lh);
        AddSubRow("TWO-HAND",  "—",  ref y, lh);
        AddSubRow("PARRY",     "—",  ref y, lh);
        AddSubRow("SPRINT",    "—",  ref y, lh);
    }

    private void PopulateMap()
    {
        AddSubLabel("Procedurally generated map.\n\nYou are in the Forest biome.\nFloor: 1", 13f);
    }

    private void PopulateParty()
    {
        AddSubLabel("No party members.\nInvite other players to form a party.", 13f);
    }

    private void PopulateQuestLog()
    {
        AddSubLabel("[ ACTIVE QUESTS ]\n\n• Defeat 5 enemies  (0/5)\n• Explore the map\n• Survive", 13f);
    }

    private void PopulateSettings()
    {
        float lh = 28f; float y = 0f;
        AddSubRow("GRAPHICS",  "Medium", ref y, lh);
        AddSubRow("AUDIO",     "100%",   ref y, lh);
        AddSubRow("HAPTICS",   "On",     ref y, lh);
        AddSubRow("SNAP TURN", "45°",    ref y, lh);
    }

    private void PopulateComingSoon(string name)
    {
        AddSubLabel($"{name} — Coming soon.", 14f);
    }

    // ── Sub-panel helpers ─────────────────────────────────────────────────────

    private void AddSubRow(string label, string value, ref float yOffset, float lineH)
    {
        var row = new GameObject("Row_" + label);
        row.transform.SetParent(subContent, false);
        var rowRT = row.AddComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0, 1);
        rowRT.anchorMax = new Vector2(1, 1);
        rowRT.offsetMin = new Vector2(0, -yOffset - lineH);
        rowRT.offsetMax = new Vector2(0, -yOffset);

        SAOStyler.CreateText(row.transform, "Label", label, 12f, SAOStyler.TextLabel,
            new Vector2(0, 0), new Vector2(0.4f, 1),
            Vector2.zero, Vector2.zero, TextAlignmentOptions.MidlineLeft);

        SAOStyler.CreateText(row.transform, "Value", value, 12f, SAOStyler.TextPrimary,
            new Vector2(0.4f, 0), new Vector2(1, 1),
            Vector2.zero, Vector2.zero, TextAlignmentOptions.MidlineLeft);

        // thin separator
        CreateHLine(subContent.GetComponent<RectTransform>(),
            new Vector2(0, -yOffset - lineH + 1f), 280f, 0.25f);

        yOffset += lineH;
    }

    private void AddSubLabel(string text, float fontSize)
    {
        var go = new GameObject("InfoText");
        go.transform.SetParent(subContent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = SAOStyler.TextPrimary;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.enableWordWrapping = true;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(4f, 4f);
        rt.offsetMax = new Vector2(-4f, -4f);
    }

    // ── Animation ─────────────────────────────────────────────────────────────

    private IEnumerator AnimatePanel(CanvasGroup cg, Transform panelTf,
        bool show, System.Action onDone = null)
    {
        float duration = 0.2f;
        float t = 0f;

        float startAlpha = show ? 0f : 1f;
        float endAlpha   = show ? 1f : 0f;
        Vector3 startScale = show ? Vector3.one * 0.88f : Vector3.one;
        Vector3 endScale   = show ? Vector3.one : Vector3.one * 0.88f;

        // Use unscaled time so it works when timeScale == 0
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            float ease = show ? EaseOutCubic(p) : EaseInCubic(p);
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, ease);
            panelTf.localScale = Vector3.Lerp(startScale, endScale, ease);
            yield return null;
        }

        cg.alpha = endAlpha;
        panelTf.localScale = endScale;
        onDone?.Invoke();
    }

    private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
    private static float EaseInCubic(float t)  => t * t * t;

    // ── Menu positioning ──────────────────────────────────────────────────────

    private void PositionMenu()
    {
        if (headCamera == null) return;
        Vector3 forward = headCamera.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = headCamera.transform.forward;
        forward.Normalize();

        menuCanvas.transform.position =
            headCamera.transform.position
            + forward * menuDistance
            + Vector3.up * menuVertOffset;

        menuCanvas.transform.rotation =
            Quaternion.LookRotation(forward);
    }

    // ── Log Out ───────────────────────────────────────────────────────────────

    private void OnLogOut()
    {
        Debug.Log("[SAOMenu] Log Out – (prototype: returning to menu not implemented)");
        CloseMenu();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Mini-HP update ────────────────────────────────────────────────────────

    private void LateUpdate()
    {
        if (!menuOpen || playerCombat == null) return;
        if (_miniHPFill == null) return;
        float hp = playerCombat.HealthPercent;
        _miniHPFill.fillAmount = hp;
        _miniHPFill.color = SAOStyler.GetHPColor(hp);
    }

    // ── Utility helpers ───────────────────────────────────────────────────────

    private static RectTransform NewRectGO(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        return rt;
    }

    private static void CreateHLine(Transform parent, Vector2 anchoredPos,
        float width, float alpha = 0.5f)
    {
        var go = new GameObject("HLine");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(SAOStyler.BorderColor.r,
                              SAOStyler.BorderColor.g,
                              SAOStyler.BorderColor.b, alpha);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot     = new Vector2(0, 1);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(width, 1f);
    }
}
