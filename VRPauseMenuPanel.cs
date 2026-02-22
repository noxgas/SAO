using UnityEngine;
using TMPro;

/// <summary>
/// VR Pause menu that appears with controller grip button.
/// </summary>
public class VRPauseMenuPanel : VRUIPanel
{
    [Header("Buttons")]
    [SerializeField] private VRButton resumeButton;
    [SerializeField] private VRButton settingsButton;
    [SerializeField] private VRButton characterButton;
    [SerializeField] private VRButton inventoryButton;
    [SerializeField] private VRButton mainMenuButton;
    [SerializeField] private VRButton quitButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        titleText.text = "⏸️ PAUSED ⏸️";
        titleText.color = VRUIManager.Instance.GetAccentColor(AccentType.Red);

        resumeButton.OnClicked += OnResumeClicked;
        settingsButton.OnClicked += OnSettingsClicked;
        characterButton.OnClicked += OnCharacterClicked;
        inventoryButton.OnClicked += OnInventoryClicked;
        mainMenuButton.OnClicked += OnMainMenuClicked;
        quitButton.OnClicked += OnQuitClicked;
    }

    private void OnResumeClicked()
    {
        VRUIManager.Instance.HidePauseMenu();
    }

    private void OnSettingsClicked()
    {
        VRUIManager.Instance.ShowSettings();
    }

    private void OnCharacterClicked()
    {
        VRUIManager.Instance.ShowCharacter();
    }

    private void OnInventoryClicked()
    {
        VRUIManager.Instance.ShowInventory();
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
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