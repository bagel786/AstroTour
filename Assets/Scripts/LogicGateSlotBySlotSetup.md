# Logic Gate Factory - Slot by Slot Configuration

## SLOT 1 (CircuitSlot1)

### Slot Configuration Section
- **Slot Name**: `Volatility Gate`
- **Accepted Gate Types**: Array size = 1
  - Element 0: `XOR`

### Input Signals Section
- **Input Signals**: Array size = 2
  - Element 0: `0`
  - Element 1: `1`

### Expected Output
- **Expected Output**: `1`

### Why these values?
- XOR gate with inputs [0, 1] produces output 1 (because inputs are different)
- Player must drag the XOR gate here to get the correct output

---

## SLOT 2 (CircuitSlot2)

### Slot Configuration Section
- **Slot Name**: `Correlation Gate`
- **Accepted Gate Types**: Array size = 1
  - Element 0: `AND`

### Input Signals Section
- **Input Signals**: Array size = 2
  - Element 0: `1`
  - Element 1: `1`

### Expected Output
- **Expected Output**: `1`

### Why these values?
- AND gate with inputs [1, 1] produces output 1 (both inputs are 1)
- Player must drag the AND gate here to get the correct output

---

## SLOT 3 (CircuitSlot3)

### Slot Configuration Section
- **Slot Name**: `Aggregator`
- **Accepted Gate Types**: Array size = 1
  - Element 0: `OR`

### Input Signals Section
- **Input Signals**: Array size = 2
  - Element 0: `0`
  - Element 1: `1`

### Expected Output
- **Expected Output**: `1`

### Why these values?
- OR gate with inputs [0, 1] produces output 1 (at least one input is 1)
- Player must drag the OR gate here to get the correct output

---

## SLOT 4 (CircuitSlot4)

### Slot Configuration Section
- **Slot Name**: `Normalizer`
- **Accepted Gate Types**: Array size = 1
  - Element 0: `NOT`

### Input Signals Section
- **Input Signals**: Array size = 1
  - Element 0: `1`

### Expected Output
- **Expected Output**: `0`

### Why these values?
- NOT gate with input [1] produces output 0 (inverts the input)
- Player must drag the NOT gate here to get the correct output

---

## UI References (for each slot)

For each slot, you should have these UI components as children:

### Slot Label
- Component: TextMeshPro
- Text: (will be set from Slot Name field)
- Purpose: Shows "Volatility Gate", "Correlation Gate", etc.

### Input Display
- Component: TextMeshPro
- Text: (auto-generated from Input Signals)
- Purpose: Shows "In: 0,1" or "In: 1"

### Output Display
- Component: TextMeshPro
- Text: (auto-generated when gate is placed)
- Purpose: Shows "Out: 1" or "Out: ?"

### Slot Background
- Component: Image
- Purpose: Shows color feedback (gray=empty, green=correct, red=incorrect)

---

## Summary Table

| Slot | Name | Input | Gate Type | Expected Output |
|------|------|-------|-----------|-----------------|
| 1 | Volatility Gate | [0, 1] | XOR | 1 |
| 2 | Correlation Gate | [1, 1] | AND | 1 |
| 3 | Aggregator | [0, 1] | OR | 1 |
| 4 | Normalizer | [1] | NOT | 0 |

---

## After Setting Up All Slots

Once all 4 slots are configured, go to **LogicGateController** and set:
- **Target Output Pattern**: Array size = 4
  - Element 0: `1`
  - Element 1: `1`
  - Element 2: `1`
  - Element 3: `0`

This matches the expected outputs from all 4 slots.
