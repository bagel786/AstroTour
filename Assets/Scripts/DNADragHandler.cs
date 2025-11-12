using UnityEngine;
using UnityEngine.EventSystems;

public class DNADragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private DNABase dnaBase;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        dnaBase = GetComponent<DNABase>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Don't allow dragging if base is already placed
        if (dnaBase != null && dnaBase.IsPlaced()) return;
        
        // Store original position and parent for potential return
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // Make base semi-transparent while dragging
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        
        // Move to top of hierarchy for proper rendering
        transform.SetParent(transform.root);
        
        Debug.Log($"Started dragging DNA base: {dnaBase.GetBaseType()}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Follow mouse/touch position
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore appearance
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // Check if we dropped on a valid slot
        DNASlot targetSlot = GetDropTarget(eventData);
        
        if (targetSlot != null && targetSlot.CanAcceptBase(dnaBase))
        {
            // Valid drop - place base in slot
            PlaceInSlot(targetSlot);
            Debug.Log($"Placed base '{dnaBase.GetBaseType()}' to pair with '{targetSlot.GetDisplayedBase()}'");
        }
        else
        {
            // Invalid drop - return to original position
            ReturnToOriginalPosition();
            if (targetSlot != null)
            {
                Debug.Log($"Base '{dnaBase.GetBaseType()}' cannot pair with '{targetSlot.GetDisplayedBase()}'");
            }
        }
    }
    
    private DNASlot GetDropTarget(PointerEventData eventData)
    {
        // Raycast to find what we're hovering over
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            DNASlot slot = result.gameObject.GetComponent<DNASlot>();
            if (slot != null)
            {
                return slot;
            }
        }
        
        return null;
    }
    
    private void PlaceInSlot(DNASlot slot)
    {
        // Set new parent and position
        transform.SetParent(slot.transform);
        
        // Hard-code anchoring to center
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Notify slot that it has a base
        slot.TryPlaceBase(dnaBase);
        
        // Notify game controller about the placement
        SimpleDNASequencingController gameController = FindAnyObjectByType<SimpleDNASequencingController>();
        if (gameController != null)
        {
            gameController.OnBasePlaced(slot, dnaBase);
        }
    }
    
    private void ReturnToOriginalPosition()
    {
        // Return to original parent and position
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}