using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Pause menu that appears when ESC is pressed.
/// </summary>
public class PauseMenuPanel : UIPanel
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        titleText.text = "⏸️ PAUSED ⏸️";
        titleText.color = UIManager.Instance.GetAccentColor(AccentType.Red);

        resumeButton.onClick.AddListener(OnResumeClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        characterButton.onClick.AddListener(OnCharacterClicked);
        inventoryButton.onClick.AddListener(OnInventoryClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnResumeClicked()
    {
        UIManager.Instance.HidePauseMenu();
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.ShowSettings();
    }

    private void OnCharacterClicked()
    {
        UIManager.Instance.ShowCharacter();
    }

    private void OnInventoryClicked()
    {
        UIManager.Instance.ShowInventory();
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        // Load main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void OnQuitClicked()
    {
        Time.timeScale = 1f;
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}