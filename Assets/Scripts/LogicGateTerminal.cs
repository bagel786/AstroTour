using UnityEngine;

public class LogicGateTerminal : MonoBehaviour, IInteractable
{
    [Header("Terminal Configuration")]
    public string terminalName = "Quant Logic Gate Factory";
    public string interactionPrompt = "Press E to access Logic Gate Factory";
    
    [Header("Game References")]
    public LogicGateController gameController;
    
    [Header("Quest Integration")]
    public string questID = "logic_gate_challenge";
    public string objectiveID = "complete_logic_gate_puzzle";
    
    private bool isCompleted = false;
    
    void Start()
    {
        if (gameController == null)
        {
            gameController = FindAnyObjectByType<LogicGateController>();
        }
    }
    
    public string GetInteractionPrompt()
    {
        if (isCompleted)
        {
            return "Press E to view completed Logic Gate Factory";
        }
        return interactionPrompt;
    }
    
    public bool canInteract()
    {
        // Can always interact - will show different states based on completion
        return true;
    }
    
    public void Interact()
    {
        if (gameController == null)
        {
            Debug.LogError("LogicGateTerminal: No LogicGateController found!");
            return;
        }
        
        PauseController.SetPause(true);
        
        if (isCompleted)
        {
            gameController.ShowCompletedState();
        }
        else
        {
            gameController.StartLogicGateChallenge();
        }
        
        Debug.Log($"Interacted with {terminalName}");
    }
    
    public void OnChallengeCompleted()
    {
        isCompleted = true;
        
        // Update quest progress using terminal ID
        if (QuestController.Instance != null)
        {
            QuestController.Instance.CheckTerminalUnlock(questID);
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
