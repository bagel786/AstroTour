using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;
    public string questId;
    public List<QuestObjective> objectives;
    public List<QuestReward> questRewards;


    private void OnValidate() // called whenever object is edited in inspector
    {
        if (string.IsNullOrEmpty(questId))
        {
            questId = System.Guid.NewGuid().ToString();
        }
        
        // Validate objectives
        ValidateObjectives();
    }

    /// <summary>
    /// Validates quest objectives for common configuration issues
    /// </summary>
    private void ValidateObjectives()
    {
        if (objectives == null) return;

        for (int i = 0; i < objectives.Count; i++)
        {
            QuestObjective objective = objectives[i];
            if (objective == null) continue;

            // Validate CollectItem objectives only
            if (objective.type == ObjectiveType.CollectItem)
            {
                int[] acceptableIDs = objective.GetAcceptableItemIDs();
                
                if (acceptableIDs.Length == 0)
                {
                    Debug.LogWarning($"Quest '{questName}': Objective {i} ('{objective.description}') has no valid item IDs. Set either objectiveID or acceptableItemIDs for CollectItem objectives.");
                }
                else if (acceptableIDs.Length > 1)
                {
                    Debug.Log($"Quest '{questName}': Objective {i} ('{objective.description}') accepts multiple item types: [{string.Join(", ", acceptableIDs)}]");
                }
            }
            
            // Validate non-collection objectives have proper IDs
            else if (objective.type == ObjectiveType.InteractWithTerminal || 
                     objective.type == ObjectiveType.CompleteTerminal ||
                     objective.type == ObjectiveType.TalkNPC)
            {
                if (string.IsNullOrEmpty(objective.objectiveID))
                {
                    Debug.LogWarning($"Quest '{questName}': Objective {i} ('{objective.description}') of type {objective.type} needs an objectiveID (terminal ID or NPC name).");
                }
            }

            // Validate required amount
            if (objective.requiredAmount <= 0)
            {
                Debug.LogWarning($"Quest '{questName}': Objective {i} ('{objective.description}') has requiredAmount <= 0");
            }
        }
    }
}

[System.Serializable]
public class QuestObjective
{
    [Header("Basic Objective Settings")]
    public string objectiveID; // match with item id that you need to collect, kill, etc (for backward compatibility)
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    [Header("Multi-Item Collection (Optional)")]
    [Tooltip("For CollectItem objectives: List of acceptable item IDs. If empty, uses objectiveID. Example: Different gym badges with different IDs but same objective.")]
    public int[] acceptableItemIDs; // Array of item IDs that can fulfill this objective

    public bool isCompleted => currentAmount >= requiredAmount;

    /// <summary>
    /// Gets all item IDs that can fulfill this objective (only for CollectItem objectives)
    /// </summary>
    /// <returns>Array of acceptable item IDs</returns>
    public int[] GetAcceptableItemIDs()
    {
        // Only return item IDs for CollectItem objectives
        if (type != ObjectiveType.CollectItem)
        {
            return new int[0];
        }
        
        // If acceptableItemIDs is set and not empty, use it
        if (acceptableItemIDs != null && acceptableItemIDs.Length > 0)
        {
            return acceptableItemIDs;
        }

        // Otherwise, fall back to single objectiveID for backward compatibility
        if (int.TryParse(objectiveID, out int singleItemID))
        {
            return new int[] { singleItemID };
        }

        // Return empty array if no valid IDs found
        return new int[0];
    }

    /// <summary>
    /// Checks if a given item ID is acceptable for this objective
    /// </summary>
    /// <param name="itemID">The item ID to check</param>
    /// <returns>True if this item ID can fulfill the objective</returns>
    public bool AcceptsItemID(int itemID)
    {
        int[] acceptableIDs = GetAcceptableItemIDs();
        foreach (int acceptableID in acceptableIDs)
        {
            if (acceptableID == itemID)
            {
                return true;
            }
        }
        return false;
    }
}

public enum ObjectiveType
{
    CollectItem,
    DefeatEnemy,
    ReachLocation,
    TalkNPC,
    InteractWithTerminal,
    CompleteTerminal,
    Custom
}

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        // deep copy to avoid modifying original quest objectives
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0,
                acceptableItemIDs = obj.acceptableItemIDs != null ? (int[])obj.acceptableItemIDs.Clone() : null
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.isCompleted);

    public string QuestID => quest.questId;
}


[System.Serializable]
public class QuestReward{
    public RewardType type;
    public int rewardID;
    public int amount = 1;
}

public enum RewardType {Item, Gold, Experience, Custom}