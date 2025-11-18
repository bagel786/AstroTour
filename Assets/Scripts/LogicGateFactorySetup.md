# Logic Gate Factory Setup Guide

## Overview
The Logic Gate Factory is a puzzle mini-game where players route binary signals (0/1) through logic gates (XOR, AND, OR, NOT, SUM, NORM) to achieve a target output pattern. If gates are miswired, a "fat-tail event" can trigger a circuit reset.

## Components

### 1. LogicGate.cs
Represents individual logic gate pieces that can be dragged.
- **Gate Types**: XOR, AND, OR, NOT, SUM, NORM
- Provides visual feedback (green/red) when placed
- Tracks placement state

### 2. GateSlot.cs
Circuit slots where gates are placed.
- Accepts specific gate types
- Has input signals [0,1]
- Calculates output based on gate logic
- Validates against expected output

### 3. LogicGateDragHandler.cs
Handles drag-and-drop interaction for gates.
- Follows your existing drag handler pattern
- Snaps gates to valid slots
- Returns to original position if invalid

### 4. LogicGateController.cs
Main game controller managing the puzzle.
- Tracks score and completion
- Validates circuit configuration
- Triggers "fat-tail event" on wrong placement (optional)
- Integrates with quest system

### 5. LogicGateTerminal.cs
Terminal to start the game (implements IInteractable).
- Shows interaction prompt
- Launches game panel
- Tracks completion state

## Unity Setup

### Step 1: Create UI Canvas
1. Create a new Canvas in your scene
2. Set Canvas Scaler to "Scale With Screen Size"

### Step 2: Create Game Panel
1. Create a Panel under Canvas named "LogicGateGamePanel"
2. Add the following child objects:
   - **InstructionText** (TextMeshPro)
   - **ScoreText** (TextMeshPro)
   - **TargetPatternText** (TextMeshPro)
   - **CloseButton** (Button)
   - **ResetButton** (Button)
   - **CompletionPanel** (Panel, initially inactive)

### Step 3: Create Circuit Slots
1. Create a Panel named "CircuitSlotsContainer"
2. Add 4-6 empty GameObjects as children, each with:
   - **GateSlot.cs** component
   - **Image** component (slot background)
   - **TextMeshPro** for slot label
   - **TextMeshPro** for input display
   - **TextMeshPro** for output display

Configure each slot:
```
Slot 1: Input [0,1], Expected Output: 1, Accepts: ["XOR"]
Slot 2: Input [1,1], Expected Output: 1, Accepts: ["AND"]
Slot 3: Input [0,1], Expected Output: 1, Accepts: ["OR"]
Slot 4: Input [1,0], Expected Output: 0, Accepts: ["NOT"]
```

### Step 4: Create Logic Gates
1. Create a Panel named "AvailableGatesContainer"
2. Add 4-6 GameObjects as children, each with:
   - **LogicGate.cs** component
   - **LogicGateDragHandler.cs** component
   - **Image** component (gate visual)
   - **TextMeshPro** for gate label
   - **CanvasGroup** component

Configure gates:
```
Gate 1: Type "XOR", Display "XOR Gate"
Gate 2: Type "AND", Display "AND Gate"
Gate 3: Type "OR", Display "OR Gate"
Gate 4: Type "NOT", Display "NOT Gate"
Gate 5: Type "SUM", Display "Σ Aggregator"
Gate 6: Type "NORM", Display "Normalizer"
```

### Step 5: Setup Controller
1. Create empty GameObject "LogicGateController"
2. Add **LogicGateController.cs** component
3. Configure in Inspector:
   - Drag all GateSlots to `circuitSlots` array
   - Drag all LogicGates to `availableGates` array
   - Set `targetOutputPattern` (e.g., [1, 1, 1, 0])
   - Assign UI references
   - Set `allowFatTailReset` to true for hard mode

### Step 6: Create Terminal
1. Create a 3D object (Cube/Plane) in your scene
2. Add **LogicGateTerminal.cs** component
3. Add **BoxCollider** (set as trigger)
4. Assign `gameController` reference
5. Set quest IDs if using quest system

### Step 7: Configure Colors
Set visual feedback colors in Inspector:
- **Empty Color**: Dark gray (0.3, 0.3, 0.3)
- **Correct Color**: Green (0, 1, 0)
- **Incorrect Color**: Red (1, 0, 0)

## Logic Gate Behaviors

### AND Gate
- Output: 1 only if both inputs are 1
- Example: [1,1] → 1, [1,0] → 0

### OR Gate
- Output: 1 if either input is 1
- Example: [0,1] → 1, [0,0] → 0

### XOR Gate (Volatility)
- Output: 1 if inputs are different
- Example: [0,1] → 1, [1,1] → 0

### NOT Gate
- Output: Inverts first input
- Example: [1] → 0, [0] → 1

### SUM Gate (Aggregator)
- Output: Sum of inputs
- Example: [1,1] → 2, [0,1] → 1

### NORM Gate (Normalizer)
- Output: 1 if sum > 0, else 0
- Example: [1,1] → 1, [0,0] → 0

## Fat-Tail Event Feature
When `allowFatTailReset` is enabled:
- Any incorrect gate placement triggers a circuit reset
- Adds difficulty and risk/reward element
- Shows "FAT-TAIL EVENT!" message
- All gates return to starting positions

## Quest Integration
The terminal automatically updates quest objectives:
```csharp
questController.UpdateObjective("logic_gate_challenge", "complete_logic_gate_puzzle", 1);
```

## Testing
1. Play scene
2. Approach terminal and press E
3. Drag gates to slots
4. Verify correct placements turn green
5. Verify incorrect placements trigger reset (if enabled)
6. Complete puzzle to see completion screen

## Customization Ideas
- Add more complex gate types (NAND, NOR)
- Create multi-stage circuits
- Add time pressure
- Implement signal visualization (animated flow)
- Add difficulty levels with more slots
- Create "return curve" visualization graph
