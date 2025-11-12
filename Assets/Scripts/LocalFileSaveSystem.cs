using UnityEngine;
using System.IO;

public class LocalFileSaveSystem : MonoBehaviour, ISaveSystem
{
    [Header("Save Configuration")]
    public string saveFileName = "gamedata.json";
    
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);
    
    // ISaveSystem interface implementation
    public void SaveGame(object saveData)
    {
        SaveData(saveData);
    }
    
    public T LoadGame<T>() where T : class
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string jsonData = File.ReadAllText(SavePath);
                T result = JsonUtility.FromJson<T>(jsonData);
                return result;
            }
            else
            {
                Debug.LogWarning("Save file not found, returning null");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            return null;
        }
    }
    
    public bool HasSaveFile()
    {
        return SaveExists();
    }
    
    void ISaveSystem.DeleteSave()
    {
        DeleteSave();
    }
    
    // Original methods for backward compatibility
    public void SaveData(object data)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log($"Data saved to: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }
    
    public T LoadData<T>() where T : class, new()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string jsonData = File.ReadAllText(SavePath);
                T result = JsonUtility.FromJson<T>(jsonData);
                return result ?? new T();
            }
            else
            {
                Debug.LogWarning("Save file not found, returning default data");
                return new T();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            return new T();
        }
    }
    
    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }
    
    public void DeleteSave()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete save: {e.Message}");
        }
    }
}