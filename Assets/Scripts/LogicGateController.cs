using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicGateController : MonoBehaviour
{
    [Header("Game Configuration")]
    public GateSlot[] circuitSlots;
    public LogicGate[] availableGates;
    
    [Header("UI References")]
    public GameObject gamePanel;
    public GameObject completionPanel;
    public TMP_Text instructionText;
    public TMP_Text scoreText;
    public TMP_Text targetPatternText;
    public Button closeButton;
    public Button resetButton;
    
    [Header("Game Settings")]
    public int[] targetOutputPattern; // Expected outputs for each slot
    public int pointsPerCorrectGate = 10;
    public float feedbackDisplayTime = 1.5f;
    public bool allowFatTailReset = true; // If true, wrong placement triggers full reset
    
    private int currentScore = 0;
    private int correctPlacements = 0;
    private bool gameCompleted = false;
    
    void Start()
    {
        InitializeGame();
        SetupUI();
        
        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }
    }
    
    private void InitializeGame()
    {
        if (circuitSlots.Length == 0)
        {
            circuitSlots = FindObjectsByType<GateSlot>(FindObjectsSortMode.None);
        }
        
        if (availableGates.Length == 0)
        {
            availableGates = FindObjectsByType<LogicGate>(FindObjectsSortMode.None);
        }
        
        // Set expected outputs for each slot
        for (int i = 0; i < circuitSlots.Length && i < targetOutputPattern.Length; i++)
        {
            circuitSlots[i].SetExpectedOutput(targetOutputPattern[i]);
        }
        
        // Ensure all gates are visible
        foreach (LogicGate gate in availableGates)
        {
            if (gate != null)
            {
                CanvasGroup gateCanvasGroup = gate.GetComponent<CanvasGroup>();
                if (gateCanvasGroup != null)
                {
                    gateCanvasGroup.alpha = 1f;
                    gateCanvasGroup.blocksRaycasts = true;
                }
            }
        }
        
        if (!gameCompleted)
        {
            currentScore = 0;
            correctPlacements = 0;
        }
        
        if (gamePanel != null) gamePanel.SetActive(true);
        if (completionPanel != null) completionPanel.SetActive(false);
        
        UpdateUI();
    }
    
    private void SetupUI()
    {
        if (instructionText != null)
        {
            instructionText.text = "Route signals through logic gates to match the target output pattern!";
        }
        
        if (targetPatternText != null)
        {
            string pattern = "Target: [";
            for (int i = 0; i < targetOutputPattern.Length; i++)
            {
                pattern += targetOutputPattern[i];
                if (i < targetOutputPattern.Length - 1) pattern += ", ";
            }
            pattern += "]";
            targetPatternText.text = pattern;
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseGame);
        }
        
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetCircuit);
        }
        
        UpdateUI();
    }
    
    public void OnGatePlaced()
    {
        if (gameCompleted) return;
        
        CheckGameState();
    }
    
    private void CheckGameState()
    {
        correctPlacements = 0;
        bool hasIncorrectPlacement = false;
        
        foreach (GateSlot slot in circuitSlots)
        {
            if (slot.HasCorrectGate())
            {
                correctPlacements++;
            }
            else if (slot.GetCurrentOutput() != -1 && !slot.HasCorrectGate())
            {
                hasIncorrectPlacement = true;
            }
        }
        
        currentScore = correctPlacements * pointsPerCorrectGate;
        UpdateUI();
        
        // Fat-tail event: if any gate is wrong, trigger reset
        if (allowFatTailReset && hasIncorrectPlacement)
        {
            StartCoroutine(TriggerFatTailEvent());
            return;
        }
        
        // Check completion
        if (correctPlacements == circuitSlots.Length)
        {
            StartCoroutine(CompleteGame());
        }
    }
    
    private IEnumerator TriggerFatTailEvent()
    {
        if (instructionText != null)
        {
            instructionText.text = "FAT-TAIL EVENT! Circuit mismatch detected. Resetting...";
        }
        
        SoundEffectManager.Play("logic_gate_error");
        
        yield return new WaitForSeconds(1.5f);
        
        ResetCircuit();
    }
    
    private IEnumerator CompleteGame()
    {
        gameCompleted = true;
        
        if (instructionText != null)
        {
            instructionText.text = "SUCCESS! Target distribution achieved!";
        }
        
        SoundEffectManager.Play("logic_gate_complete");
        
        yield return new WaitForSeconds(feedbackDisplayTime);
        
        ShowCompletionScreen();
        CompleteLogicGateQuest();
    }
    
    private void ShowCompletionScreen()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
        
        Debug.Log($"Logic Gate Challenge Complete! Scored {currentScore} points.");
    }
    
    private void CompleteLogicGateQuest()
    {
        LogicGateTerminal terminal = FindAnyObjectByType<LogicGateTerminal>();
        if (terminal != null)
        {
            terminal.OnChallengeCompleted();
        }
        
        GiveCompletionReward();
    }
    
    private void GiveCompletionReward()
    {
        if (DialogueRewardManager.Instance != null)
        {
            Debug.Log($"LogicGateController: Player earned {currentScore} points!");
        }
    }
    
    public void ResetCircuit()
    {
        foreach (GateSlot slot in circuitSlots)
        {
            if (slot != null)
            {
                slot.ResetSlot();
            }
        }
        
        foreach (LogicGate gate in availableGates)
        {
            if (gate != null)
            {
                gate.ResetGate();
                gate.SetInteractable(true);
            }
        }
        
        currentScore = 0;
        correctPlacements = 0;
        
        if (instructionText != null)
        {
            instructionText.text = "Route signals through logic gates to match the target output pattern!";
        }
        
        UpdateUI();
        
        Debug.Log("Logic gate circuit reset");
    }
    
    public void CloseGame()
    {
        if (gamePanel != null) gamePanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
        
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.ShowDialogueUI(false);
        }
        
        PauseController.SetPause(false);
        
        Debug.Log("Logic gate game closed");
    }
    
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Correct: {correctPlacements}/{circuitSlots.Length}";
        }
    }
    
    public void StartLogicGateChallenge()
    {
        if (!gameCompleted)
        {
            InitializeGame();
            Debug.Log("Logic Gate Challenge started!");
        }
        else
        {
            Debug.Log("Logic Gate Challenge already completed.");
        }
    }
    
    public bool IsGameCompleted()
    {
        return gameCompleted;
    }
    
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    public void SetInteractionEnabled(bool enabled)
    {
        foreach (LogicGate gate in availableGates)
        {
            if (gate != null)
            {
                gate.SetInteractable(enabled);
            }
        }
    }
    
    public void SetCompletionState(bool completed)
    {
        gameCompleted = completed;
        
        if (completed)
        {
            correctPlacements = circuitSlots.Length;
            currentScore = correctPlacements * pointsPerCorrectGate;
        }
        else
        {
            correctPlacements = 0;
            currentScore = 0;
        }
    }
    
    public void ShowCompletedState()
    {
        gameCompleted = true;
        
        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }
        
        foreach (GateSlot slot in circuitSlots)
        {
            if (slot != null)
            {
                slot.PlaceCorrectGate();
            }
        }
        
        correctPlacements = circuitSlots.Length;
        currentScore = correctPlacements * pointsPerCorrectGate;
        UpdateUI();
        
        if (instructionText != null)
        {
            instructionText.text = "Challenge Already Completed! Target distribution achieved.";
        }
        
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
        
        SetInteractionEnabled(false);
    }
}
