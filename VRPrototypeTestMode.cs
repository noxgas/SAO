using UnityEngine;

/// <summary>
/// Desktop test harness for the VR Swordsman prototype.
///
/// Add this component to the same GameObject as (or a parent of) the player rig
/// to enable full keyboard + mouse testing without a VR headset.
///
/// Controls
/// ──────────────────────────────────────────────────────
/// WASD / Arrow keys  – move (Unity's default Horizontal/Vertical axes)
/// Mouse X            – rotate body (yaw)
/// Left Mouse Button  – swing sword  (when holding sword)
/// E                  – grab / release nearest sword
/// R                  – respawn all enemies via EnemySpawner
/// Tab                – toggle on-screen debug overlay
/// Esc                – unlock cursor (Editor use)
/// ──────────────────────────────────────────────────────
/// </summary>
public class VRPrototypeTestMode : MonoBehaviour
{
    [Header("References (auto-found if left empty)")]
    [SerializeField] private VRMovementSystem movementSystem;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerCombat playerCombat;

    [Header("Sword Interaction")]
    [SerializeField] private KeyCode grabKey = KeyCode.E;
    [SerializeField] private float grabRadius = 3f;

    [Header("Enemy Respawn")]
    [SerializeField] private KeyCode respawnKey = KeyCode.R;

    [Header("Debug Overlay")]
    [SerializeField] private KeyCode toggleOverlayKey = KeyCode.Tab;
    [SerializeField] private bool showOverlay = true;

    // Runtime
    private VRSwordController heldSword;
    private bool overlayVisible = true;
    private int cachedEnemyCount;
    private float enemyCountRefreshTimer;
    private const float EnemyCountRefreshInterval = 0.5f; // refresh twice per second

    // Cached GUI styles (allocated once in Start, reused every frame)
    private GUIStyle labelStyle;
    private Texture2D overlayBgTexture;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Start()
    {
        if (movementSystem == null)
            movementSystem = FindObjectOfType<VRMovementSystem>();
        if (enemySpawner == null)
            enemySpawner = FindObjectOfType<EnemySpawner>();
        if (playerCombat == null)
            playerCombat = FindObjectOfType<PlayerCombat>();

        overlayVisible = showOverlay;

        // Pre-build GUI assets once
        overlayBgTexture = new Texture2D(1, 1);
        overlayBgTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        overlayBgTexture.Apply();

        // Lock cursor so mouse-look works in Play Mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[VRPrototypeTestMode] Desktop test mode active. Tab=overlay, E=grab sword, R=respawn enemies.");
    }

    private void OnDestroy()
    {
        if (overlayBgTexture != null)
            Destroy(overlayBgTexture);
    }

    private void Update()
    {
        HandleGrab();
        HandleRespawn();
        HandleOverlayToggle();
        HandleCursorUnlock();

        // Refresh enemy count on a timer instead of every frame
        enemyCountRefreshTimer += Time.deltaTime;
        if (enemyCountRefreshTimer >= EnemyCountRefreshInterval)
        {
            enemyCountRefreshTimer = 0f;
            cachedEnemyCount = FindObjectsOfType<AIEnemy>().Length;
        }
    }

    // ── Input handlers ───────────────────────────────────────────────────────

    private void HandleGrab()
    {
        if (!Input.GetKeyDown(grabKey)) return;

        if (heldSword != null)
        {
            // Release current sword
            heldSword.OnRelease();
            heldSword = null;
            Debug.Log("[TestMode] Sword released.");
            return;
        }

        // Find nearest sword in grab radius
        VRSwordController nearest = FindNearestSword();
        if (nearest != null)
        {
            heldSword = nearest;
            heldSword.OnGrab(transform);
            Debug.Log($"[TestMode] Grabbed sword: {heldSword.name} ({heldSword.Type})");
        }
        else
        {
            Debug.Log("[TestMode] No sword nearby. Move closer and press E.");
        }
    }

    private VRSwordController FindNearestSword()
    {
        VRSwordController[] swords = FindObjectsOfType<VRSwordController>();
        VRSwordController nearest = null;
        float bestDist = grabRadius;

        foreach (var sword in swords)
        {
            if (sword.IsHeld) continue;
            float dist = Vector3.Distance(transform.position, sword.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                nearest = sword;
            }
        }
        return nearest;
    }

    private void HandleRespawn()
    {
        if (Input.GetKeyDown(respawnKey) && enemySpawner != null)
        {
            enemySpawner.DespawnAll();
            cachedEnemyCount = 0;
            Debug.Log("[TestMode] All enemies despawned. New enemies will spawn shortly.");
        }
    }

    private void HandleOverlayToggle()
    {
        if (Input.GetKeyDown(toggleOverlayKey))
            overlayVisible = !overlayVisible;
    }

    private void HandleCursorUnlock()
    {
        // Escape unlocks the cursor so the developer can interact with the Editor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // ── On-screen debug HUD ──────────────────────────────────────────────────

    private void OnGUI()
    {
        if (!overlayVisible) return;

        // Lazily initialise label style the first time OnGUI runs
        // (GUI.skin is only valid inside OnGUI)
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 14;
            labelStyle.normal.textColor = Color.white;
        }

        float x = 10f, y = 10f, w = 320f, lineH = 20f;
        int lines = 10;

        // Background
        GUIStyle bgStyle = new GUIStyle();
        bgStyle.normal.background = overlayBgTexture;
        GUI.Box(new Rect(x, y, w, lineH * lines + 8f), GUIContent.none, bgStyle);
        y += 4f;

        GUI.Label(new Rect(x + 4f, y, w, lineH), "── VR Prototype Test Mode ──", labelStyle); y += lineH;

        // ── Health / Stamina ──
        if (playerCombat != null)
        {
            DrawBar(x + 4f, y, w - 8f, lineH - 4f,
                playerCombat.HealthPercent, "HP", Color.green, Color.red);
            y += lineH;
            DrawBar(x + 4f, y, w - 8f, lineH - 4f,
                playerCombat.StaminaPercent, "STA", new Color(0.2f, 0.6f, 1f), Color.gray);
            y += lineH;
        }

        // ── Sword ──
        string swordInfo = heldSword != null
            ? $"Sword: {heldSword.name} [{heldSword.Type}]  spd:{heldSword.SwingSpeed:F1} m/s"
            : "Sword: none  (E = grab)";
        GUI.Label(new Rect(x + 4f, y, w, lineH), swordInfo, labelStyle); y += lineH;

        // ── Enemy count (cached) ──
        GUI.Label(new Rect(x + 4f, y, w, lineH), $"Enemies: {cachedEnemyCount}", labelStyle); y += lineH;

        // ── Controls reminder ──
        y += 4f;
        GUI.Label(new Rect(x + 4f, y, w, lineH), "WASD/Arrows=move  Mouse=look  LMB=swing", labelStyle); y += lineH;
        GUI.Label(new Rect(x + 4f, y, w, lineH), "E=grab  R=respawn  Tab=overlay  Esc=cursor", labelStyle);
    }

    private void DrawBar(float x, float y, float w, float h,
        float fraction, string label, Color fillColor, Color emptyColor)
    {
        // Background
        GUI.color = emptyColor;
        GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
        // Fill
        GUI.color = fillColor;
        GUI.DrawTexture(new Rect(x, y, w * Mathf.Clamp01(fraction), h), Texture2D.whiteTexture);
        GUI.color = Color.white;
        // Label
        if (labelStyle != null)
            GUI.Label(new Rect(x + 2f, y - 1f, w, h + 2f),
                $"{label}: {Mathf.RoundToInt(fraction * 100f)}%", labelStyle);
    }
}
