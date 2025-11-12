using System.Collections.Generic;
using UnityEngine;

public class DialogueRewardManager : MonoBehaviour
{
    public static DialogueRewardManager Instance { get; private set; }
    
    // Track which rewards have been given
    private List<string> givenRewards = new List<string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Checks if a reward can be given based on tracking rules
    /// </summary>
    /// <param name="rewardID">Unique identifier for the reward</param>
    /// <param name="canGiveMultipleTimes">Whether this reward can be given multiple times</param>
    /// <returns>True if the reward can be given, false otherwise</returns>
    public bool CanGiveReward(string rewardID, bool canGiveMultipleTimes)
    {
        if (string.IsNullOrEmpty(rewardID))
        {
            Debug.LogWarning("DialogueRewardManager: Cannot validate reward with null or empty rewardID");
            return false;
        }
        
        // If reward can be given multiple times, always allow it
        if (canGiveMultipleTimes)
        {
            return true;
        }
        
        // Otherwise, check if it hasn't been given before
        return !HasReceivedReward(rewardID);
    }
    
    /// <summary>
    /// Marks a reward as given for tracking purposes
    /// </summary>
    /// <param name="rewardID">Unique identifier for the reward</param>
    public void MarkRewardAsGiven(string rewardID)
    {
        if (string.IsNullOrEmpty(rewardID))
        {
            Debug.LogWarning("DialogueRewardManager: Cannot mark reward as given with null or empty rewardID");
            return;
        }
        
        if (!givenRewards.Contains(rewardID))
        {
            givenRewards.Add(rewardID);
            Debug.Log($"DialogueRewardManager: Marked reward '{rewardID}' as given");
        }
    }
    
    /// <summary>
    /// Checks if a specific reward has already been received
    /// </summary>
    /// <param name="rewardID">Unique identifier for the reward</param>
    /// <returns>True if the reward has been received, false otherwise</returns>
    public bool HasReceivedReward(string rewardID)
    {
        if (string.IsNullOrEmpty(rewardID))
        {
            return false;
        }
        
        return givenRewards.Contains(rewardID);
    }
    
    /// <summary>
    /// Loads given rewards from save data
    /// </summary>
    /// <param name="savedRewards">List of reward IDs that have been given</param>
    public void LoadGivenRewards(List<string> savedRewards)
    {
        if (savedRewards == null)
        {
            givenRewards = new List<string>();
            Debug.Log("DialogueRewardManager: Initialized with empty reward list");
        }
        else
        {
            givenRewards = new List<string>(savedRewards);
            Debug.Log($"DialogueRewardManager: Loaded {givenRewards.Count} given rewards from save data");
        }
    }
    
    /// <summary>
    /// Gets the current list of given rewards for saving
    /// </summary>
    /// <returns>List of reward IDs that have been given</returns>
    public List<string> GetGivenRewards()
    {
        return new List<string>(givenRewards);
    }
    
    /// <summary>
    /// Clears all reward tracking data (useful for testing or new game)
    /// </summary>
    public void ClearAllRewards()
    {
        givenRewards.Clear();
        Debug.Log("DialogueRewardManager: Cleared all reward tracking data");
    }
    
    /// <summary>
    /// Gets the count of rewards that have been given
    /// </summary>
    /// <returns>Number of unique rewards given</returns>
    public int GetGivenRewardCount()
    {
        return givenRewards.Count;
    }
    
    /// <summary>
    /// Validates a DialogueItemReward before attempting to give it
    /// </summary>
    /// <param name="reward">The reward to validate</param>
    /// <returns>True if the reward is valid and can be processed, false otherwise</returns>
    public bool ValidateReward(DialogueItemReward reward)
    {
        if (reward == null)
        {
            Debug.LogError("DialogueRewardManager: Cannot validate null reward");
            return false;
        }
        
        // Validate reward ID
        if (string.IsNullOrEmpty(reward.uniqueRewardID))
        {
            Debug.LogError($"DialogueRewardManager: Reward has null or empty uniqueRewardID. ItemID: {reward.itemID}");
            return false;
        }
        
        // Validate item ID
        if (reward.itemID <= 0)
        {
            Debug.LogError($"DialogueRewardManager: Invalid itemID '{reward.itemID}' for reward '{reward.uniqueRewardID}'. ItemID must be positive.");
            return false;
        }
        
        // Validate quantity
        if (reward.quantity <= 0)
        {
            Debug.LogError($"DialogueRewardManager: Invalid quantity '{reward.quantity}' for reward '{reward.uniqueRewardID}'. Quantity must be positive.");
            return false;
        }
        
        // Validate dialogue index
        if (reward.dialogueIndex < 0)
        {
            Debug.LogError($"DialogueRewardManager: Invalid dialogueIndex '{reward.dialogueIndex}' for reward '{reward.uniqueRewardID}'. DialogueIndex cannot be negative.");
            return false;
        }
        
        // Check if item exists in ItemDictionary
        ItemDictionary itemDictionary = FindAnyObjectByType<ItemDictionary>();
        if (itemDictionary == null)
        {
            Debug.LogError($"DialogueRewardManager: ItemDictionary not found in scene. Cannot validate reward '{reward.uniqueRewardID}'");
            return false;
        }
        
        GameObject itemPrefab = itemDictionary.GetItemPrefab(reward.itemID);
        if (itemPrefab == null)
        {
            Debug.LogError($"DialogueRewardManager: Item with ID '{reward.itemID}' not found in ItemDictionary for reward '{reward.uniqueRewardID}'");
            return false;
        }
        
        // Validate that the item prefab has an Item component
        Item itemComponent = itemPrefab.GetComponent<Item>();
        if (itemComponent == null)
        {
            Debug.LogError($"DialogueRewardManager: Item prefab with ID '{reward.itemID}' does not have an Item component for reward '{reward.uniqueRewardID}'");
            return false;
        }
        
        Debug.Log($"DialogueRewardManager: Reward '{reward.uniqueRewardID}' validation passed");
        return true;
    }
    
    /// <summary>
    /// Checks if any rewards from a specific NPC have been given
    /// </summary>
    /// <param name="npcRewards">Array of rewards from an NPC</param>
    /// <returns>True if any reward from this NPC has been given, false otherwise</returns>
    public bool HasReceivedAnyRewardFromNPC(DialogueItemReward[] npcRewards)
    {
        if (npcRewards == null || npcRewards.Length == 0)
        {
            return false;
        }

        foreach (DialogueItemReward reward in npcRewards)
        {
            if (reward != null && !string.IsNullOrEmpty(reward.uniqueRewardID))
            {
                if (HasReceivedReward(reward.uniqueRewardID))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to give a reward with full validation and error handling
    /// </summary>
    /// <param name="reward">The reward to give</param>
    /// <param name="onSuccess">Callback invoked when reward is successfully given</param>
    /// <param name="onFailure">Callback invoked when reward fails with error message</param>
    /// <returns>True if reward was successfully given, false otherwise</returns>
    public bool TryGiveReward(DialogueItemReward reward, System.Action<GameObject> onSuccess = null, System.Action<string> onFailure = null)
    {
        // Validate the reward first
        if (!ValidateReward(reward))
        {
            string errorMsg = $"Reward validation failed for '{reward?.uniqueRewardID ?? "unknown"}'";
            Debug.LogError($"DialogueRewardManager: {errorMsg}");
            onFailure?.Invoke(errorMsg);
            return false;
        }
        
        // Check if reward can be given based on tracking rules
        if (!CanGiveReward(reward.uniqueRewardID, reward.canGiveMultipleTimes))
        {
            string errorMsg = $"Reward '{reward.uniqueRewardID}' has already been given and cannot be given multiple times";
            Debug.LogWarning($"DialogueRewardManager: {errorMsg}");
            onFailure?.Invoke(errorMsg);
            return false;
        }
        
        try
        {
            // Get item prefab from dictionary (we already validated it exists)
            ItemDictionary itemDictionary = FindAnyObjectByType<ItemDictionary>();
            GameObject itemPrefab = itemDictionary.GetItemPrefab(reward.itemID);
            
            // Create item instance with correct quantity
            GameObject itemInstance = Instantiate(itemPrefab);
            Item itemComponent = itemInstance.GetComponent<Item>();
            if (itemComponent != null)
            {
                itemComponent.quantity = reward.quantity;
                itemComponent.UpdateQuantityDisplay();
            }
            
            // Mark reward as given for tracking
            MarkRewardAsGiven(reward.uniqueRewardID);
            
            Debug.Log($"DialogueRewardManager: Successfully created reward item - ID: {reward.itemID}, Quantity: {reward.quantity}, RewardID: {reward.uniqueRewardID}");
            onSuccess?.Invoke(itemInstance);
            return true;
        }
        catch (System.Exception ex)
        {
            string errorMsg = $"Exception occurred while creating reward '{reward.uniqueRewardID}': {ex.Message}";
            Debug.LogError($"DialogueRewardManager: {errorMsg}");
            onFailure?.Invoke(errorMsg);
            return false;
        }
    }
}