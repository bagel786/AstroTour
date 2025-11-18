using UnityEngine;
using UnityEngine.EventSystems;

public class LogicGateDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
    private LogicGate logicGate;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        logicGate = GetComponent<LogicGate>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        if (logicGate != null && logicGate.IsPlaced()) return;
        
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;
        
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
        
        GateSlot targetSlot = GetDropTarget(eventData);
        
        if (targetSlot != null && targetSlot.CanAcceptGate(logicGate))
        {
            PlaceInSlot(targetSlot);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }
    
    private GateSlot GetDropTarget(PointerEventData eventData)
    {
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            GateSlot slot = result.gameObject.GetComponent<GateSlot>();
            if (slot != null)
            {
                return slot;
            }
        }
        
        return null;
    }
    
    private void PlaceInSlot(GateSlot slot)
    {
        GateSlot previousSlot = GetCurrentSlot();
        if (previousSlot != null && previousSlot != slot)
        {
            previousSlot.RemoveGate();
        }
        
        transform.SetParent(slot.transform);
        
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        slot.SetGate(logicGate);
    }
    
    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        
        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;
        rectTransform.anchoredPosition = originalPosition;
    }
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
    
    private GateSlot GetCurrentSlot()
    {
        if (transform.parent != null)
        {
            return transform.parent.GetComponent<GateSlot>();
        }
        return null;
    }
}
