using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketingGameController : MonoBehaviour
{
    [Header("Game Configuration")]
    public MarketingSlot[] targetSlots;
    public MarketingCard[] marketingCards;
    
    [Header("UI References")]
    public GameObject gamePanel;
    public GameObject completionPanel;
    public TMP_Text instructionText;
    public TMP_Text scoreText;
    public Button closeButton;
    
    [Header("Game Settings")]
    public int pointsPerCorrectMatch = 10;
    public float feedbackDisplayTime = 1.5f;
    
    private int currentScore = 0;
    private int correctMatches = 0;
    private bool gameCompleted = false;
    
    void Start()
    {
        InitializeGame();
        SetupUI();
        
        // Ensure game panel starts inactive
        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }
    }
    
    private void InitializeGame()
    {
        // Find components if not assigned
        if (targetSlots.Length == 0)
        {
            targetSlots = FindObjectsByType<MarketingSlot>(FindObjectsSortMode.None);
        }
        
        if (marketingCards.Length == 0)
        {
            marketingCards = FindObjectsByType<MarketingCard>(FindObjectsSortMode.None);
        }
        
        // Ensure all cards are fully visible
        foreach (MarketingCard card in marketingCards)
        {
            if (card != null)
            {
                MarketingDragHandler dragHandler = card.GetComponent<MarketingDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.ResetVisualState();
                }
            }
        }
        
        // Ensure all slots are fully visible
        foreach (MarketingSlot slot in targetSlots)
        {
            if (slot != null)
            {
                CanvasGroup slotCanvasGroup = slot.GetComponent<CanvasGroup>();
                if (slotCanvasGroup != null)
                {
                    slotCanvasGroup.alpha = 1f;
                    slotCanvasGroup.blocksRaycasts = true;
                }
            }
        }
        
        // Reset game state (but preserve completion state if already set by save system)
        if (!gameCompleted)
        {
            currentScore = 0;
            correctMatches = 0;
        }
        // If gameCompleted is true, keep the score and matches as set by SetCompletionState
        
        // Show game panel, hide completion panel
        if (gamePanel != null) gamePanel.SetActive(true);
        if (completionPanel != null) completionPanel.SetActive(false);
        
        UpdateUI();
    }
    
    private void SetupUI()
    {
        // Set instruction text
        if (instructionText != null)
        {
            instructionText.text = "Drag each marketing strategy to the correct target audience!";
        }
        
        // Setup buttons
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseGame);
        }
        
        UpdateUI();
    }
    
    public void OnCardPlaced()
    {

        if (gameCompleted) return;
        
        // Check if this placement is correct
        CheckGameState();
    }
    
    private void CheckGameState()
    {
        correctMatches = 0;
        
        // Count correct matches
        foreach (MarketingSlot slot in targetSlots)
        {
            if (slot.HasCorrectCard())
            {
                correctMatches++;
            }
        }
        
        // Update score
        currentScore = correctMatches * pointsPerCorrectMatch;
        UpdateUI();
        
        // Check if game is complete
        if (correctMatches == targetSlots.Length)
        {
            StartCoroutine(CompleteGame());
        }
    }
    
    private IEnumerator CompleteGame()
    {

        gameCompleted = true;
        
        // Show success feedback
        if (instructionText != null)
        {
            instructionText.text = "Excellent! All marketing strategies matched correctly!";
        }
        
        // Wait a moment for player to see the success
        yield return new WaitForSeconds(feedbackDisplayTime);
        
        // Show completion
        ShowCompletionScreen();
        
        // Integrate with your existing systems
        CompleteMarketingQuest();
    }
    
    private void ShowCompletionScreen()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
        
        // Simple debug message instead of dialogue
        Debug.Log($"Marketing Challenge Complete! Scored {currentScore} points by correctly matching all strategies.");
    }
    
    private void CompleteMarketingQuest()
    {
        // Notify terminal of completion
        MarketingTerminal terminal = FindAnyObjectByType<MarketingTerminal>();
        if (terminal != null)
        {
            terminal.OnChallengeCompleted();
        }
        else
        {
            Debug.LogError("MarketingGameController: No MarketingTerminal found in scene!");
        }
        
        // Give rewards using your reward system
        GiveCompletionReward();
    }
    
    private void GiveCompletionReward()
    {
        // Example: Give coins or items as reward
        if (DialogueRewardManager.Instance != null)
        {
            // You could create a DialogueItemReward for this
            Debug.Log($"MarketingGame: Player earned {currentScore} points and completed the challenge!");
        }
        
        // Or integrate with your inventory system directly
        // InventoryController.Instance.AddItem(rewardItemID, quantity);
    }
    

    
    public void CloseGame()
    {
        // Hide all game UI
        if (gamePanel != null) gamePanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
        
        // Hide dialogue if showing
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.ShowDialogueUI(false);
        }
        
        // Unpause the game
        PauseController.SetPause(false);
        
        Debug.Log("Marketing game closed and game unpaused");
    }
    
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {correctMatches}/{targetSlots.Length}";
        }
    }
    
    // Method to start the game (can be called from dialogue choices or NPCs)
    public void StartMarketingChallenge()
    {
        // Only initialize if not already completed
        if (!gameCompleted)
        {
            InitializeGame();
            Debug.Log("Marketing Challenge started! Drag each marketing strategy to match it with the correct target audience.");
        }
        else
        {
            Debug.Log("Marketing Challenge already completed - showing completed state.");
        }
    }
    
    // Public method for external systems to check if game is available
    public bool IsGameCompleted()
    {
        return gameCompleted;
    }
    
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Enable/disable game interaction (for completed terminals)
    /// </summary>
    public void SetInteractionEnabled(bool enabled)
    {
        // Disable drag handlers on all cards
        foreach (MarketingCard card in marketingCards)
        {
            if (card != null)
            {
                MarketingDragHandler dragHandler = card.GetComponent<MarketingDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.enabled = enabled;
                }
            }
        }
        
        // Note: Don't disable the game panel here - let the calling code handle panel visibility
        // The panel should stay active when showing completed state
        

    }
    
    /// <summary>
    /// Set the game completion state (called when loading from save)
    /// </summary>
    public void SetCompletionState(bool completed)
    {
        gameCompleted = completed;
        
        if (completed)
        {
            correctMatches = targetSlots.Length;
            currentScore = correctMatches * pointsPerCorrectMatch;

        }
        else
        {
            correctMatches = 0;
            currentScore = 0;

        }
    }
    
    /// <summary>
    /// Show the completed state of the game (all correct answers in place)
    /// </summary>
    public void ShowCompletedState()
    {
        // Set game as completed
        gameCompleted = true;
        
        // Ensure game panel is active
        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }
        
        // Place all cards in their correct slots
        foreach (MarketingSlot slot in targetSlots)
        {
            if (slot != null)
            {
                slot.PlaceCorrectCard();
            }
        }
        
        // Update UI to show completion
        correctMatches = targetSlots.Length;
        currentScore = correctMatches * pointsPerCorrectMatch;
        UpdateUI();
        
        // Show completion message
        if (instructionText != null)
        {
            instructionText.text = "Challenge Already Completed! All marketing strategies correctly matched.";
        }
        
        // Show completion panel
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
        
        // Disable drag interactions but keep panel active
        SetInteractionEnabled(false);
        

    }
}