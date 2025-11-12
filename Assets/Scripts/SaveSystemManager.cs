using System.IO;
using UnityEngine;

public class SaveSystemManager : MonoBehaviour
{
    public static SaveSystemManager Instance { get; private set; }
    
    [Header("Save Configuration")]
    public bool forceCloudSave = false; // For testing cloud behavior locally
    
    private ISaveSystem saveSystem;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSaveSystem()
    {
        // Detect platform and choose appropriate save system
        if (IsCloudPlatform() || forceCloudSave)
        {
            saveSystem = new CloudSaveSystem();
            Debug.Log("SaveSystemManager: Using Cloud Save System (PlayerPrefs)");
        }
        else
        {
            saveSystem = new LocalFileSaveSystem();
            Debug.Log("SaveSystemManager: Using Local File Save System (JSON)");
        }
    }
    
    private bool IsCloudPlatform()
    {
        // Check if running on Unity Play or other cloud platforms
        #if UNITY_WEBGL && !UNITY_EDITOR
            return true;
        #elif UNITY_CLOUD_BUILD
            return true;
        #else
            // You can add additional checks here for Unity Play specifically
            // For now, WebGL build indicates cloud deployment
            return Application.platform == RuntimePlatform.WebGLPlayer;
        #endif
    }
    
    public void SaveGame(object saveData)
    {
        saveSystem.SaveGame(saveData);
    }
    
    public T LoadGame<T>() where T : class
    {
        return saveSystem.LoadGame<T>();
    }
    
    public bool HasSaveFile()
    {
        return saveSystem.HasSaveFile();
    }
    
    public void DeleteSave()
    {
        saveSystem.DeleteSave();
    }
}