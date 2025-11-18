using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicGate : MonoBehaviour
{
    [Header("Gate Configuration")]
    public string gateType; // "XOR", "AND", "OR", "NOT", "SUM", "NORM"
    public string gateDisplayName;
    
    [Header("UI References")]
    public Image gateImage;
    public TMP_Text gateLabel;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    
    private bool isPlaced = false;
    private Color originalColor;
    
    void Start()
    {
        if (gateLabel != null)
        {
            gateLabel.text = gateDisplayName;
        }
        
        if (gateImage != null)
        {
            originalColor = gateImage.color;
        }
    }
    
    public string GetGateType()
    {
        return gateType;
    }
    
    public bool IsPlaced()
    {
        return isPlaced;
    }
    
    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
    }
    
    public void ShowCorrectFeedback()
    {
        if (gateImage != null)
        {
            gateImage.color = correctColor;
        }
    }
    
    public void ShowIncorrectFeedback()
    {
        if (gateImage != null)
        {
            gateImage.color = incorrectColor;
        }
    }
    
    public void ResetAppearance()
    {
        if (gateImage != null)
        {
            gateImage.color = originalColor;
        }
        isPlaced = false;
    }
    
    public void SetInteractable(bool interactable)
    {
        LogicGateDragHandler dragHandler = GetComponent<LogicGateDragHandler>();
        if (dragHandler != null)
        {
            dragHandler.SetDraggable(interactable);
        }
    }
    
    public void ResetGate()
    {
        ResetAppearance();
        isPlaced = false;
    }
}
