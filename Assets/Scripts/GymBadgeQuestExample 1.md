# Gym Badge Quest Example

## How to Set Up a Multi-Item Collection Quest

### Example: "Collect 8 Gym Badges"

Instead of creating 8 separate objectives, you can now create a single objective that accepts multiple item IDs:

**Quest Setup:**
- **Quest Name**: "Gym Badge Master"
- **Description**: "Collect 8 gym badges to prove your worth"

**Single Objective:**
- **Description**: "Collect gym badges"
- **Type**: CollectItem
- **Required Amount**: 8
- **Acceptable Item IDs**: [101, 102, 103, 104, 105, 106, 107, 108]
  - 101 = Boulder Badge
  - 102 = Cascade Badge  
  - 103 = Thunder Badge
  - 104 = Rainbow Badge
  - 105 = Soul Badge
  - 106 = Marsh Badge
  - 107 = Volcano Badge
  - 108 = Earth Badge

### How It Works:

1. **Player collects any gym badge** (e.g., Thunder Badge, ID 103)
2. **Quest progress updates** to 1/8 badges collected
3. **Player collects different badge** (e.g., Soul Badge, ID 105)  
4. **Quest progress updates** to 2/8 badges collected
5. **Continues until 8 badges collected** from any combination of the acceptable IDs

### Benefits:

- ✅ **Single objective** instead of 8 separate ones
- ✅ **Flexible collection** - any combination of badges works
- ✅ **Cleaner quest log** - shows "Collect gym badges (5/8)" instead of 8 lines
- ✅ **Easy to configure** - just add item IDs to the array
- ✅ **Backward compatible** - existing quests still work

### Alternative Use Cases:

- **"Collect 10 flowers"** - Rose (ID 201), Tulip (ID 202), Daisy (ID 203)
- **"Gather 5 gems"** - Ruby (ID 301), Sapphire (ID 302), Emerald (ID 303)
- **"Find 3 keys"** - Bronze Key (ID 401), Silver Key (ID 402), Gold Key (ID 403)