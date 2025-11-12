using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleDNASequencingController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gamePanel;
    
    [Header("UI Components")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI scoreText;
    public Button closeButton;
    
    [Header("Game Components")]
    public DNASlot[] targetSlots;
    public DNABase[] dnaBases;
    
    private int currentScore = 0;
    private int maxScore = 4; // 4 base pairs
    private bool isCompleted = false;
    
    void Start()
    {
        InitializeGame();
        SetupUI();
    }
    
    private void InitializeGame()
    {
        // Auto-find components if not assigned - only look within this game's panel
        if (targetSlots == null || targetSlots.Length == 0)
        {
            if (gamePanel != null)
            {
                targetSlots = gamePanel.GetComponentsInChildren<DNASlot>();
            }
            else
            {
                targetSlots = FindObjectsByType<DNASlot>(FindObjectsSortMode.None);
            }
        }
        
        if (dnaBases == null || dnaBases.Length == 0)
        {
            if (gamePanel != null)
            {
                dnaBases = gamePanel.GetComponentsInChildren<DNABase>();
            }
            else
            {
                dnaBases = FindObjectsByType<DNABase>(FindObjectsSortMode.None);
            }
        }
        
        // Set max score based on slots found for this specific game
        maxScore = targetSlots != null ? targetSlots.Length : 4;
        
        // Initially hide game panel
        if (gamePanel != null)
            gamePanel.SetActive(false);
    }
    
    private void SetupUI()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseGame);
            
        UpdateUI();
    }
    
    public void StartGame()
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }
        
        if (!isCompleted)
        {
            ResetGame();
        }
        else
        {
            ShowCompletedState();
        }
    }
    
    public void ShowCompletedState()
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }
        
        // Place bases in their correct slots and show completed state
        PlaceBasesInCorrectSlots();
        
        // Disable drag interactions but keep original colors
        foreach (DNABase dnaBase in dnaBases)
        {
            if (dnaBase != null)
            {
                dnaBase.SetInteractable(false);
            }
        }
        
        UpdateUI();
    }
    
    private void PlaceBasesInCorrectSlots()
    {
        // Clear any existing placements first
        foreach (DNASlot slot in targetSlots)
        {
            if (slot != null)
            {
                slot.ResetSlot();
            }
        }
        
        foreach (DNABase dnaBase in dnaBases)
        {
            if (dnaBase != null)
            {
                dnaBase.ResetBase();
            }
        }
        
        // Place each base in its correct slot
        foreach (DNASlot slot in targetSlots)
        {
            if (slot != null)
            {
                string neededBase = slot.GetComplementaryBase();
                
                // Find the matching base
                foreach (DNABase dnaBase in dnaBases)
                {
                    if (dnaBase != null && dnaBase.GetBaseType() == neededBase && !dnaBase.IsPlaced())
                    {
                        // Move base to slot position
                        dnaBase.transform.SetParent(slot.transform);
                        RectTransform baseRect = dnaBase.GetComponent<RectTransform>();
                        if (baseRect != null)
                        {
                            baseRect.anchorMin = new Vector2(0.5f, 0.5f);
                            baseRect.anchorMax = new Vector2(0.5f, 0.5f);
                            baseRect.pivot = new Vector2(0.5f, 0.5f);
                            baseRect.anchoredPosition = Vector2.zero;
                        }
                        
                        // Mark as placed and set slot state
                        slot.TryPlaceBase(dnaBase);
                        slot.SetCorrectState();
                        break;
                    }
                }
            }
        }
        
        // Update score to reflect completion
        currentScore = maxScore;
    }
    
    public void OnBasePlaced(DNASlot slot, DNABase placedBase)
    {
        if (isCompleted) return;
        
        // Check if this placement is correct
        bool isCorrect = slot.IsCorrectMatch(placedBase);
        
        if (isCorrect)
        {
            slot.SetCorrectState();
            
            // Play success sound
            SoundEffectManager.Play("dna_correct_placement");
        }
        else
        {
            slot.SetIncorrectState();
            
            // Play error sound
            SoundEffectManager.Play("dna_incorrect_placement");
        }
        
        UpdateUI();
        CheckGameCompletion();
    }
    
    public void OnBaseRemoved(DNASlot slot)
    {
        if (isCompleted) return;
        
        slot.SetEmptyState();
        UpdateUI();
        CheckGameCompletion(); // Recalculate score after removal
    }
    
    private void CheckGameCompletion()
    {
        // Check if all slots are filled with correct bases
        int correctlyFilledSlots = 0;
        int totalSlots = targetSlots != null ? targetSlots.Length : 0;
        
        foreach (DNASlot slot in targetSlots)
        {
            if (slot != null && slot.HasFragment() && slot.WasCorrect())
            {
                correctlyFilledSlots++;
            }
        }
        
        // Update current score to reflect actual correct placements
        currentScore = correctlyFilledSlots;
        
        // Complete game only when all slots are correctly filled
        if (correctlyFilledSlots >= totalSlots && totalSlots > 0)
        {
            OnGameComplete();
        }
    }
    
    private void OnGameComplete()
    {
        isCompleted = true;
        
        // Update instruction text with success message
        if (instructionText != null)
        {
            instructionText.text = "Congratulations! You correctly matched each base with its corresponding pair!";
        }
        
        // Play completion sound
        SoundEffectManager.Play("dna_sequence_complete");
        
        // Notify terminal
        SimpleDNASequencingTerminal terminal = FindAnyObjectByType<SimpleDNASequencingTerminal>();
        if (terminal != null)
        {
            terminal.OnChallengeCompleted();
        }
        
        Debug.Log("DNA Sequencing game completed!");
    }
    
    public void ResetGame()
    {
        currentScore = 0;
        isCompleted = false;
        
        // Reset all slots
        foreach (DNASlot slot in targetSlots)
        {
            if (slot != null)
            {
                slot.ResetSlot();
            }
        }
        
        // Reset all bases
        foreach (DNABase dnaBase in dnaBases)
        {
            if (dnaBase != null)
            {
                dnaBase.ResetBase();
                dnaBase.SetInteractable(true);
            }
        }
        

        
        UpdateUI();
        
        Debug.Log("DNA Sequencing game reset");
    }
    
    private void UpdateUI()
    {
        if (instructionText != null)
        {
            if (isCompleted)
            {
                instructionText.text = "Congratulations! You correctly matched each base with its corresponding pair!";
            }
            else
            {
                instructionText.text = "Drag DNA bases to pair them with their complements!";
            }
        }
        
        if (scoreText != null)
        {
            // Recalculate score in real-time to ensure accuracy
            int realTimeScore = 0;
            if (targetSlots != null)
            {
                foreach (DNASlot slot in targetSlots)
                {
                    if (slot != null && slot.HasFragment() && slot.WasCorrect())
                    {
                        realTimeScore++;
                    }
                }
            }
            
            scoreText.text = $"Correct Pairs: {realTimeScore}/{maxScore}";
            
            // Update currentScore to match real-time calculation
            currentScore = realTimeScore;
        }
    }
    
    public void CloseGame()
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }
        
        // Notify terminal that game was closed
        SimpleDNASequencingTerminal terminal = FindAnyObjectByType<SimpleDNASequencingTerminal>();
        if (terminal != null)
        {
            terminal.OnGameClosed();
        }
    }
    
    public bool IsCompleted()
    {
        return isCompleted;
    }
    
    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        if (completed)
        {
            currentScore = maxScore;
        }
    }
}