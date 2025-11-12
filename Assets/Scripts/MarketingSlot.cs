using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketingSlot : MonoBehaviour
{
    [Header("Slot Configuration")]
    public string slotName;
    public string acceptedAudience; // "young_adults", "families", "seniors", "professionals"
    
    [Header("UI References")]
    public TMP_Text slotLabel;
    public Image slotBackground;
    
    [Header("Visual Feedback")]
    public Color emptyColor = Color.gray;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    
    private MarketingCard currentCard;
    private bool hasCard = false;
    
    void Start()
    {
        // Set up slot display
        if (slotLabel != null)
        {
            slotLabel.text = slotName;
        }
        
        // Ensure slot is fully visible (fix fading issue)
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        SetSlotColor(emptyColor);
    }
    
    public bool CanAcceptCard(MarketingCard card)
    {
        // Check if slot is empty and card matches target audience
        return !hasCard && card.targetAudience == acceptedAudience;
    }
    
    public void SetCard(MarketingCard card)
    {
        currentCard = card;
        hasCard = true;
        
        // Visual feedback
        SetSlotColor(correctColor);
        if (currentCard != null)
        {
            currentCard.ShowCorrectFeedback();
        }
        
        // Notify the game controller that a card was placed
        MarketingGameController gameController = FindAnyObjectByType<MarketingGameController>();
        if (gameController != null)
        {
            gameController.OnCardPlaced();
        }
    }
    
    public void RemoveCard()
    {
        if (currentCard != null)
        {
            currentCard.ResetAppearance();
        }
        
        currentCard = null;
        hasCard = false;
        SetSlotColor(emptyColor);
        
        // Notify the game controller that a card was removed
        MarketingGameController gameController = FindAnyObjectByType<MarketingGameController>();
        if (gameController != null)
        {
            gameController.OnCardPlaced(); // This will recalculate the score
        }
    }
    
    public bool HasCorrectCard()
    {
        return hasCard && currentCard != null && currentCard.targetAudience == acceptedAudience;
    }
    
    public MarketingCard GetCard()
    {
        return currentCard;
    }
    
    private void SetSlotColor(Color color)
    {
        if (slotBackground != null)
        {
            slotBackground.color = color;
        }
    }
    
    public void ShowIncorrectFeedback()
    {
        SetSlotColor(incorrectColor);
        if (currentCard != null)
        {
            currentCard.ShowIncorrectFeedback();
        }
    }
    
    public void ResetAppearance()
    {
        if (hasCard)
        {
            SetSlotColor(correctColor);
            if (currentCard != null)
            {
                currentCard.ShowCorrectFeedback();
            }
        }
        else
        {
            SetSlotColor(emptyColor);
        }
    }
    
    /// <summary>
    /// Places the correct card for this slot (used when showing completed state)
    /// </summary>
    public void PlaceCorrectCard()
    {
        // Find the correct card for this slot
        MarketingCard[] allCards = FindObjectsByType<MarketingCard>(FindObjectsSortMode.None);
        
        foreach (MarketingCard card in allCards)
        {
            if (card != null && card.targetAudience == acceptedAudience)
            {
                // Set card as child of this slot (same as drag handler does)
                card.transform.SetParent(transform);
                
                // Get the RectTransform for proper UI positioning
                RectTransform cardRectTransform = card.GetComponent<RectTransform>();
                if (cardRectTransform != null)
                {
                    // Set up proper anchoring to center the card in the slot
                    cardRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    cardRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    cardRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    cardRectTransform.anchoredPosition = Vector2.zero;
                }
                
                // Set the card in this slot
                currentCard = card;
                hasCard = true;
                
                // Set slot background to green to show completion
                SetSlotColor(correctColor);
                
                // Show correct feedback on the card as well
                if (currentCard != null)
                {
                    currentCard.ShowCorrectFeedback(); // Show green color on card too
                }
                
                // Disable the drag handler since it's completed
                MarketingDragHandler dragHandler = card.GetComponent<MarketingDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.enabled = false;
                }
                

                break;
            }
        }
    }
}