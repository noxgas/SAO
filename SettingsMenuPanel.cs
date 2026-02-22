using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Settings menu panel - SAO inspired.
/// Handles game settings like volume, graphics, keybinds.
/// </summary>
public class SettingsMenuPanel : UIPanel
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeLabel;
    [SerializeField] private TextMeshProUGUI musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;

    [Header("Graphics Settings")]
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessLabel;

    [Header("Gameplay Settings")]
    [SerializeField] private Toggle screenShakeToggle;
    [SerializeField] private Toggle particleEffectsToggle;
    [SerializeField] private Toggle bloodEffectsToggle;

    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button defaultButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private AudioManager audioManager;
    private GameSettings gameSettings;

    protected override void Awake()
    {
        base.Awake();
        audioManager = FindObjectOfType<AudioManager>();
        gameSettings = GameSettings.Instance;
    }

    private void Start()
    {
        SetupUI();
        AttachListeners();
    }

    private void SetupUI()
    {
        titleText.text = "⚙️ SETTINGS ⚙️";
        titleText.color = UIManager.Instance.GetAccentColor(AccentType.Blue);

        // Load current settings
        if (gameSettings != null)
        {
            masterVolumeSlider.value = gameSettings.masterVolume;
            musicVolumeSlider.value = gameSettings.musicVolume;
            sfxVolumeSlider.value = gameSettings.sfxVolume;
            brightnessSlider.value = gameSettings.brightness;
            
            vSyncToggle.isOn = gameSettings.vSyncEnabled;
            screenShakeToggle.isOn = gameSettings.screenShakeEnabled;
            particleEffectsToggle.isOn = gameSettings.particleEffectsEnabled;
            bloodEffectsToggle.isOn = gameSettings.bloodEffectsEnabled;

            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }
    }

    private void AttachListeners()
    {
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        vSyncToggle.onValueChanged.AddListener(OnVSyncToggled);
        screenShakeToggle.onValueChanged.AddListener(OnScreenShakeToggled);
        particleEffectsToggle.onValueChanged.AddListener(OnParticleEffectsToggled);
        bloodEffectsToggle.onValueChanged.AddListener(OnBloodEffectsToggled);

        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

        backButton.onClick.AddListener(OnBackClicked);
        applyButton.onClick.AddListener(OnApplyClicked);
        defaultButton.onClick.AddListener(OnDefaultsClicked);
    }

    private void OnMasterVolumeChanged(float value)
    {
        gameSettings.masterVolume = value;
        masterVolumeLabel.text = $"Master Volume: {Mathf.RoundToInt(value * 100)}%";
        if (audioManager != null)
            audioManager.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        gameSettings.musicVolume = value;
        musicVolumeLabel.text = $"Music Volume: {Mathf.RoundToInt(value * 100)}%";
        if (audioManager != null)
            audioManager.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        gameSettings.sfxVolume = value;
        sfxVolumeLabel.text = $"SFX Volume: {Mathf.RoundToInt(value * 100)}%";
        if (audioManager != null)
            audioManager.SetSFXVolume(value);
    }

    private void OnBrightnessChanged(float value)
    {
        gameSettings.brightness = value;
        brightnessLabel.text = $"Brightness: {Mathf.RoundToInt(value * 100)}%";
    }

    private void OnVSyncToggled(bool enabled)
    {
        gameSettings.vSyncEnabled = enabled;
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    private void OnScreenShakeToggled(bool enabled)
    {
        gameSettings.screenShakeEnabled = enabled;
    }

    private void OnParticleEffectsToggled(bool enabled)
    {
        gameSettings.particleEffectsEnabled = enabled;
    }

    private void OnBloodEffectsToggled(bool enabled)
    {
        gameSettings.bloodEffectsEnabled = enabled;
    }

    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    private void OnApplyClicked()
    {
        gameSettings.SaveSettings();
        Debug.Log("✓ Settings saved!");
    }

    private void OnDefaultsClicked()
    {
        gameSettings.ResetToDefaults();
        SetupUI();
        Debug.Log("✓ Settings reset to defaults!");
    }

    private void OnBackClicked()
    {
        Hide();
    }

    protected override void OnShow()
    {
        SetupUI();
    }
}