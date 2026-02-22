using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main menu panel shown at game start.
/// </summary>
public class MainMenuPanel : UIPanel
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    private void Start()
    {
        titleText.text = "⚔️ SWORD ART ONLINE VR ⚔️";
        subtitleText.text = "100 FLOORS AWAIT";
        
        titleText.color = UIManager.Instance.GetAccentColor(AccentType.Blue);

        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnNewGameClicked()
    {
        Debug.Log("Starting new game...");
        // Load character creation scene or game scene
    }

    private void OnContinueClicked()
    {
        Debug.Log("Loading saved game...");
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.ShowSettings();
    }

    private void OnCreditsClicked()
    {
        Debug.Log("Showing credits...");
    }

    private void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    protected override void OnShow()
    {
        // Any main menu specific logic
    }
}