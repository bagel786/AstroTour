using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketingCard : MonoBehaviour
{
    [Header("Card Data")]
    public string cardText;
    public string targetAudience; // "young_adults", "families", "seniors", "professionals"
    
    [Header("UI References")]
    public TMP_Text cardTextDisplay;
    public Image cardBackground;
    
    [Header("Visual Feedback")]
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    
    private MarketingDragHandler dragHandler;
    
    void Start()
    {
        // Set up the card display
        if (cardTextDisplay != null)
        {
            cardTextDisplay.text = cardText;
        }
        
        // Get or add drag handler
        dragHandler = GetComponent<MarketingDragHandler>();
        if (dragHandler == null)
        {
            dragHandler = gameObject.AddComponent<MarketingDragHandler>();
        }
        
        // Set default appearance
        SetCardColor(defaultColor);
    }
    
    public void SetCardColor(Color color)
    {
        if (cardBackground != null)
        {
            cardBackground.color = color;
        }
    }
    
    public void ShowCorrectFeedback()
    {
        SetCardColor(correctColor);
    }
    
    public void ShowIncorrectFeedback()
    {
        SetCardColor(incorrectColor);
    }
    
    public void ResetAppearance()
    {
        SetCardColor(defaultColor);
    }
}