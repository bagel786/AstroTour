# Console Password System Setup Guide

## 🎯 Overview
This guide will help you set up the console password system with proper pause functionality and close button logic.

## 📋 Step 1: Unity UI Setup

### Create the Console UI:
1. **Create Canvas** (if you don't have one)
2. **Create Panel** → Name it "ConsolePasswordPanel"
3. **Inside the Panel, add:**
   - **Text (TMP)** → Name it "PromptText" → Set text to "Enter Password:"
   - **TMP Input Field** → Name it "PasswordInput"
   - **Button** → Name it "SubmitButton" → Set text to "SUBMIT"
   - **Button** → Name it "CloseButton" → Set text to "CLOSE"
   - **Text (TMP)** → Name it "FeedbackText" → Leave empty

### Style the Console (Optional but Recommended):
- **Panel Background**: Dark gray/green (RGB: 25, 25, 25)
- **Text Color**: Bright green (RGB: 0, 255, 0)
- **Input Field**: Dark background with green text
- **Buttons**: Dark gray with green text

## 🔧 Step 2: Script Setup

### Attach Scripts:
1. **ConsolePasswordPanel** → Add `ConsolePassword.cs`
2. **Terminal GameObject** → Add `TerminalInteract.cs`
3. **Canvas or UI Manager** → Add `ConsoleUIManager.cs` (optional)

### Configure ConsolePassword Component:
```
Input Field: Drag PasswordInput
Feedback Text: Drag FeedbackText  
Submit Button: Drag SubmitButton
Close Button: Drag CloseButton
Prompt Text: Drag PromptText
Correct Password: "securepatch"
Success Message: "ACCESS GRANTED. SYSTEM REBOOTING..."
Fail Message: "ACCESS DENIED. TRY AGAIN."
```

### Configure TerminalInteract Component:
```
Console Password: Drag ConsolePasswordPanel
Terminal Name: "Security Terminal"
Interaction Prompt: "Press E to access terminal"
Is Accessible: ✓ (checked)
```

## 🎮 Step 3: Layer and Interaction Setup

### Create Terminal Layer:
1. **Edit → Project Settings → Tags and Layers**
2. **Add new layer**: "Terminal"
3. **Set your terminal GameObject layer** to "Terminal"

### Player Interaction System:
The terminal uses the existing interaction system that shows the player icon (E key prompt).

#### Player Setup (if not already configured):
1. **Player GameObject** needs:
   - `InteractionDetector.cs` component
   - **Collider2D** set as **Trigger** (for detecting nearby interactables)
   
2. **Interaction Icon UI**:
   - Create UI element for interaction prompt (usually an "E" key icon)
   - Assign this UI element to `InteractionDetector.interactionIcon`
   - Icon will automatically show/hide when near terminals

#### Terminal Setup:
1. **Terminal GameObject** needs:
   - `TerminalInteract.cs` component  
   - **Collider2D** set as **Trigger** (for player detection)
   - Layer set to "Terminal"

### Dual Interaction Methods:
The system supports both interaction methods:
- **Automatic**: InteractionDetector shows icon, player presses E
- **Manual**: PlayerController2.FindTerminal() via raycast (backup method)

## 🔗 Step 4: Integration

### Connect Everything:
1. **Terminal GameObject**:
   - Add `TerminalInteract.cs`
   - Set layer to "Terminal"
   - Add Collider2D (for interaction detection) - **Set as Trigger**
   - Assign ConsolePassword reference

2. **Console UI**:
   - Add `ConsolePassword.cs`
   - Wire up all UI references
   - Set initial state to inactive

3. **Player Interaction Icon Setup**:
   - **Player GameObject** should have `InteractionDetector.cs` (if not already present)
   - **Player** needs a **Collider2D set as Trigger** for interaction detection
   - **InteractionDetector** needs reference to interaction icon UI element
   - **Interaction Icon** appears automatically when near interactable terminals

4. **Test the Flow**:
   - Player approaches terminal
   - **Interaction icon appears** (E key prompt)
   - Press E to interact
   - Console opens, game pauses
   - Enter password or click close
   - Game resumes when console closes

## 🎯 Step 5: Advanced Features

### Quest Integration:
```csharp
// In ConsolePassword component
questObjectiveID = "unlock_server"; // Set this in inspector

// The system will automatically log quest completion
// You can extend this to actually complete quest objectives
```

### Multiple Terminals:
```csharp
// Each terminal can have different passwords
terminal1.GetComponent<ConsolePassword>().SetPassword("admin123");
terminal2.GetComponent<ConsolePassword>().SetPassword("security456");
```

### Audio and Effects:
```csharp
// Add AudioSource to terminal
// Assign access/denied sounds in TerminalInteract
// Add visual effects for successful access
```

## 🐛 Troubleshooting

### Console Won't Open:
- Check if TerminalInteract has ConsolePassword reference
- Verify terminal is on "Terminal" layer
- Make sure terminal has Collider2D component

### Game Won't Pause:
- Verify PauseController.SetPause(true) is being called
- Check if other systems respect PauseController.IsGamePaused

### Close Button Not Working:
- Ensure CloseButton has OnClick event pointing to ConsolePassword.CloseConsole()
- Check if button is interactable

### Input Not Working:
- Make sure Input Field is active and interactable
- Check if console GameObject is active when opened

### Interaction Icon Not Showing:
- Verify Player has InteractionDetector component
- Check if Player has Collider2D set as Trigger
- Ensure Terminal has Collider2D set as Trigger
- Verify InteractionDetector.interactionIcon is assigned
- Check if terminal.canInteract() returns true

## 🎨 Customization Examples

### Cybersecurity Theme:
```csharp
// Green terminal text
promptText.color = new Color(0f, 1f, 0f, 1f);

// Dark background
panel.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

// Add typing sound effects
// Add screen flicker effects
```

### Different Terminal Types:
```csharp
// Server Terminal
SetPassword("serveradmin");
SetPrompt("SERVER ACCESS REQUIRED:");

// Security Console  
SetPassword("security123");
SetPrompt("ENTER SECURITY CODE:");

// Database Terminal
SetPassword("dbaccess");
SetPrompt("DATABASE LOGIN:");
```

### Custom Interaction Feedback:
```csharp
// Get terminal status for UI
string status = terminal.GetTerminalStatus(); // "READY", "LOCKED", "ACCESSED"

// Get interaction prompt
string prompt = terminal.GetInteractionPrompt(); // "Press E to access terminal"

// Check if terminal was accessed
bool accessed = terminal.HasBeenAccessed();
```

### Visual Indicators:
```csharp
// Add visual feedback GameObjects to terminal
public GameObject terminalActiveIndicator; // Green light when accessed
public GameObject accessGrantedEffect;     // Particle effect on success

// These will automatically activate when terminal is successfully accessed
```

## 🚀 Next Steps

1. **Test the basic functionality**
2. **Add visual/audio polish**
3. **Integrate with quest system**
4. **Create multiple terminal types**
5. **Add more cybersecurity challenges**

The system is designed to be extensible - you can easily add more terminal types, different challenge formats, and integrate with your existing quest and dialogue systems!