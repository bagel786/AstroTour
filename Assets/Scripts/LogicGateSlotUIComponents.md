# Logic Gate Factory - Slot UI Components

## SLOT 1 Example

### Slot Label (TextMeshPro)
- **Initial Text**: `Volatility Gate`
- **Font Size**: 24-32
- **Color**: White
- **Purpose**: Shows the slot name to the player
- **Auto-Updated**: No (set once in inspector)

### Input Display (TextMeshPro)
- **Initial Text**: `In: 0,1`
- **Font Size**: 18-20
- **Color**: Cyan or Light Blue
- **Purpose**: Shows what binary inputs this slot receives
- **Auto-Updated**: Yes (updates from Input Signals array)
- **Examples**:
  - Slot 1: `In: 0,1` (two inputs)
  - Slot 4: `In: 1` (one input for NOT gate)

### Output Display (TextMeshPro)
- **Initial Text**: `Out: ?`
- **Font Size**: 18-20
- **Color**: Yellow or Orange
- **Purpose**: Shows the output once a gate is placed
- **Auto-Updated**: Yes (updates when gate is placed)
- **Examples**:
  - Before gate placed: `Out: ?`
  - After correct gate: `Out: 1`
  - After incorrect gate: `Out: 0` (wrong value)

### Slot Background (Image)
- **Initial Color**: Dark Gray `(0.3, 0.3, 0.3, 1)`
- **Size**: Fills the slot area
- **Purpose**: Visual feedback for slot state
- **Color Changes**:
  - **Empty**: Dark Gray `(0.3, 0.3, 0.3, 1)` - no gate placed
  - **Correct**: Green `(0, 1, 0, 1)` - correct gate placed
  - **Incorrect**: Red `(1, 0, 0, 1)` - wrong gate placed

---

## Visual Layout Example (Slot 1)

```
┌─────────────────────────────┐
│  Volatility Gate            │  ← Slot Label
│                             │
│  In: 0,1        Out: ?      │  ← Input Display & Output Display
│                             │
│     [XOR GATE AREA]         │  ← Where gate gets placed
│                             │
└─────────────────────────────┘
     ↑ Background Color
     Changes based on state
```

---

## All 4 Slots - Complete UI Setup

### SLOT 1
- **Slot Label**: `Volatility Gate`
- **Input Display**: `In: 0,1`
- **Output Display**: `Out: ?` → `Out: 1` (when XOR placed)
- **Background**: Gray → Green (when correct)

### SLOT 2
- **Slot Label**: `Correlation Gate`
- **Input Display**: `In: 1,1`
- **Output Display**: `Out: ?` → `Out: 1` (when AND placed)
- **Background**: Gray → Green (when correct)

### SLOT 3
- **Slot Label**: `Aggregator`
- **Input Display**: `In: 0,1`
- **Output Display**: `Out: ?` → `Out: 1` (when OR placed)
- **Background**: Gray → Green (when correct)

### SLOT 4
- **Slot Label**: `Normalizer`
- **Input Display**: `In: 1`
- **Output Display**: `Out: ?` → `Out: 0` (when NOT placed)
- **Background**: Gray → Green (when correct)

---

## Component Setup in Unity

For each slot, create this hierarchy:

```
CircuitSlot1 (Panel with Image for background)
├── SlotLabel (TextMeshPro)
├── InputDisplay (TextMeshPro)
├── OutputDisplay (TextMeshPro)
└── [Gate gets placed here when dragged]
```

### CircuitSlot1 (Parent Panel)
- Component: **Image** (this is the background)
- Image Source: White square or solid color
- Color: `(0.3, 0.3, 0.3, 1)` - Dark Gray
- Size: 200x150 (or whatever fits your layout)

### SlotLabel (Child TextMeshPro)
- Text: `Volatility Gate`
- Font Size: 28
- Alignment: Top Center
- Color: White `(1, 1, 1, 1)`
- Position: Top of slot

### InputDisplay (Child TextMeshPro)
- Text: `In: 0,1`
- Font Size: 20
- Alignment: Bottom Left
- Color: Cyan `(0, 1, 1, 1)`
- Position: Bottom left of slot

### OutputDisplay (Child TextMeshPro)
- Text: `Out: ?`
- Font Size: 20
- Alignment: Bottom Right
- Color: Yellow `(1, 1, 0, 1)`
- Position: Bottom right of slot

---

## Color Reference

### Background Colors
- **Empty (Gray)**: R: 0.3, G: 0.3, B: 0.3, A: 1.0
- **Correct (Green)**: R: 0, G: 1, B: 0, A: 1.0
- **Incorrect (Red)**: R: 1, G: 0, B: 0, A: 1.0

### Text Colors
- **Slot Label (White)**: R: 1, G: 1, B: 1, A: 1.0
- **Input Display (Cyan)**: R: 0, G: 1, B: 1, A: 1.0
- **Output Display (Yellow)**: R: 1, G: 1, B: 0, A: 1.0

---

## GateSlot.cs Component Settings

In the Inspector for CircuitSlot1, assign:

- **Slot Label**: Drag the SlotLabel TextMeshPro
- **Input Display**: Drag the InputDisplay TextMeshPro
- **Output Display**: Drag the OutputDisplay TextMeshPro
- **Slot Background**: Drag the Image component (the parent panel)

Then set the colors in the Visual Feedback section:
- **Empty Color**: `(0.3, 0.3, 0.3, 1)`
- **Correct Color**: `(0, 1, 0, 1)`
- **Incorrect Color**: `(1, 0, 0, 1)`
