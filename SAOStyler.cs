using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Static factory helpers that apply the Sword Art Online holographic visual style.
///
/// SAO colour palette
/// ──────────────────
/// Background   : #020A18  (very dark navy, 80 % opaque)
/// Border / glow: #00D4FF  (bright cyan)
/// HP bar       : #00FF88  (green) → #FFFF00 → #FF3333  (red when low)
/// SP bar       : #00AAFF  (sky blue)
/// XP bar       : #FFB800  (gold)
/// Text primary : #FFFFFF  (white)
/// Text label   : #88DDFF  (light cyan)
/// Highlight    : #1A4A7A  (hover blue)
/// </summary>
public static class SAOStyler
{
    // ── Palette ─────────────────────────────────────────────────────────────
    public static readonly Color BackgroundColor  = new Color(0.008f, 0.039f, 0.094f, 0.88f);
    public static readonly Color BorderColor      = new Color(0.000f, 0.831f, 1.000f, 1.000f);
    public static readonly Color TextPrimary      = new Color(1.000f, 1.000f, 1.000f, 1.000f);
    public static readonly Color TextLabel        = new Color(0.533f, 0.867f, 1.000f, 1.000f);
    public static readonly Color HPColorHigh      = new Color(0.000f, 1.000f, 0.533f, 1.000f);
    public static readonly Color HPColorMid       = new Color(1.000f, 1.000f, 0.000f, 1.000f);
    public static readonly Color HPColorLow       = new Color(1.000f, 0.200f, 0.200f, 1.000f);
    public static readonly Color SPColor          = new Color(0.000f, 0.667f, 1.000f, 1.000f);
    public static readonly Color XPColor          = new Color(1.000f, 0.722f, 0.000f, 1.000f);
    public static readonly Color ButtonNormal     = new Color(0.020f, 0.094f, 0.200f, 0.90f);
    public static readonly Color ButtonHighlight  = new Color(0.102f, 0.290f, 0.478f, 0.95f);
    public static readonly Color ButtonPressed    = new Color(0.000f, 0.831f, 1.000f, 0.40f);

    // ── Texture cache ────────────────────────────────────────────────────────
    private static Texture2D _whiteTex;
    public static Texture2D WhiteTex
    {
        get
        {
            if (_whiteTex == null)
            {
                _whiteTex = new Texture2D(1, 1);
                _whiteTex.SetPixel(0, 0, Color.white);
                _whiteTex.Apply();
            }
            return _whiteTex;
        }
    }

    // ── Canvas helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Create a Screen-Space Overlay canvas suitable for the SAO HUD.
    /// </summary>
    public static Canvas CreateOverlayCanvas(string name, int sortOrder = 100)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    // ── Panel helpers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a panel GameObject with the SAO dark-blue background.
    /// </summary>
    public static RectTransform CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax,
        float alpha = 0.88f)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, alpha);
        img.sprite = null;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        return rt;
    }

    /// <summary>
    /// Add a 1-pixel-wide border image on the edges of a panel.
    /// </summary>
    public static void AddBorder(RectTransform panel, float thickness = 1f)
    {
        // Top
        CreateBorderLine(panel, "BorderTop",
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, -thickness), new Vector2(0f, 0f));
        // Bottom
        CreateBorderLine(panel, "BorderBottom",
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 0f), new Vector2(0f, thickness));
        // Left
        CreateBorderLine(panel, "BorderLeft",
            new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0f), new Vector2(thickness, 0f));
        // Right
        CreateBorderLine(panel, "BorderRight",
            new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(-thickness, 0f), new Vector2(0f, 0f));
    }

    private static void CreateBorderLine(RectTransform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = BorderColor;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    // ── Text helpers ──────────────────────────────────────────────────────────

    /// <summary>Create a TextMeshProUGUI element with SAO styling.</summary>
    public static TextMeshProUGUI CreateText(Transform parent, string name, string text,
        float fontSize, Color color,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax,
        TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.fontStyle = FontStyles.Normal;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        return tmp;
    }

    // ── Bar helpers ───────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a labelled stat bar (background + fill image).
    /// Returns the fill Image so the caller can update fillAmount.
    /// </summary>
    public static Image CreateStatBar(Transform parent, string name, Color barColor,
        string labelText, float labelFontSize,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        // Container
        var container = new GameObject(name + "_Container");
        container.transform.SetParent(parent, false);
        var containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = anchorMin;
        containerRT.anchorMax = anchorMax;
        containerRT.offsetMin = offsetMin;
        containerRT.offsetMax = offsetMax;

        // Label
        float labelW = 50f;
        var lbl = CreateText(container.transform, name + "_Label", labelText,
            labelFontSize, TextLabel,
            new Vector2(0, 0), new Vector2(0, 1),
            new Vector2(0, 0), new Vector2(labelW, 0),
            TextAlignmentOptions.MidlineLeft);
        lbl.fontStyle = FontStyles.Bold;

        // Bar background
        var bgGO = new GameObject(name + "_BG");
        bgGO.transform.SetParent(container.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.05f, 0.15f, 0.9f);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0);
        bgRT.anchorMax = new Vector2(1, 1);
        bgRT.offsetMin = new Vector2(labelW + 4f, 2f);
        bgRT.offsetMax = new Vector2(-2f, -2f);

        // Bar fill
        var fillGO = new GameObject(name + "_Fill");
        fillGO.transform.SetParent(bgGO.transform, false);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = barColor;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;
        var fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        return fillImg;
    }

    // ── Button helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a styled SAO menu button with hover highlight.
    /// The Button's onClick listener must be added by the caller.
    /// </summary>
    public static Button CreateMenuButton(Transform parent, string label, float fontSize,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject("Btn_" + label);
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = ButtonNormal;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = ButtonNormal;
        colors.highlightedColor = ButtonHighlight;
        colors.pressedColor = ButtonPressed;
        colors.selectedColor = ButtonHighlight;
        btn.colors = colors;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        // Border accent on left edge (SAO style)
        var accent = new GameObject("Accent");
        accent.transform.SetParent(go.transform, false);
        var accentImg = accent.AddComponent<Image>();
        accentImg.color = BorderColor;
        var accentRT = accent.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0, 0);
        accentRT.anchorMax = new Vector2(0, 1);
        accentRT.offsetMin = new Vector2(0, 0);
        accentRT.offsetMax = new Vector2(3, 0);

        // Label
        var txt = CreateText(go.transform, "Label", label, fontSize, TextPrimary,
            new Vector2(0, 0), new Vector2(1, 1),
            new Vector2(12, 0), new Vector2(-8, 0),
            TextAlignmentOptions.MidlineLeft);
        txt.fontStyle = FontStyles.Bold;

        // Bottom separator
        CreateBorderLine(go.GetComponent<RectTransform>(), "Sep",
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(0, 0), new Vector2(0, 1));

        return btn;
    }

    // ── Colour helpers ────────────────────────────────────────────────────────

    public static Color GetHPColor(float pct) =>
        pct > 0.5f ? HPColorHigh : pct > 0.25f ? HPColorMid : HPColorLow;
}
