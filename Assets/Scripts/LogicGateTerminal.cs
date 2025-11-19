using UnityEngine;

public class LogicGateTerminal : MonoBehaviour, IInteractable
{
    [Header("Terminal Configuration")]
    public string terminalName = "Quant Logic Gate Factory";
    public string terminalID = "logic_gate_terminal"; // Unique ID for quest system
    
    [Header("Game References")]
    public LogicGateController gameController;
    
    private bool isCompleted = false;
    
    void Start()
    {
        if (gameController == null)
        {
            gameController = FindAnyObjectByType<LogicGateController>();
        }
        
        if (gameController == null)
        {
            Debug.LogError("LogicGateTerminal: No LogicGateController found in scene!");
        }
    }
    
    public bool canInteract()
    {
        Debug.Log("LogicGateTerminal: canInteract() called - returning true");
        return true;
    }
    
    public void Interact()
    {
        Debug.Log($"LogicGateTerminal: Interact() called on {terminalName}");
        
        if (gameController == null)
        {
            Debug.LogError("LogicGateTerminal: No LogicGateController found!");
            return;
        }
        
        Debug.Log($"LogicGateTerminal: GameController found, isCompleted = {isCompleted}");
        
        // Pause game and show game panel
        Debug.Log("LogicGateTerminal: Calling PauseController.SetPause(true)");
        PauseController.SetPause(true);
        
        if (isCompleted)
        {
            Debug.Log("LogicGateTerminal: Game already completed, showing completed state");
            gameController.ShowCompletedState();
        }
        else
        {
            Debug.Log("LogicGateTerminal: Starting new logic gate challenge");
            gameController.StartLogicGateChallenge();
        }
        
        Debug.Log($"LogicGateTerminal: Interaction complete for {terminalName}");
    }
    
    public void OnChallengeCompleted()
    {
        isCompleted = true;
        
        // Update quest progress using terminal ID
        if (QuestController.Instance != null)
        {
            QuestController.Instance.CheckTerminalUnlock(terminalID);
        }
        
        Debug.Log($"{terminalName} challenge completed!");
    }
    
    public bool IsCompleted()
    {
        return isCompleted;
    }
    
    public void SetCompletionState(bool completed)
    {
        isCompleted = completed;
        
        if (gameController != null)
        {
            gameController.SetCompletionState(completed);
        }
    }
    
}
