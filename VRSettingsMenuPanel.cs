#if STEAMVR_PRESENT
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Valve.VR;

/// <summary>
/// VR Settings menu panel - SAO inspired.
/// Handles game settings using VR controller input.
/// </summary>
public class VRSettingsMenuPanel : VRUIPanel
{
    [Header("Audio Settings")]
    [SerializeField] private VRSlider masterVolumeSlider;
    [SerializeField] private VRSlider musicVolumeSlider;
    [SerializeField] private VRSlider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeLabel;
    [SerializeField] private TextMeshProUGUI musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;

    [Header("Graphics Settings")]
    [SerializeField] private VRToggle vSyncToggle;
    [SerializeField] private VRSlider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessLabel;

    [Header("Gameplay Settings")]
    [SerializeField] private VRToggle screenShakeToggle;
    [SerializeField] private VRToggle particleEffectsToggle;
    [SerializeField] private VRToggle bloodEffectsToggle;

    [Header("UI Elements")]
    [SerializeField] private VRButton backButton;
    [SerializeField] private VRButton applyButton;
    [SerializeField] private VRButton defaultButton;

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
        titleText.color = VRUIManager.Instance.GetAccentColor(AccentType.Blue);

        // Load current settings
        if (gameSettings != null)
        {
            masterVolumeSlider.SetValue(gameSettings.masterVolume);
            musicVolumeSlider.SetValue(gameSettings.musicVolume);
            sfxVolumeSlider.SetValue(gameSettings.sfxVolume);
            brightnessSlider.SetValue(gameSettings.brightness);
            
            vSyncToggle.SetValue(gameSettings.vSyncEnabled);
            screenShakeToggle.SetValue(gameSettings.screenShakeEnabled);
            particleEffectsToggle.SetValue(gameSettings.particleEffectsEnabled);
            bloodEffectsToggle.SetValue(gameSettings.bloodEffectsEnabled);
        }
    }

    private void AttachListeners()
    {
        masterVolumeSlider.OnValueChanged += OnMasterVolumeChanged;
        musicVolumeSlider.OnValueChanged += OnMusicVolumeChanged;
        sfxVolumeSlider.OnValueChanged += OnSFXVolumeChanged;
        brightnessSlider.OnValueChanged += OnBrightnessChanged;

        vSyncToggle.OnValueChanged += OnVSyncToggled;
        screenShakeToggle.OnValueChanged += OnScreenShakeToggled;
        particleEffectsToggle.OnValueChanged += OnParticleEffectsToggled;
        bloodEffectsToggle.OnValueChanged += OnBloodEffectsToggled;

        backButton.OnClicked += OnBackClicked;
        applyButton.OnClicked += OnApplyClicked;
        defaultButton.OnClicked += OnDefaultsClicked;
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
#endif // STEAMVR_PRESENT
