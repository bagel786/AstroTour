# Logic Gate Factory - Quick Setup Guide

## Current State
You have the basic structure in place. Let's configure it properly.

## Step 1: Configure Gate Slots

For each GateSlot in your CircuitSlotsContainer, set:

### Slot 1 (Example: XOR Gate)
- **Slot Name**: "Volatility Gate"
- **Accepted Gate Types**: ["XOR"]
- **Input Signals**: [0, 1]
- **Expected Output**: 1 (because 0 XOR 1 = 1)

### Slot 2 (Example: AND Gate)
- **Slot Name**: "Correlation Gate"
- **Accepted Gate Types**: ["AND"]
- **Input Signals**: [1, 1]
- **Expected Output**: 1 (because 1 AND 1 = 1)

### Slot 3 (Example: OR Gate)
- **Slot Name**: "Aggregator"
- **Accepted Gate Types**: ["OR"]
- **Input Signals**: [0, 1]
- **Expected Output**: 1 (because 0 OR 1 = 1)

### Slot 4 (Example: NOT Gate)
- **Slot Name**: "Normalizer"
- **Accepted Gate Types**: ["NOT"]
- **Input Signals**: [1, 0]  (NOT only uses first input)
- **Expected Output**: 0 (because NOT 1 = 0)

## Step 2: Configure Available Gates

Create 4 LogicGate objects with these settings:

### Gate 1
- **Gate Type**: "XOR"
- **Gate Display Name**: "XOR Gate"
- **Color**: Yellow or custom

### Gate 2
- **Gate Type**: "AND"
- **Gate Display Name**: "AND Gate"
- **Color**: Blue or custom

### Gate 3
- **Gate Type**: "OR"
- **Gate Display Name**: "OR Gate"
- **Color**: Green or custom

### Gate 4
- **Gate Type**: "NOT"
- **Gate Display Name**: "NOT Gate"
- **Color**: Red or custom

## Step 3: Configure LogicGateController

In the Inspector:

1. **Circuit Slots**: Drag all 4 GateSlots into the array
2. **Available Gates**: Drag all 4 LogicGates into the array
3. **Target Output Pattern**: Set to `[1, 1, 1, 0]`
   - This matches the expected outputs from slots 1-4 above
4. **UI References**:
   - Game Panel: Drag the LogicGateGamePanel
   - Instruction Text: Drag the InstructionText
   - Score Text: Drag the ScoreText
   - Target Pattern Text: Drag the TargetPatternText
   - Close Button: Drag the CloseButton
5. **Game Settings**:
   - Points Per Correct Gate: 10
   - Feedback Display Time: 1.5
   - Allow Fat Tail Reset: true (enables auto-reset on wrong placement)

## Step 4: Verify UI References

Make sure these TextMeshPro components exist as children of LogicGateGamePanel:
- InstructionText (shows "Route signals through logic gates...")
- ScoreText (shows "Correct: 0/4")
- TargetPatternText (shows "Target: [1, 1, 1, 0]")

## Step 5: Test the Setup

1. Play the scene
2. Approach the terminal and press E
3. Game panel should open
4. Try dragging gates to slots:
   - **Correct placement**: Gate turns green, output matches target
   - **Incorrect placement**: Gate turns red, circuit resets (fat-tail event)
5. Complete all 4 slots to finish

## Troubleshooting

### Gates not dragging
- Check LogicGateDragHandler is on each gate
- Check CanvasGroup exists on each gate
- Verify gates are in the availableGates array

### Slots not accepting gates
- Verify Accepted Gate Types array is filled
- Check gate type matches exactly (case-sensitive)
- Ensure slot is empty before placing

### Output not calculating
- Check Input Signals array has correct values
- Verify Expected Output is set
- Check gate logic matches the inputs

### Fat-tail event not triggering
- Verify allowFatTailReset is true
- Check that incorrect gates are being placed
- Ensure SoundEffectManager has "logic_gate_error" sound

## Example Working Configuration

**Slots**: 4
**Gates**: 4
**Target Pattern**: [1, 1, 1, 0]

| Slot | Name | Input | Gate Type | Expected | Gate |
|------|------|-------|-----------|----------|------|
| 1 | Volatility | [0,1] | XOR | 1 | XOR Gate |
| 2 | Correlation | [1,1] | AND | 1 | AND Gate |
| 3 | Aggregator | [0,1] | OR | 1 | OR Gate |
| 4 | Normalizer | [1,0] | NOT | 0 | NOT Gate |

This creates a simple 4-slot puzzle that teaches all basic gate types.
