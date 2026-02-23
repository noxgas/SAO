using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// SAO-style in-game HUD.  Builds itself entirely at runtime — no prefabs needed.
///
/// Layout (bottom-left corner, mirroring the SAO anime HUD)
/// ─────────────────────────────────────────────────────────
/// ┌────────────────────────────────────┐
/// │ KIRITO                      Lv. 1 │  ← name + level
/// │ HP ████████████░░░░░░  850/1000  │  ← green→red
/// │ SP █████████░░░░░░░░░  430/500   │  ← cyan
/// │ XP █████░░░░░░░░░░░░   250/1000  │  ← gold
/// └────────────────────────────────────┘
///
/// Add this component to your player / XR rig root.
/// It auto-finds PlayerCombat and Player.
/// </summary>
public class SAOHUDController : MonoBehaviour
{
    [Header("References (auto-found if empty)")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Player       player;

    [Header("HUD Tuning")]
    [SerializeField] private float panelWidth  = 380f;
    [SerializeField] private float panelHeight = 120f;
    [SerializeField] private float marginLeft  = 24f;
    [SerializeField] private float marginBottom = 24f;

    // Generated UI references
    private Canvas         hudCanvas;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI levelText;
    private Image          hpFill;
    private Image          spFill;
    private Image          xpFill;
    private TextMeshProUGUI hpValueText;
    private TextMeshProUGUI spValueText;

    // Fake XP for prototype (no real XP system yet)
    private float fakeXP      = 250f;
    private float fakeXPMax   = 1000f;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        if (playerCombat == null) playerCombat = FindObjectOfType<PlayerCombat>();
        if (player       == null) player       = FindObjectOfType<Player>();

        BuildHUD();
    }

    private void Update()
    {
        RefreshHUD();
    }

    // ── Build ─────────────────────────────────────────────────────────────────

    private void BuildHUD()
    {
        // ── Canvas ──
        hudCanvas = SAOStyler.CreateOverlayCanvas("SAO_HUD", sortOrder: 50);

        // ── Main panel ──
        var panel = SAOStyler.CreatePanel(hudCanvas.transform, "HUDPanel",
            anchorMin : new Vector2(0, 0),
            anchorMax : new Vector2(0, 0),
            offsetMin : new Vector2(marginLeft, marginBottom),
            offsetMax : new Vector2(marginLeft + panelWidth, marginBottom + panelHeight));
        SAOStyler.AddBorder(panel, thickness: 1f);

        // Padding container
        var inner = new GameObject("Inner");
        inner.transform.SetParent(panel, false);
        var innerRT = inner.AddComponent<RectTransform>();
        innerRT.anchorMin = Vector2.zero;
        innerRT.anchorMax = Vector2.one;
        innerRT.offsetMin = new Vector2(10f, 6f);
        innerRT.offsetMax = new Vector2(-6f, -6f);

        // Row heights (bottom to top)
        float totalInnerH = panelHeight - 12f;
        float row0Bot = 0f;
        float row0Top = totalInnerH * 0.28f;   // XP
        float row1Bot = row0Top + 2f;
        float row1Top = totalInnerH * 0.56f;   // SP
        float row2Bot = row1Top + 2f;
        float row2Top = totalInnerH * 0.78f;   // HP
        float row3Bot = row2Top + 2f;
        float row3Top = totalInnerH;           // Name + Level

        // ── Name & Level row ──
        nameText = SAOStyler.CreateText(inner.transform, "NameText", "PLAYER",
            fontSize: 16f, color: SAOStyler.TextPrimary,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0.7f, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row3Bot)),
            offsetMax: new Vector2(0, -(totalInnerH - row3Top)),
            alignment: TextAlignmentOptions.MidlineLeft);
        nameText.fontStyle = FontStyles.Bold;

        levelText = SAOStyler.CreateText(inner.transform, "LevelText", "Lv. 1",
            fontSize: 14f, color: SAOStyler.TextLabel,
            anchorMin: new Vector2(0.7f, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row3Bot)),
            offsetMax: new Vector2(0, -(totalInnerH - row3Top)),
            alignment: TextAlignmentOptions.MidlineRight);

        // ── HP bar ──
        float barLabelFontSize = 12f;
        hpFill = SAOStyler.CreateStatBar(inner.transform, "HP", SAOStyler.HPColorHigh,
            "HP", barLabelFontSize,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row2Bot)),
            offsetMax: new Vector2(0, -(totalInnerH - row2Top)));

        // HP value text (overlaid right-side of bar)
        hpValueText = SAOStyler.CreateText(inner.transform, "HPValue", "",
            fontSize: 11f, color: SAOStyler.TextPrimary,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row2Bot)),
            offsetMax: new Vector2(-2f, -(totalInnerH - row2Top)),
            alignment: TextAlignmentOptions.MidlineRight);

        // ── SP bar ──
        spFill = SAOStyler.CreateStatBar(inner.transform, "SP", SAOStyler.SPColor,
            "SP", barLabelFontSize,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row1Bot)),
            offsetMax: new Vector2(0, -(totalInnerH - row1Top)));

        spValueText = SAOStyler.CreateText(inner.transform, "SPValue", "",
            fontSize: 11f, color: SAOStyler.TextPrimary,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row1Bot)),
            offsetMax: new Vector2(-2f, -(totalInnerH - row1Top)),
            alignment: TextAlignmentOptions.MidlineRight);

        // ── XP bar ──
        xpFill = SAOStyler.CreateStatBar(inner.transform, "XP", SAOStyler.XPColor,
            "XP", barLabelFontSize,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            offsetMin: new Vector2(0, -(totalInnerH - row0Bot)),
            offsetMax: new Vector2(0, -(totalInnerH - row0Top)));
    }

    // ── Refresh every frame ───────────────────────────────────────────────────

    private void RefreshHUD()
    {
        if (playerCombat == null) return;

        float hp  = playerCombat.HealthPercent;
        float sp  = playerCombat.StaminaPercent;

        // HP
        if (hpFill != null)
        {
            hpFill.fillAmount = hp;
            hpFill.color = SAOStyler.GetHPColor(hp);
        }
        if (hpValueText != null)
            hpValueText.text = $"{Mathf.RoundToInt(hp * 100)}%";

        // SP
        if (spFill != null)
            spFill.fillAmount = sp;
        if (spValueText != null)
            spValueText.text = $"{Mathf.RoundToInt(sp * 100)}%";

        // XP (fake – just bounces slightly for demo)
        if (xpFill != null)
            xpFill.fillAmount = fakeXP / fakeXPMax;

        // Name / Level
        if (nameText != null && player != null)
            nameText.text = player.name.ToUpper();

        if (levelText != null && player != null)
            levelText.text = $"Lv. {player.Level}";
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Award XP (prototype helper — real game uses levelling system).</summary>
    public void AddXP(float amount)
    {
        fakeXP = Mathf.Min(fakeXP + amount, fakeXPMax);
    }
}
