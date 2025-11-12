# DNA Sequencing Game - Quick Reference

## Component Summary

| Component | Purpose | Key Settings |
|-----------|---------|--------------|
| **SimpleDNASequencingController** | Main game logic | Target Sequence, UI references |
| **DNABase** | Individual DNA bases (A,T,G,C) | Base Type, Base Color |
| **DNASlot** | Target positions for bases | Expected Base, Visual colors |
| **DNASequenceFragment** | Container for multiple bases | Base Sequence string |
| **DNADragHandler** | Drag and drop functionality | Auto-attached to bases |

## Quick Setup Checklist

### 1. UI Setup (5 minutes)
- [ ] Create Canvas with DNA_Game_Panel
- [ ] Add 4 DNA slots with DNASlot script
- [ ] Add 4 DNA bases with DNABase + DNADragHandler scripts
- [ ] Add UI buttons (Close, Reset) and text (Instructions, Progress)

### 2. Controller Setup (2 minutes)
- [ ] Create empty GameObject with SimpleDNASequencingController
- [ ] Set Target Sequence (e.g., "ATCG")
- [ ] Assign UI references in inspector
- [ ] Assign DNA slots array in order

### 3. Base Configuration (3 minutes)
- [ ] Set each base type: A (Red), T (Blue), G (Green), C (Yellow)
- [ ] Ensure each base has CanvasGroup component
- [ ] Add Text child for base labels

### 4. Slot Configuration (2 minutes)
- [ ] Set expected base for each slot to match target sequence
- [ ] Configure colors: Empty (Gray), Correct (Green), Incorrect (Red)
- [ ] Add hint text children (optional)

## Default Color Scheme

| Base | Color | RGB |
|------|-------|-----|
| A (Adenine) | Red | (255, 0, 0) |
| T (Thymine) | Blue | (0, 0, 255) |
| G (Guanine) | Green | (0, 255, 0) |
| C (Cytosine) | Yellow | (255, 255, 0) |

| Slot State | Color | RGB |
|------------|-------|-----|
| Empty | Light Gray | (200, 200, 200) |
| Correct | Light Green | (144, 238, 144) |
| Incorrect | Light Red | (255, 182, 193) |

## Common Target Sequences

| Difficulty | Sequence | Description |
|------------|----------|-------------|
| Easy | "ATCG" | Basic 4-base sequence |
| Medium | "ATCGAT" | 6-base with repetition |
| Hard | "ATCGATCG" | 8-base palindrome |
| Expert | "ATCGTAGC" | 8-base complex |

## Integration Code Snippets

### Quest Completion:
```csharp
// In SimpleDNASequencingController.OnSequenceComplete()
if (QuestController.Instance != null)
{
    QuestController.Instance.CompleteObjective("dna_analysis_complete");
}
```

### Sound Effects:
```csharp
// In DNADragHandler.OnEndDrag()
SoundEffectManager.Play(targetSlot != null ? "dna_correct" : "dna_incorrect");
```

### Save Integration:
```csharp
// Add to SaveData
public bool dnaGameCompleted = false;

// In SaveController
saveData.dnaGameCompleted = dnaController.isCompleted;
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Bases won't drag | Add CanvasGroup component |
| Slots don't accept bases | Check Expected Base matches Base Type |
| UI doesn't scale | Set Canvas Scaler to "Scale With Screen Size" |
| No visual feedback | Verify Image components on slots |
| Game doesn't close | Check Game Panel reference in controller |

## Performance Tips

- Use object pooling for multiple DNA games
- Limit RaycastAll calls in drag handler
- Cache component references in Start()
- Use Canvas Groups for efficient UI hiding/showing

## Educational Extensions

- Show complementary DNA strand (A↔T, G↔C)
- Add timer for challenge mode
- Include DNA facts in instruction text
- Create multiple sequences for progression