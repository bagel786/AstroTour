using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DNASlot : MonoBehaviour
{
    [Header("Slot Configuration")]
    public string displayedBase = "A"; // What shows on the left side (A, T, C, G)
    public string expectedBase = "T";  // What the player needs to place (T, A, G, C)
    
    [Header("Visual Components")]
    public Image slotBackground;
    public TextMeshProUGUI displayText;  // Shows the displayed base (left side)
    public TextMeshProUGUI slotLabel;    // Shows what's needed (right side when filled)
    
    [Header("Visual Colors")]
    public Color emptyColor = Color.gray;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    
    private DNABase currentBase;
    private bool hasBase = false;
    private bool wasCorrect = false;
    
    void Start()
    {
        InitializeSlot();
    }
    
    private void InitializeSlot()
    {
        // Auto-find components if not assigned
        if (slotBackground == null)
            slotBackground = GetComponent<Image>();
            
        if (displayText == null)
            displayText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Set initial appearance
        SetEmptyState();
        
        // Show the displayed base
        if (displayText != null)
            displayText.text = displayedBase;
    }
    
    public bool CanAcceptBase(DNABase dnaBase)
    {
        if (dnaBase == null) return false;
        if (hasBase) return false; // Slot already occupied
        
        // Check if the dragged base is the complement of the displayed base
        return dnaBase.CanPairWith(displayedBase);
    }
    
    public bool IsCorrectMatch(DNABase dnaBase)
    {
        if (dnaBase == null) return false;
        return dnaBase.CanPairWith(displayedBase);
    }
    
    public bool TryPlaceBase(DNABase dnaBase)
    {
        if (hasBase) return false; // Slot already occupied
        
        currentBase = dnaBase;
        hasBase = true;
        wasCorrect = IsCorrectMatch(dnaBase);
        
        // Update visual state
        if (wasCorrect)
        {
            SetCorrectState();
        }
        else
        {
            SetIncorrectState();
        }
        
        // Show what was placed
        if (slotLabel != null)
            slotLabel.text = dnaBase.GetBaseType();
        
        return true;
    }
    
    public void RemoveBase()
    {
        if (currentBase != null)
        {
            currentBase = null;
        }
        
        hasBase = false;
        wasCorrect = false;
        SetEmptyState();
        
        // Clear the placed base label
        if (slotLabel != null)
            slotLabel.text = "";
    }
    
    public void SetEmptyState()
    {
        if (slotBackground != null)
            slotBackground.color = emptyColor;
    }
    
    public void SetCorrectState()
    {
        if (slotBackground != null)
            slotBackground.color = correctColor;
    }
    
    public void SetIncorrectState()
    {
        if (slotBackground != null)
            slotBackground.color = incorrectColor;
    }
    
    public void ShowCompletedState()
    {
        // This method is called when showing a completed game state
        // The actual base placement is handled by the controller
        SetCorrectState();
    }
    
    public void ResetSlot()
    {
        RemoveBase();
    }
    
    public bool HasFragment()
    {
        return hasBase;
    }
    
    public bool WasCorrect()
    {
        return wasCorrect;
    }
    
    public DNABase GetCurrentBase()
    {
        return currentBase;
    }
    
    public string GetDisplayedBase()
    {
        return displayedBase;
    }
    
    public string GetExpectedBase()
    {
        return expectedBase;
    }
    
    /// <summary>
    /// Get the complementary base for the displayed base
    /// </summary>
    public string GetComplementaryBase()
    {
        switch (displayedBase.ToUpper())
        {
            case "A": return "T";
            case "T": return "A";
            case "C": return "G";
            case "G": return "C";
            default: return "A";
        }
    }
    
    /// <summary>
    /// Set the displayed and expected bases for this slot
    /// </summary>
    public void SetBases(string displayed, string expected)
    {
        displayedBase = displayed;
        expectedBase = expected;
        
        if (displayText != null)
            displayText.text = displayed;
    }
}