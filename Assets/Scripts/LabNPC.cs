using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LabNPC : MonoBehaviour, IInteractable
{
    protected DialogueController dialogueUI;

    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;

    [Header("UI Elements")]
    public Button closeButton;

    private int dialogueIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private bool rewardsProcessedForCurrentLine;

    protected static LabNPC currentActiveNPC;
    public static LabNPC CurrentActive => currentActiveNPC;
    private enum QuestState { NotStarted, InProgress, Completed };
    private QuestState questState = QuestState.NotStarted;
    private enum RewardState { NotGiven, Given };
    private RewardState rewardState = RewardState.NotGiven;

    public bool CanInteract() => currentActiveNPC == null;


    private void Awake()
    {
        // Ensure close button always calls the current active NPC
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() =>
            {
                if (currentActiveNPC != null)
                    currentActiveNPC.EndDialogue();
            });
        }
    }

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && currentActiveNPC != this))
            return;

        if (currentActiveNPC == this)
        {
            NextLine();
        }
        else if (currentActiveNPC == null)
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        if (currentActiveNPC != null) return;

        // Sync with quest and reward data
        SyncQuestState();
        SyncRewardState();
        
        // Determine starting dialogue index based on states
        // Priority order (quest states take precedence over item rewards):
        // 1. Quest rewards given (highest priority)
        // 2. Quest completed (but rewards not yet given)
        // 3. Quest in progress
        // 4. Item rewards given (only if no active quest)
        // 5. Default start (index 0)
        
        if (dialogueData.quest != null && 
            QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questId) && 
            dialogueData.quest.questRewardsGivenIndex >= 0)
        {
            // Quest rewards have been given - use questRewardsGivenIndex
            dialogueIndex = dialogueData.quest.questRewardsGivenIndex;
            Debug.Log($"LabNPC: Starting dialogue at questRewardsGivenIndex {dialogueIndex} for NPC '{dialogueData.npcName}' (quest rewards already given)");
        }
        else if (questState == QuestState.Completed)
        {
            // Quest completed but rewards not yet given
            dialogueIndex = dialogueData.questCompletedIndex;
            Debug.Log($"LabNPC: Starting dialogue at questCompletedIndex {dialogueIndex} for NPC '{dialogueData.npcName}' (quest completed, rewards not yet given)");
        }
        else if (questState == QuestState.InProgress)
        {
            // Quest in progress
            dialogueIndex = dialogueData.questInProgressIndex;
            Debug.Log($"LabNPC: Starting dialogue at questInProgressIndex {dialogueIndex} for NPC '{dialogueData.npcName}' (quest in progress)");
        }
        else if (rewardState == RewardState.Given && dialogueData.itemRewardsGivenIndex >= 0)
        {
            // Item rewards have been given (and no quest is active) - use itemRewardsGivenIndex
            dialogueIndex = dialogueData.itemRewardsGivenIndex;
            Debug.Log($"LabNPC: Starting dialogue at itemRewardsGivenIndex {dialogueIndex} for NPC '{dialogueData.npcName}' (item rewards already given, no active quest)");
        }
        else
        {
            // Quest not started and no rewards given
            dialogueIndex = 0;
            Debug.Log($"LabNPC: Starting dialogue at index 0 for NPC '{dialogueData.npcName}' (quest not started, no rewards given)");
        }
        
        currentActiveNPC = this;

        // Notify quest system about NPC interaction
        if (QuestController.Instance != null && dialogueData != null && !string.IsNullOrEmpty(dialogueData.npcName))
        {
            QuestController.Instance.CheckNPCInteraction(dialogueData.npcName);
        }

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);

        PauseController.SetPause(true);

        DisplayCurrentLine();
    }
    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;
        string questID = dialogueData.quest.questId;
        if(QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID)){
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }

    private void SyncRewardState()
    {
        // Check if any rewards from this NPC have been given
        if (DialogueRewardManager.Instance != null && 
            dialogueData.itemRewards != null && 
            dialogueData.itemRewards.Length > 0)
        {
            if (DialogueRewardManager.Instance.HasReceivedAnyRewardFromNPC(dialogueData.itemRewards))
            {
                rewardState = RewardState.Given;
                Debug.Log($"LabNPC: Reward state set to Given for NPC '{dialogueData.npcName}'");
            }
            else
            {
                rewardState = RewardState.NotGiven;
            }
        }
        else
        {
            rewardState = RewardState.NotGiven;
        }
    }

    public void NextLine()
    {
        if (currentActiveNPC != this) return;

        if (isTyping)
        {
            // Auto-complete current line
            StopCoroutine(typingCoroutine);
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            
            // Process rewards immediately when typing is skipped
            ProcessItemRewards();
            return; // Don't advance to next line yet, let player click again to advance
        }

        dialogueUI.ClearChoices();

        // Check for end of dialogue
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        // Display choices if any
        if (dialogueData.choices != null)
        {
            foreach (DialogueChoice dialogueChoice in dialogueData.choices)
            {
                if (dialogueChoice.dialogueIndex == dialogueIndex)
                {
                    DisplayChoices(dialogueChoice);
                    return;
                }
            }
        }

        // Advance to next line
        if (dialogueIndex < dialogueData.dialogueLines.Length - 1)
        {
            dialogueIndex++;
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");
        
        string fullText = dialogueData.dialogueLines[dialogueIndex];
        
        // Play voice audio based on preference
        if (dialogueData.voiceSound != null && !dialogueData.voicePerCharacter)
        {
            // Play once per line (more natural for longer dialogue)
            SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
        }

        // Type out each character
        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueUI.SetDialogueText(fullText.Substring(0, i + 1));
            
            // Play voice per character if enabled (classic style)
            if (dialogueData.voiceSound != null && dialogueData.voicePerCharacter)
            {
                SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
            }
            
            // Check for player input to skip typing (auto-completion for impatient players)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Complete the text immediately
                dialogueUI.SetDialogueText(fullText);
                break;
            }
            
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Process item rewards after typing animation completes
        ProcessItemRewards();

        // Auto-progress if flagged (for impatient players who want automatic progression)
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            
            // Double-check that player hasn't manually progressed already
            if (!isTyping && currentActiveNPC == this)
            {
                NextLine();
            }
        }
    }

    // private void DisplayChoices(DialogueChoice choice)
    // {
    //     for (int i = 0; i < choice.choices.Length; i++)
    //     {
    //         // Capture the index in a local variable to avoid closure issues
    //         int choiceIndex = i;
    //         int nextIndex = choice.nextDialogueIndexes[choiceIndex];
    //         Debug.Log(choiceIndex + " " + nextIndex);
            
    //         bool givesQuest = choice.givesQuest[i];
    //         dialogueUI.CreateChoiceButton(choice.choices[choiceIndex], () => ChooseOption(nextIndex, givesQuest));

    //         // Create the button with improved event handling 
    //         // dialogueUI.CreateChoiceButton(choice.choices[choiceIndex], () =>
    //         // {
    //         //     // Invoke the UnityEvent if it exists 
    //         //     if (choice.onChoiceEvents != null && 
    //         //         choice.onChoiceEvents.Length > choiceIndex && 
    //         //         choice.onChoiceEvents[choiceIndex] != null)
    //         //     {
    //         //         Debug.Log($"Invoking choice event for choice {choiceIndex}: {choice.choices[choiceIndex]}");
    //         //         choice.onChoiceEvents[choiceIndex].Invoke();
    //         //     }

    //         //     // Move to next dialogue index
    //         //     ChooseOption(nextIndex);
    //         // });

    //     }
    // }


    private void DisplayChoices(DialogueChoice choice)
    {
        if (choice == null || choice.choices == null) return;

        for (int i = 0; i < choice.choices.Length; i++)
        {
            int choiceIndex = i;

            int nextIndex = (choice.nextDialogueIndexes != null && choice.nextDialogueIndexes.Length > choiceIndex)
                ? choice.nextDialogueIndexes[choiceIndex]
                : dialogueIndex + 1;

            bool givesQuest = (choice.givesQuest != null && choice.givesQuest.Length > choiceIndex)
                ? choice.givesQuest[choiceIndex]
                : false;

            dialogueUI.CreateChoiceButton(choice.choices[choiceIndex], () =>
            {
                // Invoke any UnityEvent tied to this choice
                if (choice.onChoiceEvents != null &&
                    choice.onChoiceEvents.Length > choiceIndex &&
                    choice.onChoiceEvents[choiceIndex] != null)
                {
                    choice.onChoiceEvents[choiceIndex].Invoke();
                }

                // If this choice should teleport, perform the teleport but DO NOT end the dialogue.
                // Dialogue will continue to the next line; player can close it using the close button.
                bool shouldTeleport = choice.shouldTeleport != null &&
                                      choice.shouldTeleport.Length > choiceIndex &&
                                      choice.shouldTeleport[choiceIndex];

                if (shouldTeleport)
                {
                    // Perform teleport but keep dialogue active
                    choice.ExecuteTeleport(choiceIndex);
                }

                // Normal choice flow (quest awarding + advance)
                ChooseOption(nextIndex, givesQuest);
            });
        }
        
        // Adjust choice container position based on number of choices
        dialogueUI.AdjustChoiceContainerPosition(choice.choices.Length);
    }

    private void ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Reset reward processing flag for new line
        rewardsProcessedForCurrentLine = false;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {

        if (currentActiveNPC != this) return;

        if (isTyping && typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        if(questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questId)){
            // handle quest completion
            HandleQuestCompletion(dialogueData.quest);
        }
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);

        PauseController.SetPause(false);
        currentActiveNPC = null;
    }
    void HandleQuestCompletion(Quest quest){
        RewardsController.Instance.GivesQuestRewards(quest);
        QuestController.Instance.HandInQuest(quest.questId);
    }

    // Helper method for waypoint teleportation that can be called from UnityEvents
    public void TeleportToWaypoint(string waypointName)
    {
        Debug.Log($"Triggering teleport to waypoint: {waypointName}");
        
        if (WaypointTeleportManager.Instance != null)
        {
            EndDialogue(); // Close dialogue before teleporting
            WaypointTeleportManager.Instance.TeleportToWaypoint(waypointName);
        }
        else
        {
            Debug.LogError("WaypointTeleportManager not found in scene!");
        }
    }

    // Specific teleport methods for common areas

    // Another helper method for other common dialogue actions
    public void TriggerCustomAction(string actionName)
    {
        Debug.Log($"Triggering custom action: {actionName}");
        // Add your custom action logic here
    }

    public bool canInteract()
    {
        return true;
    }

    /// <summary>
    /// Processes item rewards for the current dialogue index
    /// </summary>
    private void ProcessItemRewards()
    {
        // Prevent duplicate processing for the same dialogue line
        if (rewardsProcessedForCurrentLine)
        {
            return;
        }

        if (dialogueData == null)
        {
            Debug.LogWarning("LabNPC: Cannot process item rewards - dialogueData is null");
            return;
        }

        if (dialogueData.itemRewards == null || dialogueData.itemRewards.Length == 0)
        {
            // This is normal - not all NPCs have item rewards
            return;
        }

        try
        {
            int rewardsProcessed = 0;
            foreach (DialogueItemReward reward in dialogueData.itemRewards)
            {
                if (reward == null)
                {
                    Debug.LogWarning($"LabNPC: Skipping null reward in itemRewards array for NPC '{dialogueData.npcName}'");
                    continue;
                }

                // Check if this reward should be given at the current dialogue index
                if (reward.dialogueIndex == dialogueIndex)
                {
                    try
                    {
                        GiveItemReward(reward);
                        rewardsProcessed++;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"LabNPC: Exception occurred while processing reward '{reward.uniqueRewardID}' at dialogue index {dialogueIndex}: {ex.Message}");
                        // Continue processing other rewards even if one fails
                    }
                }
            }

            if (rewardsProcessed > 0)
            {
                Debug.Log($"LabNPC: Processed {rewardsProcessed} reward(s) at dialogue index {dialogueIndex} after typing completed");
            }

            // Mark rewards as processed for this line
            rewardsProcessedForCurrentLine = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LabNPC: Critical exception occurred while processing item rewards: {ex.Message}");
            // Continue dialogue flow even if reward processing fails completely
        }
    }

    /// <summary>
    /// Gives an item reward to the player, handling inventory management and tracking
    /// </summary>
    /// <param name="reward">The reward to give</param>
    private void GiveItemReward(DialogueItemReward reward)
    {
        // Check if DialogueRewardManager is available
        if (DialogueRewardManager.Instance == null)
        {
            Debug.LogError("LabNPC: DialogueRewardManager instance not found. Cannot process reward.");
            return;
        }

        // Use the enhanced validation and reward creation from DialogueRewardManager
        bool rewardSuccess = DialogueRewardManager.Instance.TryGiveReward(
            reward,
            onSuccess: (itemInstance) => {
                // Successfully created the reward item, now handle inventory management
                HandleRewardInventoryManagement(itemInstance, reward);
                
                // Update reward state since we just gave a reward
                SyncRewardState();
            },
            onFailure: (errorMessage) => {
                // Log the failure and continue dialogue flow gracefully
                Debug.LogWarning($"LabNPC: Failed to give reward at dialogue index {dialogueIndex}: {errorMessage}");
                
                // Optionally, you could show a message to the player here
                // For now, we'll just continue the dialogue flow without the reward
            }
        );

        if (!rewardSuccess)
        {
            Debug.LogWarning($"LabNPC: Reward processing failed for dialogue index {dialogueIndex}. Continuing dialogue without reward.");
        }
    }

    /// <summary>
    /// Handles adding the reward item to inventory or dropping it in the world
    /// </summary>
    /// <param name="itemInstance">The created item instance</param>
    /// <param name="reward">The original reward data for logging</param>
    private void HandleRewardInventoryManagement(GameObject itemInstance, DialogueItemReward reward)
    {
        if (itemInstance == null)
        {
            Debug.LogError("LabNPC: Cannot handle inventory management for null item instance");
            return;
        }

        // Try to add to inventory
        bool wasAddedToInventory = false;
        if (InventoryController.Instance != null)
        {
            try
            {
                wasAddedToInventory = InventoryController.Instance.AddItem(itemInstance, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LabNPC: Exception occurred while adding item to inventory: {ex.Message}");
                wasAddedToInventory = false;
            }
        }
        else
        {
            Debug.LogError("LabNPC: InventoryController instance not found. Cannot add item to inventory.");
        }

        // Handle the result
        if (wasAddedToInventory)
        {
            // Successfully added to inventory - destroy the original instance since InventoryController creates its own copy
            Item itemComponent = itemInstance.GetComponent<Item>();
            string itemName = itemComponent?.itemName ?? $"Item ID {reward.itemID}";
            Debug.Log($"LabNPC: Successfully added '{itemName}' (x{reward.quantity}) to player inventory");
            
            // Destroy the original instance to prevent duplication (InventoryController.AddItem creates its own copy)
            Destroy(itemInstance);
        }
        else
        {
            // Couldn't add to inventory, drop in world near player
            try
            {
                DropItemInWorld(itemInstance);
                Item itemComponent = itemInstance.GetComponent<Item>();
                string itemName = itemComponent?.itemName ?? $"Item ID {reward.itemID}";
                Debug.Log($"LabNPC: Inventory full, dropped '{itemName}' (x{reward.quantity}) in world near player");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LabNPC: Exception occurred while dropping item in world: {ex.Message}");
                // As a last resort, destroy the item to prevent memory leaks
                if (itemInstance != null)
                {
                    Destroy(itemInstance);
                    Debug.LogWarning("LabNPC: Destroyed item instance due to handling failure");
                }
            }
        }
    }

    /// <summary>
    /// Drops an item in the world near the player when inventory is full
    /// </summary>
    /// <param name="itemInstance">The item instance to drop</param>
    private void DropItemInWorld(GameObject itemInstance)
    {
        if (itemInstance == null)
        {
            Debug.LogError("LabNPC: Cannot drop null item instance in world");
            return;
        }

        try
        {
            // Find player position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 dropPosition;

            if (player != null)
            {
                // Drop item near player with slight random offset
                Vector3 playerPos = player.transform.position;
                Vector2 randomOffset = Random.insideUnitCircle * 2f; // Random position within 2 units
                dropPosition = new Vector3(playerPos.x + randomOffset.x, playerPos.y + randomOffset.y, playerPos.z);
            }
            else
            {
                // Fallback: drop at NPC position
                Debug.LogWarning("LabNPC: Player not found, dropping item near NPC instead");
                dropPosition = transform.position + Vector3.right * 1.5f;
            }

            // Position the item in the world
            itemInstance.transform.position = dropPosition;

            // Ensure item has proper components for world interaction
            Collider2D collider = itemInstance.GetComponent<Collider2D>();
            if (collider == null)
            {
                collider = itemInstance.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                Debug.Log("LabNPC: Added BoxCollider2D to dropped item");
            }

            // Ensure item has the "Item" tag for pickup detection
            if (!itemInstance.CompareTag("Item"))
            {
                itemInstance.tag = "Item";
                Debug.Log("LabNPC: Set 'Item' tag on dropped item");
            }

            // Enable SpriteRenderer for world display and set size to 100x100 pixels
            SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                
                // Set the sprite size to 100x100 pixels
                if (spriteRenderer.sprite != null)
                {
                    // Calculate the scale needed to make the sprite 100x100 pixels
                    float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
                    float targetSizeInUnits = 100f / pixelsPerUnit;
                    
                    // Get the original sprite size in pixels
                    Vector2 spriteSize = spriteRenderer.sprite.rect.size;
                    
                    // Calculate scale factors for each axis
                    float scaleX = targetSizeInUnits / (spriteSize.x / pixelsPerUnit);
                    float scaleY = targetSizeInUnits / (spriteSize.y / pixelsPerUnit);
                    
                    // Apply the scale to make it 100x100 pixels
                    itemInstance.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                }
            }
            else
            {
                Debug.LogWarning("LabNPC: Dropped item does not have a SpriteRenderer component - may not be visible in world");
            }

            // Validate the item component
            Item itemComponent = itemInstance.GetComponent<Item>();
            if (itemComponent == null)
            {
                Debug.LogWarning("LabNPC: Dropped item does not have an Item component - may not function correctly");
            }

            Debug.Log($"LabNPC: Successfully dropped item '{itemComponent?.itemName ?? "Unknown"}' at position {dropPosition}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LabNPC: Exception occurred while dropping item in world: {ex.Message}");
            
            // Try to position the item at a safe fallback location
            try
            {
                itemInstance.transform.position = transform.position;
                Debug.LogWarning("LabNPC: Placed item at NPC position as emergency fallback");
            }
            catch (System.Exception fallbackEx)
            {
                Debug.LogError($"LabNPC: Even fallback positioning failed: {fallbackEx.Message}");
                // As a last resort, destroy the item to prevent issues
                if (itemInstance != null)
                {
                    Destroy(itemInstance);
                    Debug.LogError("LabNPC: Destroyed item instance due to critical drop failure");
                }
            }
        }
    }
}