using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static QuestController Instance { get; private set; }
    public List<QuestProgress> activeQuests = new List<QuestProgress>();
    private QuestUI questUI;
    public List<string> handInQuestIDS = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        questUI = FindAnyObjectByType<QuestUI>();
        
        // Ensure activeQuests is initialized
        if (activeQuests == null)
        {
            activeQuests = new List<QuestProgress>();
        }
        
        // Clean up any null quests that might have been loaded
        CleanupNullQuests();
    }
    
    /// <summary>
    /// Remove any null or corrupted quest entries from the active quests list
    /// </summary>
    private void CleanupNullQuests()
    {
        if (activeQuests != null)
        {
            int removedCount = activeQuests.RemoveAll(q => 
            {
                if (q == null) return true;
                try
                {
                    return string.IsNullOrEmpty(q.QuestID) || q.objectives == null;
                }
                catch
                {
                    return true; // Remove if we can't access properties
                }
            });
            if (removedCount > 0)
            {
                Debug.LogWarning($"QuestController: Removed {removedCount} corrupted quest entries");
            }
        }
    }
    
    /// <summary>
    /// Public method to manually clean up corrupted quests
    /// </summary>
    public void ForceCleanupQuests()
    {
        CleanupNullQuests();
        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    }
    
    private void Start()
    {
        // Subscribe to inventory changes after all Awake methods have been called
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
        }
    }
    public void AcceptQuest(Quest quest)
    {
        if(IsQuestActive(quest.questId)) return;
        Debug.Log($"QuestController: Accepting quest '{quest.questId}' - checking for retroactive progress");
        activeQuests.Add(new QuestProgress(quest));
        CheckInventoryForQuests();
        CheckTerminalsForQuests(); // Check for already completed terminals
        
        // Force an immediate check for this specific quest
        CheckSpecificQuestForCompletedTerminals(quest.questId);
        
        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    } 
    public bool IsQuestActive(string questId)
    {
        if (activeQuests == null || string.IsNullOrEmpty(questId))
        {
            return false;
        }
        
        try
        {
            foreach (QuestProgress quest in activeQuests)
            {
                if (quest == null) continue;
                
                try
                {
                    if (!string.IsNullOrEmpty(quest.QuestID) && quest.QuestID == questId)
                    {
                        return true;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"QuestController: Error checking quest active status: {ex.Message}");
                    continue;
                }
            }
            
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QuestController: Error in IsQuestActive: {ex.Message}");
            return false;
        }
    }
    public void CheckInventoryForQuests(){
        if (InventoryController.Instance == null)
        {
            return;
        }
        
        // Clean up any corrupted quests before processing
        if (activeQuests == null)
        {
            activeQuests = new List<QuestProgress>();
            return;
        }
        
        CleanupNullQuests();
        
        // Get items from both inventory and hotbar
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        
        // Add hotbar items to the count
        HotbarController hotbarController = FindAnyObjectByType<HotbarController>();
        if (hotbarController != null)
        {
            var hotbarItems = hotbarController.GetHotbarItems();
            
            foreach(var hotbarItem in hotbarItems)
            {
                itemCounts[hotbarItem.itemID] = itemCounts.GetValueOrDefault(hotbarItem.itemID, 0) + hotbarItem.quantity;
            }
        }
        
        foreach(QuestProgress quest in activeQuests){
            // Skip null or invalid quests
            if (quest == null || quest.objectives == null || string.IsNullOrEmpty(quest.QuestID))
            {
                continue;
            }
            
            foreach(QuestObjective questObjective in quest.objectives){
                if (questObjective == null) continue;
                if(questObjective.type != ObjectiveType.CollectItem) 
                {

                    continue;
                }
                
                // Get all acceptable item IDs for this objective
                int[] acceptableItemIDs = questObjective.GetAcceptableItemIDs();

                
                if (acceptableItemIDs.Length == 0) 
                {
                    Debug.LogWarning($"QuestController: Collect objective '{questObjective.description}' has no acceptable item IDs!");
                    continue;
                }
                
                // Count total items across all acceptable IDs
                int totalCount = 0;
                foreach (int itemID in acceptableItemIDs)
                {
                    if (itemCounts.TryGetValue(itemID, out int count))
                    {
                        totalCount += count;

                    }
                }
                
                int newAmount = Mathf.Min(totalCount, questObjective.requiredAmount);

                
                if(questObjective.currentAmount != newAmount){

                    questObjective.currentAmount = newAmount;
                }
            }
        }
        
        if (questUI != null)
        {

            questUI.UpdateQuestUI();
        }
        else
        {
            Debug.LogWarning("QuestController: questUI is null, cannot update quest UI");
        }
    }
    public bool IsQuestCompleted(string questID){
        if (activeQuests == null || string.IsNullOrEmpty(questID))
        {
            return false;
        }
        
        try
        {
            // Use foreach instead of Find to avoid lambda issues with corrupted data
            foreach (QuestProgress quest in activeQuests)
            {
                if (quest == null) continue;
                
                try
                {
                    if (string.IsNullOrEmpty(quest.QuestID)) continue;
                    if (quest.QuestID != questID) continue;
                    
                    // Found the quest, check if completed
                    if (quest.objectives == null) return false;
                    
                    foreach (var objective in quest.objectives)
                    {
                        if (objective == null)
                        {
                            Debug.LogWarning($"QuestController: Found null objective in quest {quest.QuestID}");
                            return false;
                        }
                        
                        try
                        {
                            if (!objective.isCompleted)
                            {
                                return false;
                            }
                        }
                        catch (System.Exception objEx)
                        {
                            Debug.LogError($"QuestController: Error accessing objective.isCompleted: {objEx.Message}");
                            return false;
                        }
                    }
                    
                    return true; // All objectives completed
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"QuestController: Error checking quest objective: {ex.Message}");
                    continue;
                }
            }
            
            return false; // Quest not found
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QuestController: Error in IsQuestCompleted: {ex.Message}");
            return false;
        }
    }
    public void HandInQuest(string questID){
        //
        if(!RemoveRequiredItemsFromInventory(questID)){
            return;
        }
        // remove quest from quest log

        if (activeQuests == null) return;
        
        try
        {
            QuestProgress questToRemove = null;
            
            foreach (QuestProgress quest in activeQuests)
            {
                if (quest == null) continue;
                
                try
                {
                    string currentQuestID = null;
                    try
                    {
                        currentQuestID = quest.QuestID;
                    }
                    catch (System.Exception idEx)
                    {
                        Debug.LogError($"QuestController: Error accessing quest.QuestID: {idEx.Message}");
                        continue;
                    }
                    
                    if (!string.IsNullOrEmpty(currentQuestID) && currentQuestID == questID)
                    {
                        questToRemove = quest;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"QuestController: Error in HandInQuest quest search: {ex.Message}");
                    continue;
                }
            }
            
            if (questToRemove != null)
            {
                handInQuestIDS.Add(questID);
                activeQuests.Remove(questToRemove);
                
                if (questUI != null)
                {
                    questUI.UpdateQuestUI();
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QuestController: Error in HandInQuest: {ex.Message}");
        }


    }
    public bool IsQuestHandedIn(string questID){
        return handInQuestIDS.Contains(questID);
    }
    public bool RemoveRequiredItemsFromInventory(string questID){
        if (activeQuests == null) return false;
        
        QuestProgress quest = null;
        
        try
        {
            foreach (QuestProgress q in activeQuests)
            {
                if (q == null) continue;
                
                try
                {
                    if (!string.IsNullOrEmpty(q.QuestID) && q.QuestID == questID)
                    {
                        quest = q;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"QuestController: Error in RemoveRequiredItemsFromInventory quest search: {ex.Message}");
                    continue;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QuestController: Error in RemoveRequiredItemsFromInventory: {ex.Message}");
            return false;
        }
        
        if (quest == null) return false;

        // Build list of objectives that require item removal
        List<QuestObjective> collectObjectives = new List<QuestObjective>();
        foreach(QuestObjective objective in quest.objectives){
            if(objective.type == ObjectiveType.CollectItem){
                int[] acceptableItemIDs = objective.GetAcceptableItemIDs();
                if (acceptableItemIDs.Length > 0)
                {
                    collectObjectives.Add(objective);
                }
            }
        }

        Dictionary<int,int> itemCounts = InventoryController.Instance.GetItemCounts();
        
        // Check if we have enough items for each objective
        foreach(QuestObjective objective in collectObjectives){
            int[] acceptableItemIDs = objective.GetAcceptableItemIDs();
            
            // Count total available items across all acceptable IDs
            int totalAvailable = 0;
            foreach (int itemID in acceptableItemIDs)
            {
                totalAvailable += itemCounts.GetValueOrDefault(itemID, 0);
            }
            
            if (totalAvailable < objective.requiredAmount)
            {
                Debug.LogWarning($"QuestController: Not enough items for objective '{objective.description}'. Required: {objective.requiredAmount}, Available: {totalAvailable}");
                return false;
            }
        }

        // Remove items from inventory for each objective
        foreach(QuestObjective objective in collectObjectives){
            int[] acceptableItemIDs = objective.GetAcceptableItemIDs();
            int remainingToRemove = objective.requiredAmount;
            
            // Remove items in order of availability until we've removed enough
            foreach (int itemID in acceptableItemIDs)
            {
                if (remainingToRemove <= 0) break;
                
                int availableCount = itemCounts.GetValueOrDefault(itemID, 0);
                if (availableCount > 0)
                {
                    int toRemove = Mathf.Min(availableCount, remainingToRemove);
                    InventoryController.Instance.RemoveItemsFromInventory(itemID, toRemove);
                    remainingToRemove -= toRemove;
                    
                    Debug.Log($"QuestController: Removed {toRemove} of item ID {itemID} for quest objective");
                }
            }
            
            if (remainingToRemove > 0)
            {
                Debug.LogError($"QuestController: Failed to remove all required items for objective '{objective.description}'. Still need {remainingToRemove} more.");
                return false;
            }
        }
        
        return true;
    }
    public void LoadQuestProgress(List<QuestProgress> savedQuests){
        activeQuests = savedQuests ?? new();
        
        // Restore quest ScriptableObject references from questId
        foreach (QuestProgress questProgress in activeQuests)
        {
            if (questProgress != null && !string.IsNullOrEmpty(questProgress.questId))
            {
                // Find the quest ScriptableObject by ID
                if (questProgress.quest == null)
                {
                    Quest[] allQuests = Resources.LoadAll<Quest>("");
                    questProgress.quest = System.Array.Find(allQuests, q => q != null && q.questId == questProgress.questId);
                    
                    if (questProgress.quest == null)
                    {
                        Debug.LogWarning($"QuestController: Could not find Quest ScriptableObject for questId '{questProgress.questId}'");
                    }
                }
                
                // Restore acceptableItemIDs from original quest configurations
                if (questProgress.quest?.objectives != null && questProgress.objectives != null)
                {
                    for (int i = 0; i < questProgress.objectives.Count && i < questProgress.quest.objectives.Count; i++)
                    {
                        // Restore acceptableItemIDs from the original quest ScriptableObject
                        questProgress.objectives[i].acceptableItemIDs = questProgress.quest.objectives[i].acceptableItemIDs;
                    }
                }
            }
        }
        
        CheckInventoryForQuests();
        CheckTerminalsForQuests(); // Check for completed terminals when loading
        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
        
        // Do an additional check after a short delay to ensure inventory is fully loaded
        StartCoroutine(DelayedInventoryCheck());
    }
    
    private System.Collections.IEnumerator DelayedInventoryCheck()
    {
        yield return new WaitForSeconds(0.1f);
        CheckInventoryForQuests();
        CheckTerminalsForQuests(); // Also check terminals after delay
    }
    
    private System.Collections.IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForEndOfFrame();
        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    }
    
    /// <summary>
    /// Check if completing a terminal should progress any quest objectives
    /// </summary>
    public void CheckTerminalUnlock(string terminalID)
    {
        if (activeQuests == null || string.IsNullOrEmpty(terminalID)) return;
        
        CleanupNullQuests();
        
        foreach(QuestProgress quest in activeQuests)
        {
            if (quest == null || quest.objectives == null) continue;
            
            foreach(QuestObjective objective in quest.objectives)
            {
                if (objective == null) continue;
                
                // Check if this is a terminal-related objective
                if(objective.type == ObjectiveType.InteractWithTerminal || 
                   objective.type == ObjectiveType.CompleteTerminal)
                {
                    // Check if this terminal matches the objective's target
                    if(objective.objectiveID == terminalID || 
                       objective.description.Contains(terminalID) ||
                       objective.description.ToLower().Contains("marketing") ||
                       objective.description.ToLower().Contains("dna") ||
                       objective.description.ToLower().Contains("sequencing"))
                    {
                        objective.currentAmount = Mathf.Min(objective.currentAmount + 1, objective.requiredAmount);
                        Debug.Log($"QuestController: Terminal {terminalID} completed, updated objective: {objective.description}");
                    }
                }
            }
        }
        
        // Update quest UI to reflect any changes (with safety check)
        if (questUI != null)
        {
            // Use delayed update to avoid threading issues with UI
            StartCoroutine(DelayedUIUpdate());
        }
    }
    
    /// <summary>
    /// Check if interacting with an NPC should progress any quest objectives
    /// </summary>
    public void CheckNPCInteraction(string npcName)
    {
        if (activeQuests == null || string.IsNullOrEmpty(npcName)) return;
        
        CleanupNullQuests();
        
        foreach(QuestProgress quest in activeQuests)
        {
            if (quest == null || quest.objectives == null) continue;
            
            foreach(QuestObjective objective in quest.objectives)
            {
                if (objective == null) continue;
                
                // Check if this is an NPC interaction objective
                if(objective.type == ObjectiveType.TalkNPC)
                {
                    // Check if this NPC matches the objective's target
                    if(objective.objectiveID == npcName || 
                       objective.description.Contains(npcName) ||
                       objective.description.ToLower().Contains(npcName.ToLower()))
                    {
                        objective.currentAmount = Mathf.Min(objective.currentAmount + 1, objective.requiredAmount);
                        Debug.Log($"QuestController: Talked to NPC {npcName}, updated objective: {objective.description}");
                    }
                }
            }
        }
        
        // Update quest UI to reflect any changes (with safety check)
        if (questUI != null)
        {
            // Use delayed update to avoid threading issues with UI
            StartCoroutine(DelayedUIUpdate());
        }
    }
    
    /// <summary>
    /// Check all terminals in the scene for quest objectives when a quest is accepted
    /// This handles the case where terminals were completed before the quest was accepted
    /// </summary>
    public void CheckTerminalsForQuests()
    {
        if (activeQuests == null) return;
        
        CleanupNullQuests();
        
        // Find all completed terminals in the scene
        SimpleConsolePassword[] passwordTerminals = FindObjectsByType<SimpleConsolePassword>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        MarketingTerminal[] marketingTerminals = FindObjectsByType<MarketingTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        SimpleDNASequencingTerminal[] dnaTerminals = FindObjectsByType<SimpleDNASequencingTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach(QuestProgress quest in activeQuests)
        {
            if (quest == null || quest.objectives == null || string.IsNullOrEmpty(quest.QuestID))
            {
                continue;
            }
            
            foreach(QuestObjective objective in quest.objectives)
            {
                if (objective == null) continue;
                
                // Check if this is a terminal-related objective
                if(objective.type == ObjectiveType.InteractWithTerminal || 
                   objective.type == ObjectiveType.CompleteTerminal)
                {
                    // Check password terminals
                    foreach(SimpleConsolePassword terminal in passwordTerminals)
                    {
                        if(terminal != null && terminal.hasAccessed)
                        {
                            if(objective.objectiveID == terminal.terminalID || 
                               objective.description.Contains(terminal.terminalID))
                            {
                                objective.currentAmount = Mathf.Min(objective.currentAmount + 1, objective.requiredAmount);
                                Debug.Log($"QuestController: Retroactively found completed password terminal {terminal.terminalID} for objective: {objective.description}");
                            }
                        }
                    }
                    
                    // Check marketing terminals
                    foreach(MarketingTerminal terminal in marketingTerminals)
                    {
                        if(terminal != null && terminal.hasCompleted)
                        {
                            // Generate terminal ID if needed (same logic as in MarketingTerminal)
                            string terminalID = !string.IsNullOrEmpty(terminal.terminalID) ? 
                                terminal.terminalID : 
                                $"marketing_terminal_{terminal.transform.position.x:F1}_{terminal.transform.position.y:F1}";
                                
                            if(objective.objectiveID == terminalID || 
                               objective.description.Contains(terminalID) ||
                               objective.description.ToLower().Contains("marketing"))
                            {
                                objective.currentAmount = Mathf.Min(objective.currentAmount + 1, objective.requiredAmount);
                                Debug.Log($"QuestController: Retroactively found completed marketing terminal {terminalID} for objective: {objective.description}");
                            }
                        }
                    }
                    
                    // Check DNA terminals
                    foreach(SimpleDNASequencingTerminal terminal in dnaTerminals)
                    {
                        if(terminal != null && terminal.hasCompleted)
                        {
                            // Generate terminal ID if needed (same logic as in SimpleDNASequencingTerminal)
                            string terminalID = !string.IsNullOrEmpty(terminal.terminalID) ? 
                                terminal.terminalID : 
                                $"dna_terminal_{terminal.transform.position.x:F1}_{terminal.transform.position.y:F1}";
                                
                            if(objective.objectiveID == terminalID || 
                               objective.description.Contains(terminalID) ||
                               objective.description.ToLower().Contains("dna") ||
                               objective.description.ToLower().Contains("sequencing"))
                            {
                                objective.currentAmount = Mathf.Min(objective.currentAmount + 1, objective.requiredAmount);
                                Debug.Log($"QuestController: Retroactively found completed DNA terminal {terminalID} for objective: {objective.description}");
                            }
                        }
                    }
                }
            }
        }
        
        // Update quest UI to reflect any changes
        if (questUI != null)
        {
            questUI.UpdateQuestUI();
        }
    }
    
    /// <summary>
    /// Check completed terminals for a specific quest (used when quest is newly accepted)
    /// </summary>
    private void CheckSpecificQuestForCompletedTerminals(string questID)
    {
        try
        {
            QuestProgress targetQuest = null;
            
            foreach (QuestProgress quest in activeQuests)
            {
                if (quest == null) continue;
                
                try
                {
                    if (!string.IsNullOrEmpty(quest.QuestID) && quest.QuestID == questID)
                    {
                        targetQuest = quest;
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"QuestController: Error finding specific quest: {ex.Message}");
                    continue;
                }
            }
            
            if (targetQuest == null) return;
            
            // Find all completed terminals in the scene
            SimpleDNASequencingTerminal[] dnaTerminals = FindObjectsByType<SimpleDNASequencingTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            MarketingTerminal[] marketingTerminals = FindObjectsByType<MarketingTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            SimpleConsolePassword[] passwordTerminals = FindObjectsByType<SimpleConsolePassword>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach(QuestObjective objective in targetQuest.objectives)
            {
                if(objective.type == ObjectiveType.InteractWithTerminal || 
                   objective.type == ObjectiveType.CompleteTerminal)
                {
                    // Check DNA terminals
                    foreach(SimpleDNASequencingTerminal terminal in dnaTerminals)
                    {
                        if(terminal != null && terminal.hasCompleted)
                        {
                            string terminalID = terminal.TerminalID;
                            
                            if(objective.objectiveID == terminalID || 
                               objective.description.Contains(terminalID) ||
                               objective.description.ToLower().Contains("dna") ||
                               objective.description.ToLower().Contains("sequencing"))
                            {
                                objective.currentAmount = objective.requiredAmount; // Mark as completed
                                Debug.Log($"QuestController: Found completed DNA terminal {terminalID} for newly accepted quest objective: {objective.description}");
                            }
                        }
                    }
                    
                    // Check marketing terminals
                    foreach(MarketingTerminal terminal in marketingTerminals)
                    {
                        if(terminal != null && terminal.hasCompleted)
                        {
                            string terminalID = terminal.TerminalID;
                            
                            if(objective.objectiveID == terminalID || 
                               objective.description.Contains(terminalID) ||
                               objective.description.ToLower().Contains("marketing"))
                            {
                                objective.currentAmount = objective.requiredAmount; // Mark as completed
                                Debug.Log($"QuestController: Found completed marketing terminal {terminalID} for newly accepted quest objective: {objective.description}");
                            }
                        }
                    }
                    
                    // Check password terminals
                    foreach(SimpleConsolePassword terminal in passwordTerminals)
                    {
                        if(terminal != null && terminal.hasAccessed)
                        {
                            if(objective.objectiveID == terminal.terminalID || 
                               objective.description.Contains(terminal.terminalID))
                            {
                                objective.currentAmount = objective.requiredAmount; // Mark as completed
                                Debug.Log($"QuestController: Found completed password terminal {terminal.terminalID} for newly accepted quest objective: {objective.description}");
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"QuestController: Error in CheckSpecificQuestForCompletedTerminals: {ex.Message}");
        }
    }

}
