using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GateSlot : MonoBehaviour
{
    [Header("Slot Configuration")]
    public string slotName;
    public string[] acceptedGateTypes; // Can accept multiple gate types
    public int[] inputSignals = new int[2]; // Binary inputs [0,1]
    
    [Header("UI References")]
    public TMP_Text slotLabel;
    public TMP_Text inputDisplay;
    public TMP_Text outputDisplay;
    public Image slotBackground;
    
    [Header("Visual Feedback")]
    public Color emptyColor = new Color(0.3f, 0.3f, 0.3f);
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    
    private LogicGate currentGate;
    private bool hasGate = false;
    private int expectedOutput = -1; // Set by controller
    
    void Start()
    {
        if (slotLabel != null)
        {
            slotLabel.text = slotName;
        }
        
        UpdateInputDisplay();
        SetSlotColor(emptyColor);
    }
    
    public bool CanAcceptGate(LogicGate gate)
    {
        if (hasGate || gate == null) return false;
        
        // Check if this gate type is accepted
        foreach (string acceptedType in acceptedGateTypes)
        {
            if (gate.GetGateType() == acceptedType)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void SetGate(LogicGate gate)
    {
        currentGate = gate;
        hasGate = true;
        gate.SetPlaced(true);
        
        // Calculate output
        int output = CalculateOutput();
        UpdateOutputDisplay(output);
        
        // Check if correct
        bool isCorrect = (expectedOutput == -1) || (output == expectedOutput);
        
        if (isCorrect)
        {
            SetSlotColor(correctColor);
            currentGate.ShowCorrectFeedback();
        }
        else
        {
            SetSlotColor(incorrectColor);
            currentGate.ShowIncorrectFeedback();
        }
        
        // Notify controller
        LogicGateController controller = FindAnyObjectByType<LogicGateController>();
        if (controller != null)
        {
            controller.OnGatePlaced();
        }
    }
    
    public void RemoveGate()
    {
        if (currentGate != null)
        {
            currentGate.ResetAppearance();
        }
        
        currentGate = null;
        hasGate = false;
        SetSlotColor(emptyColor);
        
        if (outputDisplay != null)
        {
            outputDisplay.text = "?";
        }
        
        // Notify controller
        LogicGateController controller = FindAnyObjectByType<LogicGateController>();
        if (controller != null)
        {
            controller.OnGatePlaced();
        }
    }
    
    private int CalculateOutput()
    {
        if (currentGate == null) return -1;
        
        string gateType = currentGate.GetGateType();
        int a = inputSignals.Length > 0 ? inputSignals[0] : 0;
        int b = inputSignals.Length > 1 ? inputSignals[1] : 0;
        
        switch (gateType)
        {
            case "AND":
                return (a == 1 && b == 1) ? 1 : 0;
            case "OR":
                return (a == 1 || b == 1) ? 1 : 0;
            case "XOR":
                return (a != b) ? 1 : 0;
            case "NOT":
                return (a == 1) ? 0 : 1;
            case "SUM":
                return a + b; // For aggregation
            case "NORM":
                return (a + b > 0) ? 1 : 0; // Normalization
            default:
                return 0;
        }
    }
    
    public bool HasCorrectGate()
    {
        if (!hasGate || currentGate == null) return false;
        
        int output = CalculateOutput();
        return (expectedOutput == -1) || (output == expectedOutput);
    }
    
    public void SetExpectedOutput(int output)
    {
        expectedOutput = output;
    }
    
    public int GetCurrentOutput()
    {
        return hasGate ? CalculateOutput() : -1;
    }
    
    private void UpdateInputDisplay()
    {
        if (inputDisplay != null)
        {
            if (inputSignals.Length == 1)
            {
                inputDisplay.text = $"In: {inputSignals[0]}";
            }
            else if (inputSignals.Length >= 2)
            {
                inputDisplay.text = $"In: {inputSignals[0]},{inputSignals[1]}";
            }
        }
    }
    
    private void UpdateOutputDisplay(int output)
    {
        if (outputDisplay != null)
        {
            outputDisplay.text = $"Out: {output}";
        }
    }
    
    private void SetSlotColor(Color color)
    {
        if (slotBackground != null)
        {
            slotBackground.color = color;
        }
    }
    
    public void ResetSlot()
    {
        RemoveGate();
    }
    
    public void PlaceCorrectGate()
    {
        // Find a gate that produces the expected output
        LogicGate[] allGates = FindObjectsByType<LogicGate>(FindObjectsSortMode.None);
        
        foreach (LogicGate gate in allGates)
        {
            if (gate != null && !gate.IsPlaced() && CanAcceptGate(gate))
            {
                // Test if this gate produces correct output
                string testType = gate.GetGateType();
                int testOutput = TestGateOutput(testType);
                
                if (testOutput == expectedOutput)
                {
                    // Place this gate
                    gate.transform.SetParent(transform);
                    RectTransform gateRect = gate.GetComponent<RectTransform>();
                    if (gateRect != null)
                    {
                        gateRect.anchorMin = new Vector2(0.5f, 0.5f);
                        gateRect.anchorMax = new Vector2(0.5f, 0.5f);
                        gateRect.pivot = new Vector2(0.5f, 0.5f);
                        gateRect.anchoredPosition = Vector2.zero;
                    }
                    
                    SetGate(gate);
                    break;
                }
            }
        }
    }
    
    private int TestGateOutput(string gateType)
    {
        int a = inputSignals.Length > 0 ? inputSignals[0] : 0;
        int b = inputSignals.Length > 1 ? inputSignals[1] : 0;
        
        switch (gateType)
        {
            case "AND": return (a == 1 && b == 1) ? 1 : 0;
            case "OR": return (a == 1 || b == 1) ? 1 : 0;
            case "XOR": return (a != b) ? 1 : 0;
            case "NOT": return (a == 1) ? 0 : 1;
            case "SUM": return a + b;
            case "NORM": return (a + b > 0) ? 1 : 0;
            default: return 0;
        }
    }
}
