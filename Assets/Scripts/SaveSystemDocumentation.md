# Dual Save System Documentation

## Overview
The AstroTour project now implements a comprehensive dual save system that automatically detects the platform and uses the appropriate save method:

- **PlayerPrefs** for cloud/web platforms (Unity Play, WebGL)
- **JSON Files** for local platforms (Windows, Mac, Linux)

## Architecture

### Core Components

1. **SaveSystemManager** - Main controller that detects platform and chooses save system
2. **ISaveSystem** - Interface that both save systems implement
3. **CloudSaveSystem** - Uses PlayerPrefs for cloud platforms
4. **LocalFileSaveSystem** - Uses JSON files for local platforms
5. **SaveController** - Game-specific save controller with fallback support
6. **SettingsManager** - Handles game settings with dual save support

### Automatic Platform Detection

The system automatically detects the platform:
```csharp
private bool IsCloudPlatform()
{
    #if UNITY_WEBGL && !UNITY_EDITOR
        return true;
    #elif UNITY_CLOUD_BUILD
        return true;
    #else
        return Application.platform == RuntimePlatform.WebGLPlayer;
    #endif
}
```

## Game Data Saving

### SaveController Features
- **Dual System Support**: Uses SaveSystemManager with legacy fallback
- **Automatic Detection**: Switches between PlayerPrefs and JSON automatically
- **Backward Compatibility**: Can load from legacy save files
- **Comprehensive Data**: Saves player position, inventory, quests, terminals, etc.

### Usage Example
```csharp
// The SaveController automatically handles platform detection
SaveController saveController = FindObjectOfType<SaveController>();
saveController.SaveGame(); // Automatically uses correct save system
saveController.LoadGame(); // Automatically loads from correct system
```

## Settings System

### SettingsManager Features
- **Dual Storage**: Saves to both PlayerPrefs and JSON files
- **Audio Settings**: SFX, Music, and Master volume controls
- **Graphics Settings**: Quality level, fullscreen, resolution
- **Gameplay Settings**: Tutorials, auto-save, mouse sensitivity
- **Real-time Application**: Settings apply immediately when changed

### Settings Integration
The SoundEffectManager now integrates with SettingsManager:
```csharp
// Volume changes are automatically saved
SettingsManager.Instance.SetSFXVolume(0.8f);
SettingsManager.Instance.SetMusicVolume(0.6f);
```

## File Locations

### Local Platform (Windows/Mac/Linux)
- **Game Data**: `Application.persistentDataPath/gamedata.json`
- **Settings**: `Application.persistentDataPath/settings.json`
- **Legacy Save**: `Application.persistentDataPath/saveData.json`

### Cloud Platform (WebGL/Unity Play)
- **Game Data**: PlayerPrefs key "GameSaveData"
- **Settings**: Multiple PlayerPrefs keys (Settings_SFXVolume, etc.)

## Migration and Compatibility

### Legacy Support
- Old save files are automatically detected and loaded
- Settings migrate from old PlayerPrefs format
- No data loss during system upgrade

### Cross-Platform
- Players can move between platforms
- Save data format is consistent (JSON)
- Settings transfer between systems

## Configuration Options

### SaveController
```csharp
[Header("Save System Configuration")]
[SerializeField] private bool useModernSaveSystem = true;
```

### SaveSystemManager
```csharp
[Header("Save Configuration")]
public bool forceCloudSave = false; // For testing cloud behavior locally
```

### SettingsManager
```csharp
[Header("Settings Configuration")]
[SerializeField] private bool useModernSaveSystem = true;
[SerializeField] private string settingsFileName = "settings.json";
```

## Testing

### Force Cloud Mode
Set `forceCloudSave = true` in SaveSystemManager to test PlayerPrefs behavior on local platforms.

### Debug Information
All save systems provide detailed logging:
- Platform detection results
- Save/load success/failure messages
- File paths and PlayerPrefs keys used

## Benefits

1. **Platform Agnostic**: Works seamlessly across all Unity platforms
2. **Automatic**: No manual configuration required
3. **Reliable**: Dual storage prevents data loss
4. **Backward Compatible**: Existing saves continue to work
5. **Extensible**: Easy to add new save data types
6. **Performance**: Optimized for each platform's strengths

## Future Enhancements

- Cloud sync between platforms
- Compressed save data for large files
- Encrypted save data for security
- Multiple save slots
- Save data versioning for updates