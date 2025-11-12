using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneEventSystemCleaner : MonoBehaviour
{
    private void Start()
    {
        // Clean up duplicate EventSystems when this scene loads
        CleanupEventSystems();
        
        // Subscribe to scene loaded events for future cleanup
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Small delay to ensure all objects are loaded
        Invoke(nameof(CleanupEventSystems), 0.1f);
    }
    
    private void CleanupEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        
        if (eventSystems.Length > 1)
        {
            Debug.Log($"Found {eventSystems.Length} EventSystems, keeping the first one");
            
            // Keep the first one, destroy the rest
            for (int i = 1; i < eventSystems.Length; i++)
            {
                if (eventSystems[i] != null)
                {
                    Debug.Log($"Destroying duplicate EventSystem: {eventSystems[i].name}");
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
    }
}