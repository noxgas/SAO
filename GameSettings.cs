using UnityEngine;

/// <summary>
/// Persistent game settings that survive scene loads.
/// </summary>
public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;

    [Header("Audio")]
    public float masterVolume = 1f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 0.8f;

    [Header("Graphics")]
    public float brightness = 1f;
    public bool vSyncEnabled = true;

    [Header("Gameplay")]
    public bool screenShakeEnabled = true;
    public bool particleEffectsEnabled = true;
    public bool bloodEffectsEnabled = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameSettings");
                instance = go.AddComponent<GameSettings>();
            }
            return instance;
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("brightness", brightness);
        PlayerPrefs.SetInt("vSync", vSyncEnabled ? 1 : 0);
        PlayerPrefs.SetInt("screenShake", screenShakeEnabled ? 1 : 0);
        PlayerPrefs.SetInt("particles", particleEffectsEnabled ? 1 : 0);
        PlayerPrefs.SetInt("blood", bloodEffectsEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 0.8f);
        brightness = PlayerPrefs.GetFloat("brightness", 1f);
        vSyncEnabled = PlayerPrefs.GetInt("vSync", 1) == 1;
        screenShakeEnabled = PlayerPrefs.GetInt("screenShake", 1) == 1;
        particleEffectsEnabled = PlayerPrefs.GetInt("particles", 1) == 1;
        bloodEffectsEnabled = PlayerPrefs.GetInt("blood", 1) == 1;
    }

    public void ResetToDefaults()
    {
        masterVolume = 1f;
        musicVolume = 0.7f;
        sfxVolume = 0.8f;
        brightness = 1f;
        vSyncEnabled = true;
        screenShakeEnabled = true;
        particleEffectsEnabled = true;
        bloodEffectsEnabled = true;
        SaveSettings();
    }
}