using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketingDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Configuration")]
    public bool isDraggable = true;
    public float dragAlpha = 0.6f;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Canvas canvas;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;
    private int originalSiblingIndex;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Ensure card is fully visible when game starts
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // Store original anchoring values
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        // Store current anchoring values in case they were changed
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;
        
        // Move to top of canvas hierarchy to render on top
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        
        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // Check if we dropped on a valid target
        MarketingSlot targetSlot = GetDropTarget(eventData);
        
        if (targetSlot != null && targetSlot.CanAcceptCard(GetComponent<MarketingCard>()))
        {
            // Place in slot
            PlaceInSlot(targetSlot);
        }
        else
        {
            // Return to original position
            ReturnToOriginalPosition();
        }
    }
    
    private MarketingSlot GetDropTarget(PointerEventData eventData)
    {
        // Raycast to find what we're hovering over
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            MarketingSlot slot = result.gameObject.GetComponent<MarketingSlot>();
            if (slot != null)
            {
                return slot;
            }
        }
        
        return null;
    }
    
    private void PlaceInSlot(MarketingSlot slot)
    {
        // Remove card from previous slot if it was in one
        MarketingCard card = GetComponent<MarketingCard>();
        MarketingSlot previousSlot = GetCurrentSlot();
        if (previousSlot != null && previousSlot != slot)
        {
            previousSlot.RemoveCard();
        }
        
        transform.SetParent(slot.transform);
        
        // Set proper anchoring to center the card in the slot
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        slot.SetCard(card);
    }
    
    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        
        // Restore original anchoring values
        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;
        rectTransform.anchoredPosition = originalPosition;
    }
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
    
    /// <summary>
    /// Reset the card's visual state (alpha, position, etc.)
    /// </summary>
    public void ResetVisualState()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    /// <summary>
    /// Get the current slot this card is in (if any)
    /// </summary>
    private MarketingSlot GetCurrentSlot()
    {
        if (transform.parent != null)
        {
            return transform.parent.GetComponent<MarketingSlot>();
        }
        return null;
    }
}