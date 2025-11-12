using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueItemReward
{
    public int itemID;                    // Reference to item in ItemDictionary
    public int quantity = 1;              // Amount to give
    public int dialogueIndex;             // Which dialogue line triggers this reward
    public bool canGiveMultipleTimes;     // Whether item can be given repeatedly
    public string uniqueRewardID;        // Unique identifier for tracking
}

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;

    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    
    [Header("Voice Settings")]
    public AudioClip voiceSound;           // Sound to play for voice
    public float voicePitch = 1.0f;        // Pitch of the voice
    public bool voicePerCharacter = false; // Play voice per character or per line

    public DialogueChoice[] choices;
    public int questInProgressIndex;
    public int questCompletedIndex;
    public Quest quest;
    
    [Header("Item Rewards")]
    public DialogueItemReward[] itemRewards;     // Items to give at specific dialogue indices
    [Tooltip("Dialogue index to use after any item rewards have been given. Set to -1 to disable. Similar to questCompletedIndex but for item rewards.")]
    public int itemRewardsGivenIndex = -1;       // Dialogue index to use after any item rewards have been given
    // ==========================
    // Registry-based Waypoint Access
    // ==========================

    /// <summary>
    /// Get all available waypoints from the registry (useful for editor tools)
    /// </summary>
    public static string[] GetAvailableWaypoints()
    {
        var waypoints = WaypointTeleportManager.GetAllWaypointPositions();
        var names = new string[waypoints.Count];
        int i = 0;
        foreach (var kvp in waypoints)
        {
            names[i++] = kvp.Key;
        }
        return names;
    }
    
    /// <summary>
    /// Check if a waypoint exists in the registry
    /// </summary>
    public static bool IsWaypointValid(string waypointName)
    {
        return WaypointTeleportManager.HasWaypoint(waypointName);
    }
    
    /// <summary>
    /// Get the position of a waypoint (useful for preview in editor)
    /// </summary>
    public static Vector3? GetWaypointPreviewPosition(string waypointName)
    {
        return WaypointTeleportManager.GetWaypointPosition(waypointName);
    }
    
    /// <summary>
    /// Validates all item rewards in this dialogue for configuration errors
    /// </summary>
    /// <returns>True if all rewards are valid, false if any issues are found</returns>
    public bool ValidateItemRewards()
    {
        if (itemRewards == null || itemRewards.Length == 0)
        {
            return true; // No rewards to validate
        }

        bool allValid = true;
        
        for (int i = 0; i < itemRewards.Length; i++)
        {
            DialogueItemReward reward = itemRewards[i];
            
            if (reward == null)
            {
                Debug.LogError($"NPCDialogue '{name}': Item reward at index {i} is null");
                allValid = false;
                continue;
            }
            
            // Validate reward ID
            if (string.IsNullOrEmpty(reward.uniqueRewardID))
            {
                Debug.LogError($"NPCDialogue '{name}': Item reward at index {i} has null or empty uniqueRewardID");
                allValid = false;
            }
            
            // Validate item ID
            if (reward.itemID <= 0)
            {
                Debug.LogError($"NPCDialogue '{name}': Item reward '{reward.uniqueRewardID}' has invalid itemID '{reward.itemID}'");
                allValid = false;
            }
            
            // Validate quantity
            if (reward.quantity <= 0)
            {
                Debug.LogError($"NPCDialogue '{name}': Item reward '{reward.uniqueRewardID}' has invalid quantity '{reward.quantity}'");
                allValid = false;
            }
            
            // Validate dialogue index
            if (reward.dialogueIndex < 0)
            {
                Debug.LogError($"NPCDialogue '{name}': Item reward '{reward.uniqueRewardID}' has invalid dialogueIndex '{reward.dialogueIndex}'");
                allValid = false;
            }
            else if (dialogueLines != null && reward.dialogueIndex >= dialogueLines.Length)
            {
                Debug.LogWarning($"NPCDialogue '{name}': Item reward '{reward.uniqueRewardID}' has dialogueIndex '{reward.dialogueIndex}' which is beyond the dialogue lines array length ({dialogueLines.Length})");
            }
            
            // Check for duplicate reward IDs
            for (int j = i + 1; j < itemRewards.Length; j++)
            {
                if (itemRewards[j] != null && 
                    !string.IsNullOrEmpty(reward.uniqueRewardID) && 
                    reward.uniqueRewardID == itemRewards[j].uniqueRewardID)
                {
                    Debug.LogError($"NPCDialogue '{name}': Duplicate uniqueRewardID '{reward.uniqueRewardID}' found at indices {i} and {j}");
                    allValid = false;
                }
            }
        }
        
        // Validate itemRewardsGivenIndex if it's set
        if (itemRewardsGivenIndex >= 0)
        {
            if (dialogueLines != null && itemRewardsGivenIndex >= dialogueLines.Length)
            {
                Debug.LogError($"NPCDialogue '{name}': itemRewardsGivenIndex '{itemRewardsGivenIndex}' is beyond the dialogue lines array length ({dialogueLines.Length})");
                allValid = false;
            }
        }

        if (allValid)
        {
            Debug.Log($"NPCDialogue '{name}': All {itemRewards.Length} item rewards validated successfully");
        }
        
        return allValid;
    }
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;               // Where this choice appears in the dialogue
    public string[] choices;                // Text shown on buttons
    public int[] nextDialogueIndexes;       // Corresponding dialogue indexes to follow
    public UnityEvent[] onChoiceEvents;     // Optional: invoked when each choice is clicked
    
    [Header("Teleportation Options")]
    public bool[] shouldTeleport;           // Whether each choice should teleport
    public string[] waypointNames;          // Waypoint names to teleport to
    public bool[] givesQuest;            // Whether each choice gives a quest
    // Method to execute teleportation using the registry
    public void ExecuteTeleport(int choiceIndex)
    {
        if (shouldTeleport != null && 
            choiceIndex < shouldTeleport.Length && 
            shouldTeleport[choiceIndex])
        {
            if (waypointNames != null && choiceIndex < waypointNames.Length)
            {
                string waypointName = waypointNames[choiceIndex];
                if (!string.IsNullOrEmpty(waypointName))
                {
                    // This now uses the static registry method
                    WaypointTeleportManager.TeleportToRegisteredWaypoint(waypointName);
                }
            }
        }
    }
    
    // Validation method for editor
    public bool ValidateWaypoints()
    {
        if (waypointNames == null) return true;
        
        for (int i = 0; i < waypointNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(waypointNames[i]) && 
                !WaypointTeleportManager.HasWaypoint(waypointNames[i]))
            {
                Debug.LogWarning($"Waypoint '{waypointNames[i]}' not found in registry!");
                return false;
            }
        }
        return true;
    }
}