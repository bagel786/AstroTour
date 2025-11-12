# Simple Console Password Setup

## 🎯 What This Does
- Player interacts with terminal
- Password panel opens, game pauses
- Player enters password and clicks submit
- Shows "ACCESS GRANTED" or "ACCESS DENIED"
- Close button resumes game

## 📋 Setup Steps

### 1. Create UI Panel
1. **Create Panel** → Name it "PasswordPanel"
2. **Add to Panel:**
   - **TMP Input Field** → for password entry
   - **Button** → "Submit" 
   - **Button** → "Close"
   - **Text (TMP)** → for feedback messages
3. **IMPORTANT: Set Panel to Inactive in Inspector** → Uncheck the checkbox next to the panel name

### 2. Attach Scripts
1. **PasswordPanel** → Add `SimpleConsolePassword.cs`
2. **Terminal GameObject** → Add `SimpleTerminalInteract.cs`

### 3. Configure SimpleConsolePassword
```
Input Field: [Drag TMP Input Field]
Feedback Text: [Drag feedback Text]
Submit Button: [Drag Submit Button]
Close Button: [Drag Close Button]
Correct Password: "securepatch"
Success Message: "ACCESS GRANTED"
Fail Message: "ACCESS DENIED"
Terminal ID: (Auto-generated, or set custom unique ID)
```

**Note**: Terminal ID is automatically generated using the GameObject's name (e.g., "terminal_PasswordPanel"). You can set a custom unique ID if you prefer (e.g., "server_terminal_1"). **Important**: Each terminal must have a unique ID for save/load to work correctly.

### 4. Configure SimpleTerminalInteract
```
Console Password: [Drag PasswordPanel]
```

### 5. Set Terminal Layer
- Set terminal GameObject layer to "Terminal"
- PlayerController2.FindTerminal() will detect SimpleTerminalInteract components

## 🎮 How It Works

### First Time (Password Not Entered):
1. Player approaches terminal and presses E
2. `PlayerController2.FindTerminal()` detects `SimpleTerminalInteract` component
3. `SimpleTerminalInteract.Interact()` calls `consolePassword.OpenConsole()`
4. Panel activates, game pauses, input field is empty and active
5. Player types password and clicks Submit
6. `CheckPassword()` validates and shows feedback
7. If correct: input becomes non-interactable, shows success message
8. Close button calls `CloseConsole()` and resumes game

### After Correct Password Entered:
1. Player interacts with terminal again
2. Panel opens showing the correct password in input field
3. Input field is disabled (non-interactable)
4. Shows "ACCESS GRANTED" message
5. Player can only close the console

## 📝 PlayerController2 Integration
The PlayerController2 has been updated to work with SimpleTerminalInteract:
- Uses raycast to detect terminals on "Terminal" layer
- Looks for `SimpleTerminalInteract` component
- Stops player movement when interacting
- **Ignores E key input when game is paused** (fixes typing issue in console)
- Works the same as NPC and Box interactions

## 🔧 Testing
- Set `correctPassword` to "test" for easy testing
- Check `hasAccessed` property to see if terminal was successfully accessed
- Check `hasInteracted` property to see if player has attempted any password
- **Test Flow**: 
  1. Interact → Enter wrong password → Should clear input, allow retry
  2. Interact → Enter correct password → Should disable input, show success
  3. Interact again → Should show password in field, "ACCESS GRANTED", input disabled
  4. **Save/Load Test**: Access terminal, save game, reload - terminal should remember accessed state

## 💾 Save System Integration
The terminal system automatically integrates with the existing save system:
- **Terminal ID**: Each terminal gets a unique ID for save identification
- **State Persistence**: `hasAccessed` and `hasInteracted` are saved/loaded
- **Automatic**: No additional setup required - works with existing SaveController
- **New Game**: Terminals reset to initial state when starting new game

## 🐛 Troubleshooting

### Can't Type "E" in Password Field
**Problem**: E key doesn't work when typing in console
**Solution**: PlayerController2 now ignores E key when game is paused
**Fixed**: ✅ OnTalkPerformed checks PauseController.IsGamePaused

### Console Won't Open
- Check if SimpleTerminalInteract has consolePassword reference
- Verify terminal is on "Terminal" layer
- Make sure terminal has a Collider2D component
- **Panel can start inactive** - buttons are now set up in Awake()

### Game Pauses But No UI Panel Appears
**Most Common Issue**: Panel setup problem
1. **Check Console Logs** - Look for "OpenConsole called" and "Panel active state" messages
2. **Verify Panel is Inactive in Inspector** - Uncheck the panel GameObject in hierarchy
3. **Check References** - Make sure SimpleTerminalInteract has consolePassword assigned
4. **Canvas Issues** - Make sure panel is child of a Canvas with proper Canvas Scaler

### Game Won't Pause/Resume
- Verify PauseController.SetPause() is working
- Check if other systems respect PauseController.IsGamePaused

That's it! Simple and focused on just the core functionality you need.