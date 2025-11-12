# Fabric Mixer Mini-Game Setup Guide

## Overview
The Fabric Mixer allows players to experiment with combining different materials to create fabrics with specific properties. Unlike drag-and-drop sequencing games, this focuses on experimentation and property optimization through material combination.

## Required Scripts
- `FabricMaterial.cs` - Individual fabric materials with properties
- `FabricDragHandler.cs` - Handles drag and drop functionality with hover tooltips
- `FabricMixingSlot.cs` - Slots for combining materials (3 slots total)
- `FabricMixerController.cs` - Main game logic and challenge management
- `FabricMixerTerminal.cs` - Interactable object to start the game

## Core Gameplay Mechanics

### Property System
Each fabric has 5 properties (0-100 scale):
- **Flexibility**: How stretchy and movable the fabric is
- **Durability**: How long-lasting and resistant to wear
- **Waterproof**: How well it repels water and moisture
- **Breathability**: How well air flows through the fabric
- **Sustainability**: How eco-friendly the material is

### Mixing System
- **2-3 materials** can be combined per fabric
- **Properties blend** using weighted averages
- **Experimentation encouraged** - try different combinations
- **Real-time preview** shows current mix properties

## UI Structure Setup

### 1. Create Main Game Panel
```
FabricMixerPanel (Canvas/Panel)
├── Header
│   ├── InstructionText (TMP_Text) - Shows current challenge
│   ├── TargetPropertiesText (TMP_Text) - Shows required properties
│   └── ScoreText (TMP_Text) - Shows score and progress
├── MaterialsArea
│   ├── MaterialsContainer (Grid Layout Group)
│   │   ├── CottonMaterial (Image + TMP_Text + FabricMaterial + FabricDragHandler)
│   │   ├── PolyesterMaterial (Image + TMP_Text + FabricMaterial + FabricDragHandler)
│   │   ├── SilkMaterial (Image + TMP_Text + FabricMaterial + FabricDragHandler)
│   │   ├── WoolMaterial (Image + TMP_Text + FabricMaterial + FabricDragHandler)
│   │   ├── LinenMaterial (Image + TMP_Text + FabricMaterial + FabricDragHandler)
│   │   └── NylonMaterial (Image + TMP_Text + FabricMaterial + FabricDragHandler)
│   └── MaterialTooltip (Panel + TMP_Text) - Shows properties on hover
├── MixingArea
│   ├── MixingSlots (Horizontal Layout Group)
│   │   ├── PrimarySlot (Image + TMP_Text + FabricMixingSlot) - Index 0
│   │   ├── SecondarySlot (Image + TMP_Text + FabricMixingSlot) - Index 1
│   │   └── EnhancementSlot (Image + TMP_Text + FabricMixingSlot) - Index 2
│   └── CurrentResultText (TMP_Text) - Shows preview of current mix
├── ResultArea
│   ├── ResultPanel (Panel - toggleable)
│   │   ├── ResultFabricSwatch (Image) - Shows blended color
│   │   ├── PropertyBars (5 Sliders for each property)
│   │   └── ResultPropertiesText (TMP_Text)
│   └── ButtonsContainer
│       ├── MixButton (Button) - Creates the fabric
│       ├── ResetButton (Button) - Clears all slots
│       └── CloseButton (Button) - Exits the game
└── CompletionPanel (Panel - initially inactive)
    ├── SuccessText (TMP_Text)
    ├── ApplicationText (TMP_Text) - What the fabric is used for
    └── NextChallengeButton (Button)
```

## Material Configuration

### Base Materials
Configure each FabricMaterial component:

| Material | Flex | Dur | Water | Breath | Sustain | Color | Description |
|----------|------|-----|-------|--------|---------|-------|-------------|
| Cotton | 60 | 50 | 20 | 95 | 80 | Beige | Natural, breathable, eco-friendly |
| Polyester | 40 | 85 | 70 | 30 | 25 | Blue | Synthetic, durable, water-resistant |
| Silk | 90 | 40 | 10 | 85 | 60 | Gold | Luxurious, flexible, elegant |
| Wool | 50 | 70 | 60 | 70 | 75 | Gray | Warm, natural, good insulation |
| Linen | 45 | 60 | 15 | 90 | 85 | Light Green | Natural, breathable, sustainable |
| Nylon | 80 | 90 | 50 | 20 | 15 | Purple | Strong, flexible, synthetic |

### Enhancement Materials (Optional)
| Material | Flex | Dur | Water | Breath | Sustain | Color | Description |
|----------|------|-----|-------|--------|---------|-------|-------------|
| Waterproof Coating | 10 | 30 | 95 | 10 | 30 | Clear | Adds waterproofing |
| Stretch Treatment | 95 | 20 | 0 | 60 | 40 | White | Increases flexibility |
| Eco Treatment | 0 | 10 | 0 | 20 | 95 | Green | Boosts sustainability |

## Challenge Configuration

### Built-in Challenges
Configure in `FabricMixerController.availableChallenges`:

```csharp
// Challenge 1: Athletic Wear (Beginner)
challengeName: "Athletic Wear"
description: "Create fabric for sports clothing"
targetProperties: Flexibility(80), Durability(60), Waterproof(30), Breathability(90), Sustainability(50)
successMessage: "Perfect! This fabric is ideal for athletic wear."
applicationExample: "Used for: Yoga pants, running shirts, gym wear"
difficultyLevel: 1

// Challenge 2: Outdoor Jacket (Intermediate)  
challengeName: "Outdoor Jacket"
description: "Design fabric for weather protection"
targetProperties: Flexibility(40), Durability(85), Waterproof(95), Breathability(60), Sustainability(40)
successMessage: "Excellent! This fabric will keep adventurers dry and protected."
applicationExample: "Used for: Hiking jackets, rain coats, outdoor gear"
difficultyLevel: 2

// Challenge 3: Eco-Luxury Dress (Advanced)
challengeName: "Eco-Luxury Dress"
description: "Combine sustainability with elegance"
targetProperties: Flexibility(70), Durability(50), Waterproof(20), Breathability(80), Sustainability(95)
successMessage: "Outstanding! Sustainable luxury at its finest."
applicationExample: "Used for: Evening gowns, high-end fashion, eco-conscious luxury"
difficultyLevel: 3
```

### Custom Challenge Ideas
- **Baby Clothes**: High breathability + sustainability, low chemicals
- **Work Uniform**: High durability + moderate all other properties
- **Swimwear**: High flexibility + waterproof, low breathability needed
- **Winter Coat**: High durability + waterproof + warmth properties
- **Activewear**: High flexibility + breathability + moisture-wicking

## Mixing Slot Configuration

### Slot Setup
| Slot | Name | Index | Purpose | Description |
|------|------|-------|---------|-------------|
| Primary | "Base Material" | 0 | Main fabric | "The foundation of your fabric" |
| Secondary | "Blend Material" | 1 | Property modifier | "Adds specific characteristics" |
| Enhancement | "Treatment" | 2 | Special properties | "Special coatings or treatments" |

## Visual Styling

### Color Scheme
- **Natural Materials**: Earth tones (beige, green, brown)
- **Synthetic Materials**: Bright colors (blue, purple, pink)
- **Enhancement Materials**: Neutral/clear colors
- **Empty Slots**: Light gray (#E0E0E0)
- **Occupied Slots**: Blue (#2196F3)
- **Success Results**: Green (#4CAF50)
- **Failed Results**: Red (#F44336)

### Property Bar Colors
- **Flexibility**: Orange (#FF9800)
- **Durability**: Brown (#795548)
- **Waterproof**: Blue (#2196F3)
- **Breathability**: Light Blue (#03DAC6)
- **Sustainability**: Green (#4CAF50)

## Scene Setup

### 1. Create Game Controller
1. Create empty GameObject named "FabricMixerManager"
2. Add `FabricMixerController` script
3. Configure Fabric Challenges in inspector
4. Assign UI references:
   - Game Panel, Completion Panel, Result Panel
   - Instruction Text, Target Properties Text, Score Text
   - Current Result Text, Result Properties Text
   - Mix Button, Reset Button, Close Button
   - Mixing Slots array (ordered by index)
   - Available Materials array
   - Property Bars array (5 sliders)
   - Property Labels array (5 text components)

### 2. Create Material Objects
For each material:
1. Create UI Image with material icon/color
2. Add TMP_Text child for material name
3. Add `FabricMaterial` script with properties
4. Add `FabricDragHandler` script
5. Configure material properties and colors
6. Set up hover tooltip panel (optional)

### 3. Create Mixing Slots
For each slot:
1. Create UI Image for slot background
2. Add TMP_Text for slot label
3. Add `FabricMixingSlot` script
4. Configure slot index and name
5. Set up visual feedback colors

### 4. Create Interactable Terminal
1. Create GameObject with lab/computer sprite
2. Add `BoxCollider2D` set as trigger
3. Add `FabricMixerTerminal` script
4. Assign `FabricMixerController` reference
5. Configure terminal name and icon

## Integration with Existing Systems

### Quest-Driven Workflow
The fabric mixer terminal uses the quest-driven approach:

1. **NPC gives quest** (fashion designer assigns fabric research)
2. **Player approaches terminal** - checks for required quest
3. **No quest** = "Lab access restricted" message
4. **Has quest** = Terminal activates and starts challenge
5. **Completion** = Quest objective completed, terminal becomes inactive

### Quest Integration
```csharp
// Example quest for fabric challenges
{
    questID: "fabric_research_quest",
    questName: "Advanced Fabric Research",
    description: "Develop innovative fabric combinations for fashion applications",
    objectives: ["Complete fabric mixing research"],
    isCompleted: false
}
```

### Terminal Configuration
In FabricMixerTerminal inspector:
- **Terminal ID**: "fabric_mixer_01" (unique identifier)
- **Required Quest ID**: "fabric_research_quest" (quest needed to access)
- **Terminal Name**: Display name for messages

### Completion State Management
- **Persistent storage** using PlayerPrefs with terminal ID
- **Quest completion** automatically triggered on success
- **Terminal deactivation** prevents re-completion
- **Material interaction disabled** after completion

### NPC Quest Giver Example
```csharp
// Fashion Designer NPC dialogue
if (!QuestController.Instance.HasQuest("fabric_research_quest"))
{
    // Give quest
    DialogueController.Instance.CreateChoiceButton("Start Fabric Research", () => {
        QuestController.Instance.StartQuest("fabric_research_quest");
    });
}
else if (FabricMixerTerminal.IsCompleted())
{
    // Research completed
    DialogueController.Instance.SetDialogueText("Brilliant work! Your fabric innovations will revolutionize fashion.");
}
else
{
    // Quest active, research not done
    DialogueController.Instance.SetDialogueText("The fabric research lab is ready for your experiments.");
}
```

## Gameplay Flow

### 1. Challenge Phase
- Player reads challenge requirements
- Target properties are clearly displayed
- Player experiments with material combinations

### 2. Experimentation Phase
- Drag materials to mixing slots (2-3 materials)
- Real-time preview shows current properties
- Hover over materials to see their individual properties
- Mix button becomes available when 2+ materials placed

### 3. Testing Phase
- Player clicks "Mix" to create fabric
- System calculates combined properties
- Success: Shows result panel with property bars and application
- Failure: Shows what properties need improvement

### 4. Results & Progression
- Success leads to completion screen with rewards
- Player can try next challenge or experiment more
- Score accumulates based on accuracy and efficiency

## Unique Features

### Property Combination Logic
```csharp
// Simple average for 3 materials
FabricProperties result = new FabricProperties(
    (material1.flexibility + material2.flexibility + material3.flexibility) / 3,
    (material1.durability + material2.durability + material3.durability) / 3,
    // ... etc for all properties
);

// Weighted combination for 2 materials
FabricProperties result = FabricProperties.Combine(material1.properties, material2.properties, 0.6f);
```

### Experimentation Encouragement
- **No penalties** for failed attempts
- **Real-time feedback** shows current mix properties
- **Hover tooltips** explain material strengths
- **Multiple solutions** possible for each challenge

### Educational Value
- **Real fabric properties** teach material science
- **Practical applications** show real-world uses
- **Sustainability awareness** through eco-friendly options
- **Trade-offs** demonstrate engineering decisions

## Testing Checklist

### Core Functionality
- [ ] Materials drag smoothly to mixing slots
- [ ] Property calculations work correctly
- [ ] Real-time preview updates properly
- [ ] Mix button enables/disables appropriately
- [ ] Success/failure detection accurate
- [ ] Reset clears all materials properly
- [ ] Challenge progression works
- [ ] Score calculation correct

### UI/UX Testing
- [ ] Material tooltips show on hover
- [ ] Property bars display correctly
- [ ] Color coding is clear and consistent
- [ ] Text is readable on all backgrounds
- [ ] Layout works on different screen sizes
- [ ] Visual feedback is immediate
- [ ] Result panel displays properly

### Integration Testing
- [ ] Terminal interaction works smoothly
- [ ] Dialogue system integration functions
- [ ] Quest system recognizes completion
- [ ] Reward system triggers properly
- [ ] Game can be replayed multiple times
- [ ] No console errors during gameplay

## Troubleshooting

### Common Issues
1. **Materials won't drag**: Check `FabricDragHandler` and `CanvasGroup` components
2. **Properties not calculating**: Verify `FabricProperties.Combine()` methods
3. **Mix button stays disabled**: Check material count logic in `UpdateMixButtonState()`
4. **Tooltips not showing**: Ensure hover events are properly set up
5. **Visual feedback broken**: Check Image component assignments

### Debug Features
- Enable property calculation logging
- Add material placement debug messages
- Test with different material combinations
- Verify challenge requirement logic

## Future Enhancements

### Advanced Features
- **Custom Materials**: Let players create their own materials
- **Recipe Saving**: Save successful combinations
- **Advanced Properties**: Add texture, weight, cost properties
- **Market Simulation**: Show demand for different fabric types
- **Collaboration**: Share recipes with other players

### Educational Additions
- **Material Science**: Detailed explanations of fabric properties
- **Historical Context**: Learn about fabric development through history
- **Sustainability Impact**: Show environmental effects of choices
- **Industry Applications**: Connect to real fashion/textile industry

## File Structure
```
Scripts/
├── FabricMixer/
│   ├── FabricMaterial.cs
│   ├── FabricDragHandler.cs
│   ├── FabricMixingSlot.cs
│   ├── FabricMixerController.cs
│   └── FabricMixerTerminal.cs
└── Documentation/
    └── FabricMixerSetup.md
```

This system provides a unique experimentation-based gameplay experience that teaches material science and design thinking while integrating seamlessly with your existing game systems.