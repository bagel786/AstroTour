using UnityEngine;

[System.Serializable]
public class GameSettings
{
    [Header("Audio Settings")]
    public float sfxVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float masterVolume = 1.0f;
    
    [Header("Graphics Settings")]
    public int qualityLevel = 2; // Default to medium quality
    public bool fullscreen = true;
    public int resolutionIndex = 0;
    
    [Header("Gameplay Settings")]
    public bool showTutorials = true;
    public bool autoSave = true;
    public float mouseSensitivity = 1.0f;
    
    public GameSettings()
    {
        // Default constructor with default values
    }
    
    /// <summary>
    /// Create settings with default values
    /// </summary>
    public static GameSettings CreateDefault()
    {
        return new GameSettings
        {
            sfxVolume = 1.0f,
            musicVolume = 0.7f,
            masterVolume = 1.0f,
            qualityLevel = QualitySettings.GetQualityLevel(),
            fullscreen = Screen.fullScreen,
            resolutionIndex = 0,
            showTutorials = true,
            autoSave = true,
            mouseSensitivity = 1.0f
        };
    }
}