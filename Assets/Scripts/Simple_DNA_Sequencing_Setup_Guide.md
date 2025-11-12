# Simple DNA Sequencing Game Setup

## Overview
This is a simplified DNA sequencing game that works exactly like the marketing game:
- **Drag-and-drop** DNA base pairing (A↔T, C↔G)
- Always accessible terminal (no item requirements)
- **Full quest system integration** - triggers quest completion on finish
- **Complete save system integration** - saves completion state automatically
- **Same architecture** as marketing terminal for consistency

## Quick Setup

### 1. Create Terminal GameObject
1. Create empty GameObject named "SimpleDNASequencingTerminal"
2. Add `SimpleDNASequencingTerminal` component
3. Configure in inspector:
```
Terminal Name: "DNA Sequencing Terminal"
Terminal ID: (leave empty to auto-generate)
```

### 2. Create Game Controller
1. Create empty GameObject named "SimpleDNASequencingController"  
2. Add `SimpleDNASequencingController` component
3. Link controller to terminal's `gameController` field

### 3. Create UI Canvas Structure
```
DNA Sequencing Canvas
├── Game Panel
│   ├── Left Side (DNA Strand Display)
│   │   ├── DNA Slot 1 (displayedBase: "A")i
│   │   ├── DNA Slot 2 (displayedBase: "G") 
│   │   ├── DNA Slot 3 (displayedBase: "C")
│   │   └── DNA Slot 4 (displayedBase: "T")
│   ├── Right Side (Draggable Bases)
│   │   ├── DNA Base A (baseType: "A") + DNADragHandler
│   │   ├── DNA Base T (baseType: "T") + DNADragHandler
│   │   ├── DNA Base C (baseType: "C") + DNADragHandler
│   │   └── DNA Base G (baseType: "G") + DNADragHandler
│   ├── Instruction Text
│   ├── Score Text
│   └── Close Button
```

### 4. Configure DNA Slots
For each DNA slot:
- Add `DNASlot` component
- Set `displayedBase` (what shows on left - A, T, C, or G)
- The system automatically determines what base is needed (complement)
- Assign UI references (slotBackground, displayText, slotLabel)

**Base Pairing Rules:**
- A (displayed) ↔ T (dragged)
- T (displayed) ↔ A (dragged)  
- C (displayed) ↔ G (dragged)
- G (displayed) ↔ C (dragged)

### 5. Configure Draggable DNA Bases  
For each draggable base:
- Add `DNABase` component
- Add `DNADragHandler` component
- Add `CanvasGroup` component (for drag transparency)
- Set `baseType` (A, T, C, or G)
- Assign UI references (baseImage, baseText)
- Colors set automatically: A=Red, T=Green, C=Blue, G=Yellow

### 6. Link Components
In SimpleDNASequencingController:
- Assign `gamePanel`
- Assign `instructionText`, `scoreText`, `closeButton`
- `targetSlots` and `dnaBases` arrays will be found automatically

In SimpleDNASequencingTerminal:
- Assign `gameController`

## Game Flow
1. **Player interacts with terminal**
   - Game panel opens, game pauses
   - Shows DNA strand on left (e.g., A-G-C-T)
   - Shows draggable bases on right (A, T, C, G)

2. **Player drags bases to complete pairs**
   - Drag A to pair with T, drag C to pair with G, etc.
   - Correct placements turn green, incorrect turn red
   - Score updates as matches are made
   - Bases snap to slots when dropped correctly

3. **Game completes when all pairs matched**
   - Shows success message in instruction text: "Congratulations! You correctly matched each base with its corresponding pair!"
   - Saves completion state
   - Notifies quest system
   - Game remains open for player to view completed state

4. **Subsequent interactions show completed state**
   - All bases already in correct positions
   - Drag interactions disabled but panel viewable

## Quest System Integration

The DNA terminal automatically integrates with your quest system:

### Quest Objective Setup
Create quest objectives that target the terminal ID:
```
Objective Type: Terminal
Objective ID: "dna_terminal_X.X_Y.Y" (auto-generated based on position)
Description: "Complete DNA sequencing analysis"
```

### Automatic Quest Completion
When the DNA game is completed:
1. Terminal calls `QuestController.Instance.CheckTerminalUnlock(terminalID)`
2. Quest system matches terminal ID with quest objectives
3. Matching quests are automatically completed

## Save System Integration

The DNA terminal is fully integrated with your save system:

### Automatic Save Data
- **Completion state** - whether terminal has been completed
- **Interaction state** - whether terminal has been accessed
- **Terminal ID** - unique identifier for save/load matching

### Save/Load Process
1. **On completion**: Auto-saves game state via SaveController
2. **On game load**: Restores completion and interaction states
3. **Cross-session**: Maintains state between game sessions

### Save Data Structure
```csharp
TerminalSaveData {
    terminalID: "dna_terminal_1.0_2.0"
    hasAccessed: true/false
    hasInteracted: true/false
}
```

## Layer Setup
Put the terminal GameObject on the **"Terminal"** layer so PlayerController2.cs can detect it.

You'll need to modify `FindTerminal()` in PlayerController2.cs to check for `SimpleDNASequencingTerminal`:

```csharp
void FindTerminal()
{
    RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("Terminal"));

    if (hit.collider != null)
    {
        // Check for SimpleTerminalInteract first
        SimpleTerminalInteract terminal = hit.collider.GetComponent<SimpleTerminalInteract>();
        if (terminal != null && terminal.canInteract())
        {
            // ... existing code
            return;
        }

        // Check for SimpleDNASequencingTerminal
        SimpleDNASequencingTerminal dnaTerminal = hit.collider.GetComponent<SimpleDNASequencingTerminal>();
        if (dnaTerminal != null && dnaTerminal.canInteract())
        {
            move = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            StopFootSteps();
            dnaTerminal.Interact();
            return;
        }
    }
}
```

## Testing Checklist
- [ ] Terminal opens game panel when interacted with
- [ ] DNA bases can be **dragged** to compatible slots
- [ ] Correct pairs (A-T, C-G) turn green and snap into place
- [ ] Incorrect pairs return to original position
- [ ] Game completes when all pairs matched
- [ ] Completion state saves and loads correctly
- [ ] Subsequent interactions show completed state
- [ ] Game pauses/resumes correctly

## Drag System Components Required
Each draggable DNA base needs:
1. **DNABase** script (handles base type and pairing logic)
2. **DNADragHandler** script (handles drag and drop mechanics)  
3. **CanvasGroup** component (for transparency during drag)
4. **Image** component (for visual representation)
5. **TextMeshPro - Text (UI)** child (for base letter display)

This simplified system works exactly like your marketing game but with DNA base pairing drag-and-drop mechanics!