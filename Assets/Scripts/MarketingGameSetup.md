# Marketing Match Mini-Game Setup Guide

## Overview
The Marketing Match mini-game allows players to drag marketing strategy cards to match them with the correct target audiences. It integrates with your existing dialogue, quest, and inventory systems.

## Required Scripts
- `MarketingCard.cs` - Individual marketing strategy cards
- `MarketingDragHandler.cs` - Handles drag and drop functionality
- `MarketingSlot.cs` - Target audience slots that accept cards
- `MarketingGameController.cs` - Main game logic and UI management
- `MarketingTerminal.cs` - Interactable object to start the game

## UI Structure Setup

### 1. Detailed GameObject Hierarchy
```
MarketingGamePanel (Canvas/Panel)
├── Header (Horizontal Layout Group)
│   ├── InstructionText (TMP_Text)
│   │   └── Anchor: Stretch/Middle, Font: 18pt Bold
│   └── ScoreText (TMP_Text)
│       └── Anchor: Middle/Right, Font: 16pt Regular
├── GameArea (Vertical Layout Group)
│   ├── CardsContainer (Horizontal Layout Group)
│   │   ├── Spacing: 20px, Child Alignment: Middle Center
│   │   ├── MarketingCard_SocialMedia (GameObject)
│   │   │   ├── Components: Image, MarketingCard, MarketingDragHandler, CanvasGroup
│   │   │   ├── Anchor: Middle Center (0.5, 0.5)
│   │   │   ├── Size: 200x120px
│   │   │   ├── CardText (TMP_Text Child)
│   │   │   │   ├── Anchor: Stretch/Stretch
│   │   │   │   ├── Margins: 10px all sides
│   │   │   │   └── Font: 14pt, Center Aligned
│   │   │   └── CardIcon (Image Child - Optional)
│   │   │       ├── Anchor: Top/Left
│   │   │       └── Size: 32x32px
│   │   ├── MarketingCard_FamilyFun (GameObject)
│   │   │   └── [Same structure as above]
│   │   ├── MarketingCard_SeniorDiscount (GameObject)
│   │   │   └── [Same structure as above]
│   │   └── MarketingCard_Professional (GameObject)
│   │       └── [Same structure as above]
│   └── SlotsContainer (Horizontal Layout Group)
│       ├── Spacing: 30px, Child Alignment: Middle Center
│       ├── Padding: 20px top/bottom
│       ├── YoungAdultsSlot (GameObject)
│       │   ├── Components: Image, MarketingSlot
│       │   ├── Anchor: Middle Center (0.5, 0.5)
│       │   ├── Size: 220x140px
│       │   ├── SlotLabel (TMP_Text Child)
│       │   │   ├── Anchor: Top/Stretch
│       │   │   ├── Height: 30px
│       │   │   ├── Font: 16pt Bold, Center Aligned
│       │   │   └── Text: "Young Adults (18-30)"
│       │   ├── SlotDescription (TMP_Text Child)
│       │   │   ├── Anchor: Middle/Stretch
│       │   │   ├── Margins: 10px sides
│       │   │   ├── Font: 12pt Regular, Center Aligned
│       │   │   └── Text: "Tech-savvy, social media active"
│       │   └── DropZone (Image Child)
│       │       ├── Anchor: Bottom/Stretch
│       │       ├── Height: 80px
│       │       ├── Margins: 10px all sides
│       │       └── Color: Transparent with dashed border
│       ├── FamiliesSlot (GameObject)
│       │   ├── [Same structure as YoungAdultsSlot]
│       │   ├── SlotLabel Text: "Families with Kids"
│       │   └── SlotDescription Text: "Budget-conscious, value-focused"
│       ├── SeniorsSlot (GameObject)
│       │   ├── [Same structure as YoungAdultsSlot]
│       │   ├── SlotLabel Text: "Seniors (55+)"
│       │   └── SlotDescription Text: "Traditional, quality-focused"
│       └── ProfessionalsSlot (GameObject)
│           ├── [Same structure as YoungAdultsSlot]
│           ├── SlotLabel Text: "Working Professionals"
│           └── SlotDescription Text: "Time-conscious, efficiency-focused"
└── ButtonsContainer (Horizontal Layout Group)
    ├── Anchor: Bottom/Stretch, Height: 60px
    ├── Spacing: 20px, Child Alignment: Middle Center
    └── CloseButton (Button)
        ├── Size: 120x40px
        ├── Text: "Close Game"
        └── Colors: Normal #F44336, Hover #DA190B
```

### 2. Layout Component Settings

#### CardsContainer (Horizontal Layout Group)
- **Child Alignment**: Middle Center
- **Spacing**: 20px
- **Child Force Expand**: Width ✓, Height ✗
- **Child Control Size**: Width ✓, Height ✓
- **Padding**: 10px all sides

#### SlotsContainer (Horizontal Layout Group)  
- **Child Alignment**: Middle Center
- **Spacing**: 30px
- **Child Force Expand**: Width ✓, Height ✗
- **Child Control Size**: Width ✓, Height ✓
- **Padding**: 20px top/bottom, 10px left/right

### 2. Optional Completion Panel
```
CompletionPanel (Panel - initially inactive)
├── SuccessText (TMP_Text)
├── FinalScoreText (TMP_Text)
└── ContinueButton (Button)
```

## Component Configuration

### Marketing Cards Setup
Configure each MarketingCard component with:

| Card GameObject | Card Text | Target Audience | Background Color | Size |
|----------------|-----------|-----------------|------------------|------|
| MarketingCard_SocialMedia | "Social Media Influencer Campaign" | "young_adults" | #E3F2FD (Light Blue) | 200x120px |
| MarketingCard_FamilyFun | "Family Fun Weekend Promotion" | "families" | #E8F5E8 (Light Green) | 200x120px |
| MarketingCard_SeniorDiscount | "Early Bird Senior Discounts" | "seniors" | #FFF3E0 (Light Orange) | 200x120px |
| MarketingCard_Professional | "Professional Networking Events" | "professionals" | #F3E5F5 (Light Purple) | 200x120px |

#### Card Anchoring Configuration:
- **Anchor Preset**: Middle Center
- **Anchor Min/Max**: (0.5, 0.5)
- **Pivot**: (0.5, 0.5)
- **Rect Transform**: Position (0, 0, 0)

**Additional Card Ideas:**
- "TikTok Dance Challenge" → "young_adults"
- "Back-to-School Bundle Deals" → "families"
- "Retirement Planning Seminars" → "seniors"
- "LinkedIn Premium Features" → "professionals"

### Marketing Slots Setup
Configure each MarketingSlot component with:

| Slot GameObject | Slot Name | Accepted Audience | Description | Background Color | Size |
|----------------|-----------|------------------|-------------|------------------|------|
| YoungAdultsSlot | "Young Adults (18-30)" | "young_adults" | "Tech-savvy, social media active" | #BBDEFB (Blue) | 220x140px |
| FamiliesSlot | "Families with Kids" | "families" | "Budget-conscious, value-focused" | #C8E6C9 (Green) | 220x140px |
| SeniorsSlot | "Seniors (55+)" | "seniors" | "Traditional, quality-focused" | #FFE0B2 (Orange) | 220x140px |
| ProfessionalsSlot | "Working Professionals" | "professionals" | "Time-conscious, efficiency-focused" | #E1BEE7 (Purple) | 220x140px |

#### Slot Anchoring Configuration:
- **Anchor Preset**: Middle Center
- **Anchor Min/Max**: (0.5, 0.5)
- **Pivot**: (0.5, 0.5)
- **Rect Transform**: Position (0, 0, 0)

#### Slot Child Components:
- **SlotLabel**: Anchor Top/Stretch, Height 30px, Font 16pt Bold
- **SlotDescription**: Anchor Middle/Stretch, Font 12pt Regular  
- **DropZone**: Anchor Bottom/Stretch, Height 80px, Dashed border

### Visual Styling
**Recommended Colors:**
- Empty Slot: Light Gray (#C0C0C0)
- Correct Match: Light Green (#90EE90)
- Incorrect Match: Light Red (#FFB6C1)
- Card Default: White (#FFFFFF)

## Scene Setup

### 1. Create Game Controller
1. Create empty GameObject named "MarketingGameManager"
2. Add `MarketingGameController` script
3. Assign UI references in inspector:
   - Game Panel
   - Completion Panel (optional)
   - Instruction Text
   - Score Text
   - Close Button
   - Target Slots array
   - Marketing Cards array

### 2. Create Interactable Terminal
1. Create GameObject with sprite (computer/terminal/sign)
2. Add `BoxCollider2D` set as trigger
3. Add `MarketingTerminal` script
4. Assign `MarketingGameController` reference
5. Configure terminal name

### 3. UI Canvas Setup
1. Create Canvas if not exists
2. Set Canvas Scaler to "Scale With Screen Size"
3. Add MarketingGamePanel as child
4. Initially set MarketingGamePanel inactive

## Integration with Existing Systems

### Terminal Quest Workflow
The marketing terminal works exactly like the password terminal system:

1. **Player can always interact** with terminal (no quest required to access)
2. **Player completes challenge** - terminal saves completion state
3. **Quest system automatically detects** completion via `CheckTerminalUnlock(terminalID)`
4. **If player has quest** - quest objective completes and rewards become available
5. **If no quest yet** - completion is saved, quest will show as completed when received later
6. **Terminal becomes inactive** after completion (like password terminals)

### Quest Integration
Add to your quest system:
```csharp
// In QuestController or quest data
{
    questID: "marketing_training_quest",
    questName: "Marketing Training Program",
    description: "Complete the marketing audience targeting training",
    objectives: ["Complete marketing training at the terminal"],
    isCompleted: false
}
```

### Terminal Configuration
In MarketingTerminal inspector:
- **Terminal ID**: Leave empty for auto-generation or set manually (e.g., "marketing_terminal_01")
- **Terminal Name**: Display name for debugging
- **Game Controller**: Assign MarketingGameController reference

**Auto-Generated Terminal ID:**
If left empty, terminal ID is generated as: `marketing_terminal_{x}_{y}` based on position

### Completion State Saving
- **Persistent storage** using PlayerPrefs with terminal ID
- **Quest completion** automatically triggered
- **Terminal deactivation** prevents re-completion
- **Drag interaction disabled** after completion

## Quest Setup Instructions

### 1. Create the Quest Data
Create a new Quest ScriptableObject:

**File → Create → Quest**
- **Quest ID**: "marketing_training_quest"
- **Quest Name**: "Marketing Training Program"
- **Description**: "Complete the marketing audience targeting training"
- **Objectives**: Add one objective:
  - **Objective Type**: UnlockTerminal
  - **Target ID**: Terminal's terminalID (check terminal inspector or let it auto-generate)
  - **Description**: "Complete marketing training at the terminal"
  - **Is Completed**: false

**Important:** The Target ID must exactly match the terminal's terminalID for quest completion to work.

### 2. Create Marketing Supervisor NPC
Create an NPC that gives the marketing quest:

**NPCDialogue Component Setup:**
```csharp
// Marketing Supervisor dialogue data
public class MarketingSupervisorNPC : MonoBehaviour, IInteractable
{
    [Header("Quest Configuration")]
    public Quest marketingTrainingQuest; // Drag your quest ScriptableObject here
    
    public bool canInteract() { return true; }
    
    public void Interact()
    {
        if (QuestController.Instance == null) return;
        
        bool hasQuest = QuestController.Instance.IsQuestActive("marketing_training_quest") || 
                       QuestController.Instance.IsQuestCompleted("marketing_training_quest");
        
        if (!hasQuest)
        {
            // Offer the quest
            ShowQuestOffer();
        }
        else if (IsMarketingTerminalCompleted())
        {
            // Training completed
            ShowCompletionMessage();
        }
        else
        {
            // Quest active, training not done
            ShowInProgressMessage();
        }
    }
    
    private void ShowQuestOffer()
    {
        DialogueController.Instance.SetNPCInfo("Marketing Supervisor", supervisorPortrait);
        DialogueController.Instance.SetDialogueText(
            "Welcome to the marketing department! We have a new training program " +
            "that teaches audience targeting strategies. Would you like to participate?"
        );
        DialogueController.Instance.ShowDialogueUI(true);
        DialogueController.Instance.ClearChoices();
        
        DialogueController.Instance.CreateChoiceButton("Accept Training", () => {
            QuestController.Instance.AcceptQuest(marketingTrainingQuest);
            DialogueController.Instance.SetDialogueText(
                "Excellent! Head to the marketing training terminal to begin. " +
                "You'll learn to match marketing strategies with target audiences."
            );
        });
        
        DialogueController.Instance.CreateChoiceButton("Maybe later", () => {
            DialogueController.Instance.ShowDialogueUI(false);
        });
    }
    
    private void ShowInProgressMessage()
    {
        DialogueController.Instance.SetNPCInfo("Marketing Supervisor", supervisorPortrait);
        DialogueController.Instance.SetDialogueText(
            "The marketing training terminal is ready for you. " +
            "Complete the audience targeting exercise to finish your training."
        );
        DialogueController.Instance.ShowDialogueUI(true);
    }
    
    private void ShowCompletionMessage()
    {
        DialogueController.Instance.SetNPCInfo("Marketing Supervisor", supervisorPortrait);
        DialogueController.Instance.SetDialogueText(
            "Outstanding work on the marketing training! You've mastered audience targeting. " +
            "These skills will be valuable in your marketing career."
        );
        DialogueController.Instance.ShowDialogueUI(true);
    }
    
    private bool IsMarketingTerminalCompleted()
    {
        MarketingTerminal terminal = FindAnyObjectByType<MarketingTerminal>();
        return terminal != null && terminal.IsCompleted();
    }
}
```

### 3. Scene Setup Checklist

#### Marketing Terminal Setup:
- [ ] **GameObject layer**: Set to "sign"
- [ ] **BoxCollider2D**: Added and set as trigger
- [ ] **MarketingTerminal script**: Attached
- [ ] **Terminal ID**: "marketing_terminal_01"
- [ ] **Required Quest ID**: "marketing_training_quest"
- [ ] **Game Controller**: Assigned MarketingGameController reference

#### Marketing Game Controller Setup:
- [ ] **Game Panel**: Initially SetActive(false)
- [ ] **Close Button**: Assigned in inspector
- [ ] **Close Button OnClick**: Set to MarketingGameController.CloseGame()
- [ ] **UI References**: All text fields and buttons assigned
- [ ] **Target Slots**: All 4 marketing slots assigned
- [ ] **Marketing Cards**: All 4 cards assigned

#### Quest Integration:
- [ ] **Quest ScriptableObject**: Created with correct IDs
- [ ] **NPC with quest**: Marketing Supervisor created
- [ ] **Quest objective**: UnlockTerminal type with terminal ID
- [ ] **Layer setup**: Terminal on "sign" layer for E-key detection

### 4. Testing the Complete Flow

**Scenario A: Complete Before Getting Quest**
1. **Approach terminal** → Press E, game panel opens
2. **Complete challenge** → Terminal saves completion, becomes inactive
3. **Talk to Marketing Supervisor** → Offers training quest
4. **Accept quest** → Quest immediately shows as completed (retroactive)
5. **Hand in quest** → Receive rewards

**Scenario B: Get Quest First**
1. **Talk to Marketing Supervisor** → Offers training quest
2. **Accept quest** → Quest appears in quest log as active
3. **Approach terminal** → Press E, game panel opens
4. **Complete challenge** → Quest objective completes automatically
5. **Hand in quest** → Receive rewards
6. **Approach terminal again** → Terminal is inactive (completed)

### 5. Close Button Troubleshooting

If the close button isn't working:

#### Check Inspector Settings:
- [ ] **Close Button** assigned in MarketingGameController
- [ ] **Button component** has OnClick event
- [ ] **OnClick target**: MarketingGameController
- [ ] **OnClick function**: CloseGame()

#### Manual Setup:
1. Select MarketingGameController in scene
2. In inspector, find "Close Button" field
3. Drag your close button from the hierarchy
4. Select the close button GameObject
5. In Button component, add OnClick event
6. Drag MarketingGameController to the event
7. Select MarketingGameController.CloseGame() from dropdown

#### Alternative Setup:
If still not working, you can set it up in the scene:
1. Select the Close Button in hierarchy
2. In Button component OnClick events
3. Click "+" to add new event
4. Drag MarketingGameController GameObject to the field
5. Select "MarketingGameController → CloseGame()" from dropdown

## Customization Options

### Difficulty Levels
- **Easy**: 3 cards, 3 slots
- **Medium**: 4 cards, 4 slots (default)
- **Hard**: 6 cards, 4 slots (multiple cards per audience)

### Additional Features
- **Timer**: Add countdown for extra challenge
- **Scoring**: Points based on speed and accuracy
- **Multiple Rounds**: Different card sets
- **Hints**: Show correct matches after failed attempts

### Card Categories
You can create themed card sets:
- **Product Launch**: Different product types → target markets
- **Seasonal Campaigns**: Holiday promotions → demographics
- **Digital Marketing**: Platform strategies → user groups
- **Local Business**: Community events → neighborhood segments

## Testing Checklist

### Functionality Tests
- [ ] Cards can be dragged smoothly
- [ ] Slots accept only correct cards
- [ ] Incorrect placements return cards to original position
- [ ] Visual feedback works (colors change appropriately)
- [ ] Score updates correctly
- [ ] Game completion triggers properly
- [ ] Reset button clears all cards and resets game
- [ ] Close button hides game UI
- [ ] Terminal interaction shows dialogue choices
- [ ] Dialogue system integration works

### UI Tests
- [ ] All text displays correctly
- [ ] Buttons are responsive
- [ ] Layout adapts to different screen sizes
- [ ] Cards don't overlap when dragging
- [ ] Visual feedback is clear and immediate

### Integration Tests
- [ ] Quest system recognizes completion (if implemented)
- [ ] Rewards are given properly (if implemented)
- [ ] Game can be replayed multiple times
- [ ] No errors in console during gameplay

## Troubleshooting

### Common Issues
1. **Cards won't drag**: Check if `MarketingDragHandler` is attached and `CanvasGroup` exists
2. **Slots won't accept cards**: Verify `acceptedAudience` strings match exactly
3. **Visual feedback not working**: Check if Image components are assigned in slots/cards
4. **Game doesn't complete**: Ensure all slots have `HasCorrectCard()` returning true

### Debug Tips
- Enable console logs in scripts to trace card placement
- Use Unity's Event System Debugger to check UI interactions
- Verify all component references are assigned in inspector
- Test with different screen resolutions

## Future Enhancements

### Potential Additions
- **Animation**: Smooth card movement and slot highlighting
- **Sound Effects**: Audio feedback for correct/incorrect placements
- **Particle Effects**: Celebration effects on completion
- **Leaderboard**: Track best scores and completion times
- **Achievements**: Unlock rewards for perfect scores or speed runs
- **Tutorial**: Step-by-step guide for first-time players

### Advanced Features
- **Procedural Generation**: Randomly generate card/slot combinations
- **Multiplayer**: Compete with other players
- **Analytics**: Track player performance and difficulty
- **Accessibility**: Screen reader support and colorblind-friendly design

## File Structure
```
Scripts/
├── MarketingGame/
│   ├── MarketingCard.cs
│   ├── MarketingDragHandler.cs
│   ├── MarketingSlot.cs
│   ├── MarketingGameController.cs
│   └── MarketingTerminal.cs
└── Documentation/
    └── MarketingGameSetup.md
```

This mini-game provides a solid foundation for business-themed gameplay while leveraging your existing systems for maximum code reuse and consistency.