using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Settings panel - graphics, audio, VR comfort options.
/// </summary>
public class VRSettingsPanel : VRMenuPanel
{
    [Header("Graphics")]
    [SerializeField] private Slider graphicsQualitySlider;
    [SerializeField] private Toggle vSyncToggle;

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("VR Comfort")]
    [SerializeField] private Slider uiTransparencySlider;
    [SerializeField] private Slider uiScaleSlider;
    [SerializeField] private Toggle motionSicknessToggle;

    protected override void Awake()
    {
        base.Awake();
        panelWidth = 600f;
        panelHeight = 800f;
    }

    public override void Initialize(VRMenuSystem system, string type, Vector3 offset)
    {
        base.Initialize(system, type, offset);
        AttachListeners();
    }

    private void AttachListeners()
    {
        graphicsQualitySlider.onValueChanged.AddListener(OnGraphicsQualityChanged);
        vSyncToggle.onValueChanged.AddListener(OnVSyncToggled);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        uiTransparencySlider.onValueChanged.AddListener(OnUITransparencyChanged);
        uiScaleSlider.onValueChanged.AddListener(OnUIScaleChanged);
    }

    private void OnGraphicsQualityChanged(float value)
    {
        QualitySettings.SetQualityLevel((int)value);
    }

    private void OnVSyncToggled(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    private void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        // TODO: Set music volume
    }

    private void OnSFXVolumeChanged(float value)
    {
        // TODO: Set SFX volume
    }

    private void OnUITransparencyChanged(float value)
    {
        // Update all panel transparency
    }

    private void OnUIScaleChanged(float value)
    {
        // Update all panel scale
    }
}