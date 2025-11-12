using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager instance;
    public static EventSystemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<EventSystemManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("EventSystemManager");
                    instance = go.AddComponent<EventSystemManager>();
                }
            }
            return instance;
        }
    }
    
    private EventSystem eventSystem;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEventSystem();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeEventSystem()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
    
    public static void DisableEventSystem()
    {
        if (Instance.eventSystem != null)
        {
            Instance.eventSystem.enabled = false;
        }
    }
    
    public static void EnableEventSystem()
    {
        if (Instance.eventSystem != null)
        {
            Instance.eventSystem.enabled = true;
        }
    }
    
    public static void CleanupDuplicateEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        
        if (eventSystems.Length > 1)
        {
            // Keep the first one, destroy the rest
            for (int i = 1; i < eventSystems.Length; i++)
            {
                if (eventSystems[i] != Instance.eventSystem)
                {
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
    }
}