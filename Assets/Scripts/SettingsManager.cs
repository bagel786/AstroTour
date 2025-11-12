using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    [Header("Settings Configuration")]
    [SerializeField] private bool useModernSaveSystem = true;
    [SerializeField] private string settingsFileName = "settings.json";
    
    private GameSettings currentSettings;
    private string legacySettingsPath;
    
    public GameSettings Settings => currentSettings;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            legacySettingsPath = System.IO.Path.Combine(Application.persistentDataPath, settingsFileName);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        ApplySettings();
    }
    
    public void LoadSettings()
    {
        GameSettings loadedSettings = null;
        
        if (useModernSaveSystem && SaveSystemManager.Instance != null)
        {
            // Try to load using the modern save system first
            // Note: We'll need to modify SaveSystemManager to handle multiple save types
            // For now, we'll use a different approach
        }
        
        // Try legacy system
        if (loadedSettings == null && System.IO.File.Exists(legacySettingsPath))
        {
            try
            {
                string json = System.IO.File.ReadAllText(legacySettingsPath);
                loadedSettings = JsonUtility.FromJson<GameSettings>(json);
                Debug.Log($"Settings loaded from legacy system: {legacySettingsPath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load settings from legacy system: {ex.Message}");
            }
        }
        
        // Try PlayerPrefs as fallback
        if (loadedSettings == null)
        {
            loadedSettings = LoadFromPlayerPrefs();
        }
        
        // Use default settings if nothing was loaded
        currentSettings = loadedSettings ?? GameSettings.CreateDefault();
        
        Debug.Log($"Settings loaded - SFX: {currentSettings.sfxVolume}, Music: {currentSettings.musicVolume}");
    }
    
    public void SaveSettings()
    {
        if (currentSettings == null)
        {
            Debug.LogWarning("No settings to save!");
            return;
        }
        
        // Save to both systems for maximum compatibility
        SaveToPlayerPrefs();
        SaveToFile();
        
        Debug.Log("Settings saved to both PlayerPrefs and file system");
    }
    
    private void SaveToPlayerPrefs()
    {
        try
        {
            PlayerPrefs.SetFloat("Settings_SFXVolume", currentSettings.sfxVolume);
            PlayerPrefs.SetFloat("Settings_MusicVolume", currentSettings.musicVolume);
            PlayerPrefs.SetFloat("Settings_MasterVolume", currentSettings.masterVolume);
            PlayerPrefs.SetInt("Settings_QualityLevel", currentSettings.qualityLevel);
            PlayerPrefs.SetInt("Settings_Fullscreen", currentSettings.fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("Settings_ResolutionIndex", currentSettings.resolutionIndex);
            PlayerPrefs.SetInt("Settings_ShowTutorials", currentSettings.showTutorials ? 1 : 0);
            PlayerPrefs.SetInt("Settings_AutoSave", currentSettings.autoSave ? 1 : 0);
            PlayerPrefs.SetFloat("Settings_MouseSensitivity", currentSettings.mouseSensitivity);
            PlayerPrefs.Save();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save settings to PlayerPrefs: {ex.Message}");
        }
    }
    
    private void SaveToFile()
    {
        try
        {
            string json = JsonUtility.ToJson(currentSettings, true);
            System.IO.File.WriteAllText(legacySettingsPath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save settings to file: {ex.Message}");
        }
    }
    
    private GameSettings LoadFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("Settings_SFXVolume"))
        {
            return null; // No settings in PlayerPrefs
        }
        
        try
        {
            GameSettings settings = new GameSettings
            {
                sfxVolume = PlayerPrefs.GetFloat("Settings_SFXVolume", 1.0f),
                musicVolume = PlayerPrefs.GetFloat("Settings_MusicVolume", 0.7f),
                masterVolume = PlayerPrefs.GetFloat("Settings_MasterVolume", 1.0f),
                qualityLevel = PlayerPrefs.GetInt("Settings_QualityLevel", 2),
                fullscreen = PlayerPrefs.GetInt("Settings_Fullscreen", 1) == 1,
                resolutionIndex = PlayerPrefs.GetInt("Settings_ResolutionIndex", 0),
                showTutorials = PlayerPrefs.GetInt("Settings_ShowTutorials", 1) == 1,
                autoSave = PlayerPrefs.GetInt("Settings_AutoSave", 1) == 1,
                mouseSensitivity = PlayerPrefs.GetFloat("Settings_MouseSensitivity", 1.0f)
            };
            
            Debug.Log("Settings loaded from PlayerPrefs");
            return settings;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load settings from PlayerPrefs: {ex.Message}");
            return null;
        }
    }
    
    private void ApplySettings()
    {
        if (currentSettings == null) return;
        
        // Apply audio settings
        if (SoundEffectManager.Instance != null)
        {
            SoundEffectManager.SetVolume(currentSettings.sfxVolume * currentSettings.masterVolume);
            SoundEffectManager.SetMusicVolume(currentSettings.musicVolume * currentSettings.masterVolume);
        }
        
        // Apply graphics settings
        QualitySettings.SetQualityLevel(currentSettings.qualityLevel);
        
        // Apply other settings as needed
    }
    
    // Public methods to update settings
    public void SetSFXVolume(float volume)
    {
        currentSettings.sfxVolume = Mathf.Clamp01(volume);
        SoundEffectManager.SetVolume(currentSettings.sfxVolume * currentSettings.masterVolume);
        SaveSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        currentSettings.musicVolume = Mathf.Clamp01(volume);
        SoundEffectManager.SetMusicVolume(currentSettings.musicVolume * currentSettings.masterVolume);
        SaveSettings();
    }
    
    public void SetMasterVolume(float volume)
    {
        currentSettings.masterVolume = Mathf.Clamp01(volume);
        ApplySettings(); // Reapply all audio settings with new master volume
        SaveSettings();
    }
    
    public void SetQualityLevel(int level)
    {
        currentSettings.qualityLevel = level;
        QualitySettings.SetQualityLevel(level);
        SaveSettings();
    }
    
    public void SetFullscreen(bool fullscreen)
    {
        currentSettings.fullscreen = fullscreen;
        Screen.fullScreen = fullscreen;
        SaveSettings();
    }
    
    public void DeleteAllSettings()
    {
        // Delete from PlayerPrefs
        string[] keys = {
            "Settings_SFXVolume", "Settings_MusicVolume", "Settings_MasterVolume",
            "Settings_QualityLevel", "Settings_Fullscreen", "Settings_ResolutionIndex",
            "Settings_ShowTutorials", "Settings_AutoSave", "Settings_MouseSensitivity"
        };
        
        foreach (string key in keys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        PlayerPrefs.Save();
        
        // Delete from file system
        if (System.IO.File.Exists(legacySettingsPath))
        {
            System.IO.File.Delete(legacySettingsPath);
        }
        
        // Reset to defaults
        currentSettings = GameSettings.CreateDefault();
        ApplySettings();
        
        Debug.Log("All settings deleted and reset to defaults");
    }
}