# Design Document

## Overview

This feature extends the existing dialogue system to allow NPCs to give items directly to players during conversations. The system integrates with the current DialogueController, InventoryController, and NPCDialogue architecture while adding new components for item reward management and tracking.

The design leverages the existing RewardsController pattern but creates a separate dialogue-specific reward system that can be triggered during conversations rather than only at quest completion. Items are given automatically when dialogue choices are selected, with feedback provided through the existing ItemPickupUIController side panel notification system.

## Architecture

### Core Components

1. **DialogueRewardManager** - New singleton component for managing dialogue-based item rewards and tracking
2. **Enhanced NPCDialogue** - Extended to support item rewards at specific dialogue indices
3. **Enhanced LabNPC** - Modified to handle item reward logic when displaying dialogue lines

### Integration Points

- **DialogueController**: Extended to display item reward information in dialogue UI
- **InventoryController**: Uses existing AddItem() method for item management
- **SaveController**: Integrates with existing save/load system to track given items
- **ItemDictionary**: Uses existing item lookup system

## Components and Interfaces

### Enhanced NPCDialogue
```csharp
[System.Serializable]
public class DialogueItemReward
{
    public int itemID;                    // Reference to item in ItemDictionary
    public int quantity = 1;              // Amount to give
    public int dialogueIndex;             // Which dialogue line triggers this reward
    public bool canGiveMultipleTimes;     // Whether item can be given repeatedly
    public string uniqueRewardID;        // Unique identifier for tracking
}

public class NPCDialogue : ScriptableObject
{
    // Existing fields...
    
    [Header("Item Rewards")]
    public DialogueItemReward[] itemRewards;     // Items to give at specific dialogue indices
}
```

### DialogueRewardManager (MonoBehaviour)
```csharp
public class DialogueRewardManager : MonoBehaviour
{
    public static DialogueRewardManager Instance { get; private set; }
    
    // Track which rewards have been given
    private List<string> givenRewards = new List<string>();
    
    // Methods
    public bool CanGiveReward(DialogueItemReward reward);
    public bool GiveReward(DialogueItemReward reward);
    public void MarkRewardAsGiven(string rewardID);
    public bool HasReceivedReward(string rewardID);
    public void LoadGivenRewards(List<string> savedRewards);
    public List<string> GetGivenRewards();
}
```

### Enhanced DialogueController
```csharp
public class DialogueController : MonoBehaviour
{
    // Existing fields...
    // No additional UI elements needed - uses existing pickup notification system
}
```

## Data Models

### DialogueItemReward Data Structure
- **itemID**: Links to existing Item system via ItemDictionary
- **quantity**: Number of items to give (supports stacking)
- **dialogueIndex**: Which dialogue line triggers this reward (editor configurable)
- **canGiveMultipleTimes**: Prevents/allows repeat rewards
- **uniqueRewardID**: String identifier for save system tracking

### Save Data Integration
Extends existing SaveController and SaveData with:
```csharp
// Added to existing SaveData class
[System.Serializable]
public class SaveData
{
    // Existing fields...
    public List<string> givenDialogueRewardIDs = new List<string>();
}
```

## Error Handling

### Inventory Full Scenarios
1. **Primary**: Attempt to add to inventory using existing InventoryController.AddItem()
2. **Fallback**: If inventory full, offer options:
   - Drop item in world near player
   - Defer reward until inventory space available
   - Show clear message to player about full inventory

### Invalid Item References
1. **Validation**: Check ItemDictionary for valid itemID before giving reward
2. **Logging**: Log warnings for missing items during development
3. **Graceful Degradation**: Continue dialogue flow even if item reward fails

### Duplicate Reward Prevention
1. **Tracking**: Use DialogueRewardManager to track given rewards
2. **Validation**: Check if reward already given before offering
3. **Save Persistence**: Ensure tracking survives game sessions

## Testing Strategy

### Unit Testing Focus Areas
1. **DialogueRewardManager**: Reward tracking and validation logic
2. **Item Validation**: Ensure valid itemID references
3. **Save/Load**: Verify reward tracking persists across sessions

### Integration Testing
1. **Dialogue Flow**: Test complete dialogue-to-inventory flow
2. **UI Integration**: Verify reward UI displays correctly
3. **Inventory Integration**: Test with full/empty inventory scenarios

### Manual Testing Scenarios
1. **Basic Flow**: NPC gives item, player receives it
2. **Inventory Full**: Test behavior when inventory cannot accept item
3. **Repeat Interactions**: Verify one-time rewards aren't repeated
4. **Save/Load**: Ensure reward state persists across game sessions
5. **Multiple Items**: Test giving multiple different items in one dialogue

## Implementation Approach

### Phase 1: Core Infrastructure
- Implement DialogueRewardManager singleton
- Add reward tracking to save system
- Create DialogueItemReward serializable class

### Phase 2: Dialogue Integration
- Extend NPCDialogue with item reward array
- Modify LabNPC to check for and give rewards when displaying dialogue lines
- Use existing ItemPickupUIController for reward notifications

### Phase 3: Testing and Refinement
- Handle edge cases (full inventory, invalid items)
- Comprehensive testing of all scenarios
- Integration with existing save/load system