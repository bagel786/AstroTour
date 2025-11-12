# Design Document

## Overview

This design extends the existing quest and terminal interaction systems to support password-based terminal unlocking quests. The system will integrate seamlessly with the current `Quest`, `QuestController`, `SimpleConsolePassword`, and `NPCDialogue` systems while adding new objective types and quest completion mechanisms.

The core concept involves:
1. Adding new objective types (`UnlockTerminal` and `TalkToNPC`) to the existing quest system
2. Extending terminal interactions to check for and complete relevant quests
3. Leveraging the existing dialogue system for password clue delivery
4. Maintaining full save/load compatibility with existing systems

## Architecture

### System Integration Points

The design builds upon these existing systems:
- **Quest System**: `Quest.cs`, `QuestController.cs`, `QuestUI.cs`
- **Terminal System**: `SimpleConsolePassword.cs`, `SimpleTerminalInteract.cs`
- **Dialogue System**: `NPCDialogue.cs`, `DialogueController.cs`
- **Save System**: `SaveData.cs`, `SaveController.cs`

### New Components

1. **Enhanced ObjectiveType Enum**: Add `UnlockTerminal` and `TalkToNPC` types
2. **Terminal Quest Integration**: Extend `SimpleConsolePassword` to notify quest system
3. **NPC Interaction Tracking**: Extend quest system to track NPC conversations
4. **Quest Validation Logic**: New methods to check terminal unlock completion

## Components and Interfaces

### Enhanced Quest System

#### Modified ObjectiveType Enum
```csharp
public enum ObjectiveType
{
    CollectItem,
    DefeatEnemy,
    ReachLocation,
    TalkNPC,        // New: Talk to specific NPC
    UnlockTerminal, // New: Unlock specific terminal with password
    Custom
}
```

#### Enhanced QuestObjective Class
The existing `QuestObjective` class will be extended to support:
- `objectiveID` field repurposed for terminal IDs and NPC names
- New validation methods for terminal and NPC objectives
- Backward compatibility with existing objectives

#### QuestController Extensions
New methods in `QuestController`:
- `CheckNPCInteraction(string npcName)`: Called when player talks to NPCs
- `CheckTerminalUnlock(string terminalID)`: Called when terminals are unlocked
- `UpdateObjectiveProgress(ObjectiveType type, string targetID)`: Generic progress updater

### Terminal System Integration

#### SimpleConsolePassword Extensions
Enhanced to support quest integration:
- Quest completion notification when correct password is entered
- Terminal ID validation for quest matching
- Maintains existing functionality for non-quest terminals

#### Terminal Quest Validation
- Check active quests when password is successfully entered
- Update quest progress for matching `UnlockTerminal` objectives
- Provide feedback when quest objectives are completed

### NPC Interaction System

#### NPCDialogue Integration
Leverage existing dialogue system:
- No changes needed to `NPCDialogue.cs` - clues delivered through normal dialogue
- Quest system tracks which NPCs have been talked to
- Uses existing dialogue reward system for any additional mechanics

#### NPC Interaction Tracking
- Track NPC interactions in quest system
- Match NPC names/IDs with `TalkToNPC` objectives
- Update quest progress when required NPCs are talked to

## Data Models

### Quest Objective Extensions

The existing `QuestObjective` class supports the new objective types through its current structure:

```csharp
// For UnlockTerminal objectives:
// - objectiveID: Terminal ID to match (e.g., "terminal_SecurityConsole")
// - type: ObjectiveType.UnlockTerminal
// - requiredAmount: Always 1 (terminal unlocked or not)
// - currentAmount: 0 (not unlocked) or 1 (unlocked)

// For TalkToNPC objectives:
// - objectiveID: NPC name or identifier
// - type: ObjectiveType.TalkToNPC  
// - requiredAmount: Always 1 (talked to or not)
// - currentAmount: 0 (not talked to) or 1 (talked to)
```

### Save Data Integration

The existing save system already supports terminal states through `TerminalSaveData`. Quest progress is saved through the existing `QuestProgress` system. No additional save data structures are needed.

### Terminal-Quest Relationship

Terminals and quests are linked through:
- Terminal `terminalID` matches quest objective `objectiveID`
- Quest system queries terminal unlock status
- Terminal system notifies quest system of successful unlocks

## Error Handling

### Quest Configuration Validation
- Validate terminal IDs exist in scene during quest creation
- Warn about missing NPCs for `TalkToNPC` objectives
- Ensure objective amounts are valid (1 for terminal/NPC objectives)

### Runtime Error Handling
- Graceful handling of missing terminals or NPCs
- Fallback behavior for invalid quest configurations
- Logging for debugging quest progression issues

### Save/Load Robustness
- Handle missing terminal references in saved quests
- Validate quest data integrity on load
- Recover from corrupted quest objective data

## Testing Strategy

### Unit Testing Focus Areas
- Quest objective validation for new types
- Terminal-quest integration logic
- NPC interaction tracking accuracy
- Save/load data integrity

### Integration Testing Scenarios
- Complete terminal password quest workflow
- Multiple simultaneous terminal quests
- NPC interaction quest completion
- Save/load with active terminal quests
- Backward compatibility with existing quests

### Edge Case Testing
- Terminal unlocked before quest is active
- Quest completed through different means
- Invalid terminal/NPC references in quests
- Multiple quests targeting same terminal
- Quest completion with missing components

## Implementation Considerations

### Backward Compatibility
- All existing quest types continue to work unchanged
- Existing terminal functionality preserved
- No breaking changes to save data format
- Existing NPCs and dialogues work without modification

### Performance Impact
- Minimal overhead: only check active quests on terminal unlock
- Efficient NPC interaction tracking through existing dialogue system
- No continuous polling or expensive operations

### Extensibility
- Design supports future objective types
- Terminal system can be extended for other quest mechanics
- NPC system integration allows for complex dialogue-quest interactions
- Modular approach enables easy feature additions

### User Experience
- Seamless integration with existing UI systems
- Clear quest descriptions for new objective types
- Intuitive progression feedback
- Consistent behavior with existing quest mechanics