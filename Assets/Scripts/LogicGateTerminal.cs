using UnityEngine;

public class LogicGateTerminal : MonoBehaviour, IInteractable
{
    [Header("Terminal Configuration")]
    public string terminalName = "Quant Logic Gate Factory";
    
    [Header("Game References")]
    public LogicGateController gameController;
    
    [Header("Save System")]
    [Tooltip("Unique identifier for saving. Leave empty to auto-generate based on position.")]
    public string terminalID; // Unique identifier for saving
    
    // Public property to access terminal ID for save system
    public string TerminalID 
    { 
        get 
        { 
            EnsureTerminalID(); 
            return terminalID; 
        } 
    }
    
    public bool hasCompleted { get; private set; } = false;
    public bool hasInteracted { get; private set; } = false;
    
    void Start()
    {
        // Find game controller if not assigned
        if (gameController == null)
        {
            gameController = FindAnyObjectByType<LogicGateController>();
        }
        
        if (gameController == null)
        {
            Debug.LogError("LogicGateTerminal: No LogicGateController found in scene!");
        }
        
        // Generate unique ID if not set
        EnsureTerminalID();
        
        // Load completion state (this will be overwritten by SaveController if save exists)
        LoadSaveData();
        
        // Ensure game controller knows about completion state
        if (gameController != null)
        {
            gameController.SetCompletionState(hasCompleted);
        }
    }
    
    public bool canInteract()
    {
        return true; // Always accessible
    }
    
    public void Interact()
    {
        if (gameController == null)
        {
            Debug.LogError("LogicGateTerminal: No LogicGateController found!");
            return;
        }
        
        // Mark as interacted
        hasInteracted = true;
        
        // Check if already completed
        if (hasCompleted)
        {
            ActivateCompletedGamePanel();
            return;
        }
        
        // Open the game panel for new challenge
        ActivateGamePanel();
    }
    
    private void ActivateGamePanel()
    {
        // Activate the logic gate game panel and pause the game
        if (gameController != null && gameController.gamePanel != null)
        {
            gameController.gamePanel.SetActive(true);
            PauseController.SetPause(true);
        }
        else
        {
            Debug.LogError("LogicGateTerminal: Game panel not found!");
        }
    }
    
    private void ActivateCompletedGamePanel()
    {
        // Activate the logic gate game panel and show completed state
        if (gameController != null && gameController.gamePanel != null)
        {
            gameController.gamePanel.SetActive(true);
            PauseController.SetPause(true);
            
            // Show the completed state with all correct answers
            gameController.ShowCompletedState();
        }
        else
        {
            Debug.LogError("LogicGateTerminal: Game panel not found!");
        }
    }
    
    /// <summary>
    /// Called by LogicGateController when challenge is completed
    /// </summary>
    public void OnChallengeCompleted()
    {
        hasCompleted = true;
        hasInteracted = true;
        
        // Only auto-save on WebGL/cloud platforms, not on desktop
        #if UNITY_WEBGL && !UNITY_EDITOR
        SaveController saveController = FindAnyObjectByType<SaveController>();
        if (saveController != null)
        {
            saveController.SaveGame();
            Debug.Log("LogicGateTerminal: Auto-saved completion state (WebGL platform)");
        }
        #endif
        
        // Notify quest system
        NotifyQuestSystem();
    }
    
    /// <summary>
    /// Notifies the quest system that this terminal has been completed
    /// </summary>
    private void NotifyQuestSystem()
    {
        // Ensure terminal ID is generated before notifying quest system
        EnsureTerminalID();
        
        if (QuestController.Instance != null)
        {
            // Notify quest system using the terminal ID for matching with quest objectives
            QuestController.Instance.CheckTerminalUnlock(terminalID);
        }
    }
    
    /// <summary>
    /// Ensures terminal has a unique ID for save/load and quest system
    /// </summary>
    private void EnsureTerminalID()
    {
        if (string.IsNullOrEmpty(terminalID))
        {
            // Generate unique ID based on position
            terminalID = $"logic_gate_terminal_{transform.position.x:F1}_{transform.position.y:F1}";
        }
    }
    
    /// <summary>
    /// Initialize terminal state (will be loaded by main save system if save exists)
    /// </summary>
    private void LoadSaveData()
    {
        EnsureTerminalID();
        
        // Start with default values - the main save system will call LoadSaveData(TerminalSaveData) if a save exists
        hasCompleted = false;
        hasInteracted = false;
    }
    
    /// <summary>
    /// Get save data for external save system integration
    /// </summary>
    public TerminalSaveData GetSaveData()
    {
        EnsureTerminalID();
        var saveData = new TerminalSaveData
        {
            terminalID = terminalID,
            hasAccessed = hasCompleted, // Map hasCompleted to hasAccessed for compatibility
            hasInteracted = hasInteracted
        };
        
        return saveData;
    }
    
    /// <summary>
    /// Load save data from external save system
    /// </summary>
    public void LoadSaveData(TerminalSaveData saveData)
    {
        if (saveData != null && saveData.terminalID == terminalID)
        {
            hasCompleted = saveData.hasAccessed; // Map hasAccessed to hasCompleted for compatibility
            hasInteracted = saveData.hasInteracted;
            
            // Inform the game controller of the completion state
            if (gameController != null)
            {
                gameController.SetCompletionState(hasCompleted);
            }
        }
    }
    
    /// <summary>
    /// Reset terminal for testing (not used in production)
    /// </summary>
    public void ResetTerminal()
    {
        hasCompleted = false;
        hasInteracted = false;
        
        if (gameController != null)
        {
            gameController.SetCompletionState(false);
            gameController.ResetCircuit();
        }
    }
    
    /// <summary>
    /// Get completion status for external systems
    /// </summary>
    public bool IsCompleted()
    {
        return hasCompleted;
    }
}
