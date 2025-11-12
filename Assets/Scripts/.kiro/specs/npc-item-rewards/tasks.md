# Implementation Plan

- [x] 1. Create DialogueRewardManager singleton componen
  - Implement singleton pattern with Instance property
  - Create List<string> to track given reward IDs
  - Add methods for reward validation and tracking
  - Add methods to interface with save system (LoadGivenRewards, GetGivenRewards)
  - _Requirements: 2.3, 2.4_

- [x] 2. Extend SaveData and SaveController for reward tracking
  - Add givenDialogueRewardIDs list to SaveData class
  - Modify SaveController to save/load dialogue reward data
  - Integrate DialogueRewardManager with save system
  - _Requirements: 2.3, 2.4_

- [x] 3. Create DialogueItemReward serializable class and extend NPCDialogue
  - Define DialogueItemReward class with itemID, quantity, dialogueIndex, canGiveMultipleTimes, and uniqueRewardID fields
  - Add itemRewards array to NPCDialogue ScriptableObject
  - _Requirements: 2.1, 2.2_

- [x] 4. Implement reward logic in LabNPC
  - Modify DisplayCurrentLine method to check for rewards at current dialogue index
  - Add method to process item rewards using existing InventoryController.AddItem
  - Integrate with DialogueRewardManager for tracking
  - Handle inventory full scenarios by dropping items in world
  - _Requirements: 1.1, 1.3, 1.4, 3.1, 3.2, 3.3_

- [x] 5. Add reward validation and error handling
  - Validate itemID exists in ItemDictionary before giving rewards
  - Add logging for invalid item references
  - Ensure graceful handling when rewards fail
  - _Requirements: 1.4, 4.1, 4.2, 4.3, 4.4_

- [ ]* 6. Create unit tests for DialogueRewardManager
  - Test reward tracking functionality
  - Test save/load integration
  - Test duplicate reward prevention
  - _Requirements: 2.3, 2.4_