# Compound Synthesis Mini-Game Setup Guide

## Overview
The Compound Synthesis allows players to combine collected chemicals in specific ratios and sequences to create new serums, antidotes, and research compounds. Visual feedback through bubbles, color changes, and reaction animations signals progress or failure.

## Required Scripts
- `ChemicalCompound.cs` - Individual chemical ingredients with reaction properties
- `ChemicalDragHandler.cs` - Handles drag and drop with reaction tooltips
- `SynthesisSlot.cs` - Reaction vessels for combining compounds
- `CompoundSynthesisController.cs` - Main game logic with reaction validation
- `SynthesisTerminal.cs` - Interactable lab terminal

## Core Gameplay Mechanics

### Reaction-Based System
- **Chemical ratios** matter (1:2:1 vs 2:1:1 creates different compounds)
- **Sequence timing** affects reaction success
- **Temperature control** via slider affects reaction speed
- **Visual reactions** show bubbling, color changes, smoke effects

### Compound Creation
- **Base chemicals** provide foundation properties
- **Catalysts** speed up or enable reactions
- **Stabilizers** prevent dangerous reactions
- **Success indicators** through visual and audio feedback

### Risk Management
- **Unstable mixtures** can explode (reset required)
- **Incomplete reactions** produce weak compounds
- **Perfect synthesis** creates powerful research materials

## UI Structure Setup

### 1. Create Main Game Panel
```
CompoundSynthesisPanel (Canvas/Panel)
├── Header
│   ├── InstructionText (TMP_Text) - Shows current synthesis goal
│   ├── FormulaText (TMP_Text) - Shows required formula
│   └── ScoreText (TMP_Text) - Shows progress and success rate
├── ChemicalsArea
│   ├── ChemicalsContainer (Grid Layout Group)
│   │   ├── HydrogenCompound (Image + TMP_Text + ChemicalCompound + ChemicalDragHandler)
│   │   ├── OxygenCompound (Image + TMP_Text + ChemicalCompound + ChemicalDragHandler)
│   │   ├── CarbonCompound (Image + TMP_Text + ChemicalCompound + ChemicalDragHandler)
│   │   ├── NitrogenCompound (Image + TMP_Text + ChemicalCompound + ChemicalDragHandler)
│   │   ├── CatalystA (Image + TMP_Text + ChemicalCompound + ChemicalDragHandler)
│   │   └── StabilizerX (Image + TMP_Text + ChemicalCompound + ChemicalDragHandler)
│   └── ReactionTooltip (Panel + TMP_Text) - Shows compound properties
├── SynthesisArea
│   ├── ReactionVessel (Image) - Main synthesis container
│   ├── SynthesisSlots (Horizontal Layout Group)
│   │   ├── PrimarySlot (Image + TMP_Text + SynthesisSlot) - Index 0
│   │   ├── SecondarySlot (Image + TMP_Text + SynthesisSlot) - Index 1
│   │   └── CatalystSlot (Image + TMP_Text + SynthesisSlot) - Index 2
│   ├── TemperatureControl (Slider) - Controls reaction temperature
│   ├── TemperatureDisplay (TMP_Text) - Shows current temperature
│   └── ReactionProgress (Slider) - Shows synthesis progress
├── ResultArea
│   ├── ResultPanel (Panel - toggleable)
│   │   ├── ResultCompound (Image) - Shows synthesized compound
│   │   ├── PurityBar (Slider) - Shows compound purity
│   │   ├── StabilityBar (Slider) - Shows compound stability
│   │   ├── PotencyBar (Slider) - Shows compound effectiveness
│   │   └── CompoundAnalysis (TMP_Text) - Detailed results
│   └── ButtonsContainer
│       ├── SynthesizeButton (Button) - Starts the reaction
│       ├── ResetButton (Button) - Clears all compounds
│       └── CloseButton (Button) - Exits the game
└── CompletionPanel (Panel - initially inactive)
    ├── SuccessText (TMP_Text)
    ├── CompoundUseText (TMP_Text) - What the compound is used for
    └── NextFormulaButton (Button)
```

## Chemical Configuration

### Base Compounds
Configure each ChemicalCompound component:

| Compound | Type | Reactivity | Stability | Color | Reaction Temp | Description |
|----------|------|------------|-----------|-------|---------------|-------------|
| Hydrogen | Base | 90 | 30 | Light Blue | 25°C | Highly reactive gas |
| Oxygen | Base | 80 | 60 | Red | 30°C | Essential for combustion |
| Carbon | Base | 40 | 80 | Black | 50°C | Stable foundation element |
| Nitrogen | Base | 20 | 90 | Purple | 20°C | Inert stabilizing gas |
| Sulfur | Base | 70 | 50 | Yellow | 40°C | Reactive non-metal |
| Phosphorus | Base | 95 | 20 | White | 35°C | Highly reactive element |

### Catalysts & Modifiers
| Compound | Type | Effect | Stability | Color | Description |
|----------|------|--------|-----------|-------|-------------|
| Catalyst A | Catalyst | Speed +50% | 70 | Silver | Accelerates reactions |
| Catalyst B | Catalyst | Speed +30%, Purity +20% | 80 | Gold | Gentle acceleration |
| Stabilizer X | Stabilizer | Stability +40% | 95 | Clear | Prevents explosions |
| Purifier | Modifier | Purity +60% | 60 | White | Removes impurities |

## Synthesis Formulas

### Built-in Research Formulas
Configure in `CompoundSynthesisController.availableFormulas`:

```csharp
// Formula 1: Basic Water Synthesis (Beginner)
compoundName: "Pure Water"
description: "Synthesize H2O for laboratory use"
researchUse: "Base solvent for all experiments"
requiredCompounds: [
    { compoundName: "Hydrogen", requiredRatio: 2, slotIndex: 0 },
    { compoundName: "Oxygen", requiredRatio: 1, slotIndex: 1 }
]
optimalTemperature: 25
temperatureTolerance: 5
reactionTime: 3.0f
successMessage: "Perfect! You've created pure laboratory water."
difficultyLevel: 1

// Formula 2: Antioxidant Serum (Intermediate)
compoundName: "Antioxidant Serum"
description: "Create a powerful cellular protection compound"
researchUse: "Prevents cellular damage in experiments"
requiredCompounds: [
    { compoundName: "Carbon", requiredRatio: 6, slotIndex: 0 },
    { compoundName: "Hydrogen", requiredRatio: 8, slotIndex: 0 },
    { compoundName: "Oxygen", requiredRatio: 6, slotIndex: 1 },
    { compoundName: "Catalyst A", requiredRatio: 1, slotIndex: 2 }
]
optimalTemperature: 37
temperatureTolerance: 3
reactionTime: 5.0f
successMessage: "Excellent! This antioxidant serum is highly potent."
difficultyLevel: 2

// Formula 3: Neural Enhancement Compound (Expert)
compoundName: "Neural Enhancement Compound"
description: "Advanced nootropic for cognitive enhancement research"
researchUse: "Enhances neural pathway efficiency in test subjects"
requiredCompounds: [
    { compoundName: "Nitrogen", requiredRatio: 2, slotIndex: 0 },
    { compoundName: "Phosphorus", requiredRatio: 1, slotIndex: 0 },
    { compoundName: "Carbon", requiredRatio: 8, slotIndex: 1 },
    { compoundName: "Stabilizer X", requiredRatio: 1, slotIndex: 2 }
]
optimalTemperature: 42
temperatureTolerance: 1
reactionTime: 8.0f
successMessage: "Outstanding! This neural compound meets research standards."
difficultyLevel: 3
```

## Visual Reaction System

### Reaction Animations
- **Bubbling Effect**: Particle system with bubble sprites
- **Color Mixing**: Lerp between compound colors during synthesis
- **Temperature Glow**: Vessel glows based on temperature (blue=cold, red=hot)
- **Success Sparkles**: Particle burst on successful synthesis
- **Failure Smoke**: Dark smoke particles on failed reactions

### Progress Indicators
- **Reaction Progress Bar**: Fills during synthesis time
- **Temperature Gauge**: Visual thermometer with color coding
- **Purity Meter**: Shows compound cleanliness (0-100%)
- **Stability Indicator**: Warns of dangerous reactions

## Integration with Existing Systems

### Quest-Driven Workflow
The synthesis terminal follows your established pattern:

1. **NPC gives quest** (research director assigns compound development)
2. **Player approaches terminal** - checks for required quest
3. **No quest** = "Research clearance required" message
4. **Has quest** = Terminal activates with safety protocols
5. **Completion** = Quest objective completed, terminal becomes inactive

### Quest Integration
```csharp
// Example quest for compound synthesis
{
    questID: "compound_research_quest",
    questName: "Advanced Compound Synthesis",
    description: "Develop new research compounds using chemical synthesis",
    objectives: ["Complete compound synthesis research"],
    isCompleted: false
}
```

## Unique Features

### Temperature Control System
```csharp
// Temperature affects reaction success
float temperatureAccuracy = 1f - (Mathf.Abs(currentTemp - optimalTemp) / temperatureTolerance);
bool temperatureInRange = Mathf.Abs(currentTemp - optimalTemp) <= temperatureTolerance;

// Visual feedback for temperature
if (currentTemp > optimalTemp + temperatureTolerance)
    vesselGlow.color = Color.red; // Too hot
else if (currentTemp < optimalTemp - temperatureTolerance)
    vesselGlow.color = Color.blue; // Too cold
else
    vesselGlow.color = Color.green; // Perfect
```

### Real-Time Reaction Feedback
- **Immediate visual response** to compound placement
- **Temperature slider** affects reaction vessel appearance
- **Progress bar** shows synthesis advancement
- **Audio cues** for successful/failed reactions

### Research Applications
- **Practical uses** for each synthesized compound
- **Scientific terminology** and proper chemical principles
- **Safety protocols** emphasizing laboratory procedures
- **Research context** showing real-world applications

## Why This Fits Your Architecture

1. **Drag-and-Drop Pattern**: Uses your existing `DragHandler` and `Slot` system
2. **Quest Integration**: Follows your terminal quest workflow exactly
3. **Visual Feedback**: Builds on your medicine mixer's bar system
4. **Progressive Difficulty**: Matches your other mini-games' structure
5. **Educational Value**: Teaches chemistry like medicine mixer teaches pharmacology

This compound synthesis system would integrate seamlessly with your existing codebase while adding the complex visual feedback and experimentation elements you're looking for.