using UnityEngine;

public class CloudSaveSystem : ISaveSystem
{
    private readonly string saveKey = "GameSaveData";
    
    public void SaveGame(object saveData)
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();
            Debug.Log("CloudSaveSystem: Game saved to PlayerPrefs");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CloudSaveSystem: Failed to save game - {ex.Message}");
        }
    }
    
    public T LoadGame<T>() where T : class
    {
        try
        {
            if (!PlayerPrefs.HasKey(saveKey))
            {
                Debug.LogWarning("CloudSaveSystem: No save data found in PlayerPrefs");
                return null;
            }
            
            string json = PlayerPrefs.GetString(saveKey);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("CloudSaveSystem: Save data is empty");
                return null;
            }
            
            T saveData = JsonUtility.FromJson<T>(json);
            Debug.Log("CloudSaveSystem: Game loaded successfully from PlayerPrefs");
            return saveData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CloudSaveSystem: Failed to load game - {ex.Message}");
            return null;
        }
    }
    
    public bool HasSaveFile()
    {
        return PlayerPrefs.HasKey(saveKey) && !string.IsNullOrEmpty(PlayerPrefs.GetString(saveKey));
    }
    
    public void DeleteSave()
    {
        try
        {
            if (PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);
                PlayerPrefs.Save();
                Debug.Log("CloudSaveSystem: Save data deleted from PlayerPrefs");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CloudSaveSystem: Failed to delete save data - {ex.Message}");
        }
    }
}