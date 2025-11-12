using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimpleConsolePassword : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;
    public TextMeshProUGUI feedbackText;
    public Button submitButton;
    public Button closeButton;

    [Header("Settings")]
    public string correctPassword = "securepatch";
    public string successMessage = "ACCESS GRANTED";
    public string failMessage = "ACCESS DENIED";

    [Header("Save System")]
    [Tooltip("Unique identifier for saving. Leave empty to auto-generate based on position.")]
    public string terminalID; // Unique identifier for saving

    public bool hasAccessed { get; private set; } = false;
    public bool hasInteracted { get; private set; } = false;

    void Awake()
    {
        // Hook up buttons in Awake so it works even if GameObject starts inactive
        if (submitButton != null) submitButton.onClick.AddListener(CheckPassword);
        if (closeButton != null) closeButton.onClick.AddListener(CloseConsole);
        
        // Generate unique ID if not set (this will be saved in the scene)
        EnsureTerminalID();
    }

    void Start()
    {
        // Clear feedback text
        if (feedbackText != null) feedbackText.text = "";

        // Note: Panel should be set to inactive in the Inspector, not in code
        // This prevents conflicts when OpenConsole() tries to activate it
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        // Allow Enter key to submit (only if not already accessed)
        if (Input.GetKeyDown(KeyCode.Return) && !hasAccessed)
        {
            CheckPassword();
        }

        // Allow Escape key to close
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseConsole();
        }
    }

    public void OpenConsole()
    {
        gameObject.SetActive(true);
        PauseController.SetPause(true);
        
        if (hasAccessed)
        {
            // Show successful state - password in input, access granted message
            if (inputField != null)
            {
                inputField.text = correctPassword;
                inputField.interactable = false; // Disable input
            }
            if (feedbackText != null)
            {
                feedbackText.text = successMessage;
                feedbackText.color = Color.green;
            }
        }
        else
        {
            // Normal state - clear input and focus
            if (inputField != null)
            {
                inputField.text = "";
                inputField.interactable = true; // Enable input
                inputField.ActivateInputField();
            }
            if (feedbackText != null)
            {
                feedbackText.text = "";
            }
        }
    }

    public void CloseConsole()
    {
        gameObject.SetActive(false);
        PauseController.SetPause(false);
    }

    void CheckPassword()
    {
        // Don't process if already accessed
        if (hasAccessed) return;

        string userInput = inputField.text.Trim();
        hasInteracted = true;

        if (userInput.ToLower() == correctPassword.ToLower())
        {
            // Correct password
            feedbackText.text = successMessage;
            feedbackText.color = Color.green;
            hasAccessed = true;
            
            // Disable further input
            inputField.interactable = false;
        }
        else
        {
            // Wrong password
            feedbackText.text = failMessage;
            feedbackText.color = Color.red;
            
            // Clear input and refocus for another attempt
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    /// <summary>
    /// Gets the current terminal state for saving
    /// </summary>
    /// <returns>TerminalSaveData containing current state</returns>
    public TerminalSaveData GetSaveData()
    {
        // Ensure terminal ID is generated before saving
        EnsureTerminalID();
        return new TerminalSaveData
        {
            terminalID = terminalID,
            hasAccessed = hasAccessed,
            hasInteracted = hasInteracted
        };
    }

    /// <summary>
    /// Loads terminal state from save data
    /// </summary>
    /// <param name="saveData">The save data to load</param>
    public void LoadSaveData(TerminalSaveData saveData)
    {
        // Ensure terminal ID is generated before comparing
        EnsureTerminalID();
        
        if (saveData != null && saveData.terminalID == terminalID)
        {
            hasAccessed = saveData.hasAccessed;
            hasInteracted = saveData.hasInteracted;
        }
    }

    /// <summary>
    /// Ensures terminal ID is generated (called before it's needed)
    /// </summary>
    private void EnsureTerminalID()
    {
        if (string.IsNullOrEmpty(terminalID))
        {
            // Use GameObject name for more predictable ID
            terminalID = $"terminal_{gameObject.name}";
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Validates terminal configuration in the editor
    /// </summary>
    private void OnValidate()
    {
        // Auto-generate ID in editor if empty
        if (string.IsNullOrEmpty(terminalID))
        {
            terminalID = $"terminal_{gameObject.name}";
        }
    }
    #endif

    /// <summary>
    /// Resets terminal to initial state (useful for new game)
    /// </summary>
    public void ResetTerminal()
    {
        hasAccessed = false;
        hasInteracted = false;
    }
}