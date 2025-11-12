using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DNABase : MonoBehaviour
{
    [Header("DNA Base Configuration")]
    public string baseType = "A"; // A, T, G, or C
    
    [Header("Visual Components")]
    public Image baseImage;
    public TextMeshProUGUI baseText;
    
    [Header("Colors")]
    public Color baseColor = Color.white;
    
    private bool isPlaced = false;
    private DNASlot currentSlot;
    private Vector3 originalPosition;
    private Transform originalParent;
    
    void Start()
    {
        InitializeBase();
        StoreOriginalPosition();
    }
    
    private void InitializeBase()
    {
        // Auto-find components if not assigned
        if (baseImage == null)
            baseImage = GetComponent<Image>();
            
        if (baseText == null)
            baseText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Set base type colors and text
        SetBaseTypeColor();
        
        if (baseText != null)
            baseText.text = baseType;
    }
    
    private void StoreOriginalPosition()
    {
        originalPosition = transform.localPosition;
        originalParent = transform.parent;
    }
    
    private void SetBaseTypeColor()
    {
        switch (baseType.ToUpper())
        {
            case "A":
                baseColor = Color.red;
                break;
            case "T":
                baseColor = Color.green;
                break;
            case "C":
                baseColor = Color.blue;
                break;
            case "G":
                baseColor = Color.yellow;
                break;
        }
        
        if (baseImage != null)
            baseImage.color = baseColor;
    }
    
    public string GetBaseType()
    {
        return baseType;
    }
    
    public bool IsPlaced()
    {
        return isPlaced;
    }
    
    public void SetPlaced(bool placed, DNASlot slot = null)
    {
        isPlaced = placed;
        currentSlot = slot;
    }
    
    public DNASlot GetCurrentSlot()
    {
        return currentSlot;
    }
    
    public void ResetBase()
    {
        isPlaced = false;
        currentSlot = null;
        
        // Return to original position and parent
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
        }
    }
    
    public void SetInteractable(bool interactable)
    {
        // For drag system, we control this through the DNADragHandler
        DNADragHandler dragHandler = GetComponent<DNADragHandler>();
        if (dragHandler != null)
        {
            dragHandler.enabled = interactable;
        }
        
        // Keep original color when correctly placed, only gray out if not placed correctly
        if (baseImage != null)
        {
            // If the base is correctly placed, keep its original color
            if (IsPlaced() && currentSlot != null && currentSlot.IsCorrectMatch(this))
            {
                baseImage.color = baseColor; // Keep original color when correctly placed
            }
            else
            {
                baseImage.color = interactable ? baseColor : Color.gray;
            }
        }
    }
    
    public void SetBaseType(string newType)
    {
        baseType = newType.ToUpper();
        SetBaseTypeColor();
        
        if (baseText != null)
            baseText.text = baseType;
    }
    
    /// <summary>
    /// Check if this base can pair with the given base (A↔T, C↔G)
    /// </summary>
    public bool CanPairWith(string otherBase)
    {
        string thisBase = baseType.ToUpper();
        string other = otherBase.ToUpper();
        
        return (thisBase == "A" && other == "T") ||
               (thisBase == "T" && other == "A") ||
               (thisBase == "C" && other == "G") ||
               (thisBase == "G" && other == "C");
    }
    
    /// <summary>
    /// Get the complementary base for this base
    /// </summary>
    public string GetComplementaryBase()
    {
        switch (baseType.ToUpper())
        {
            case "A": return "T";
            case "T": return "A";
            case "C": return "G";
            case "G": return "C";
            default: return "A";
        }
    }
}