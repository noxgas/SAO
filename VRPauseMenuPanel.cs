using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Pause menu panel for VR - shown when player pauses the game.
/// </summary>
public class VRPauseMenuPanel : VRMenuPanel
{
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI pausedMessageText;

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 600f;
        panelHeight = 500f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    public override void Show()
    {
        base.Show();
        titleText.text = "⏸️ PAUSED ⏸️";
        titleText.color = VRMenuSystem.Instance.GetAccentColor(AccentType.Red);
        pausedMessageText.text = "Game is Paused";
        Time.timeScale = 0f;
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f;
    }

    private void OnResumeClicked()
    {
        Debug.Log("Resuming game...");
        VRMenuSystem.Instance.HidePauseMenu();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Opening settings...");
        VRMenuSystem.Instance.ShowSettings();
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}