using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [Header("Save System Configuration")]
    [SerializeField] private bool useModernSaveSystem = true;
    
    private string saveLocation; // Legacy save location for fallback
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Box[] boxes;
    private SimpleConsolePassword[] terminals;
    private MarketingTerminal[] marketingTerminals;
    private SimpleDNASequencingTerminal[] dnaTerminals;
    private LogicGateTerminal[] logicGateTerminals;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        // Delay loading to ensure all components are initialized
        StartCoroutine(LoadGameDelayed());
    }
    
    private IEnumerator LoadGameDelayed()
    {
        // Wait multiple frames to ensure all Start() methods have been called
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        
        // Re-initialize components if any are null
        if (inventoryController == null || hotbarController == null)
        {
            Debug.LogWarning("SaveController: Re-initializing components...");
            InitializeComponents();
        }
        
        LoadGame();
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        
        // Ensure SaveSystemManager exists
        if (SaveSystemManager.Instance == null)
        {
            GameObject saveSystemManagerGO = new GameObject("SaveSystemManager");
            saveSystemManagerGO.AddComponent<SaveSystemManager>();
        }
        
        inventoryController = FindAnyObjectByType<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogError("SaveController: InventoryController not found in scene!");
        }
        
        hotbarController = FindAnyObjectByType<HotbarController>();
        if (hotbarController == null)
        {
            Debug.LogError("SaveController: HotbarController not found in scene!");
        }
        
        boxes = FindObjectsByType<Box>(FindObjectsSortMode.None);
        terminals = FindObjectsByType<SimpleConsolePassword>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        marketingTerminals = FindObjectsByType<MarketingTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        dnaTerminals = FindObjectsByType<SimpleDNASequencingTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        logicGateTerminals = FindObjectsByType<LogicGateTerminal>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        

        
        Debug.Log($"SaveController initialized: Found {boxes?.Length ?? 0} boxes, {terminals?.Length ?? 0} password terminals, {marketingTerminals?.Length ?? 0} marketing terminals, {dnaTerminals?.Length ?? 0} DNA terminals, and {logicGateTerminals?.Length ?? 0} logic gate terminals");
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
            boxSaveData = GetBoxState(),
            terminalSaveData = GetTerminalState(),
            questProgressData = QuestController.Instance != null ? QuestController.Instance.activeQuests : new List<QuestProgress>(),
            handInQuestIDS = QuestController.Instance != null ? QuestController.Instance.handInQuestIDS : new List<string>(),
            givenDialogueRewardIDs = DialogueRewardManager.Instance != null ? DialogueRewardManager.Instance.GetGivenRewards() : new List<string>()
        };

        if (useModernSaveSystem && SaveSystemManager.Instance != null)
        {
            // Use the modern save system (automatically detects PlayerPrefs vs JSON)
            SaveSystemManager.Instance.SaveGame(saveData);
        }
        else
        {
            // Fallback to legacy file system
            File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        }
    }

    private List<BoxSaveData> GetBoxState()
    {
        List<BoxSaveData> boxStates = new List<BoxSaveData>();
        foreach (Box box in boxes)
        {
            BoxSaveData boxSaveData = new BoxSaveData
            {
                boxID = box.boxID,
                isOpened = box.isOpened
            };
            boxStates.Add(boxSaveData);

        }
        return boxStates;
    }

    private List<TerminalSaveData> GetTerminalState()
    {
        List<TerminalSaveData> terminalStates = new List<TerminalSaveData>();
        
        // Save password terminals
        foreach (SimpleConsolePassword terminal in terminals)
        {
            if (terminal != null)
            {
                terminalStates.Add(terminal.GetSaveData());
            }
        }
        
        // Save marketing terminals
        foreach (MarketingTerminal marketingTerminal in marketingTerminals)
        {
            if (marketingTerminal != null)
            {
                terminalStates.Add(marketingTerminal.GetSaveData());
            }
        }
        
        // Save DNA terminals
        foreach (SimpleDNASequencingTerminal dnaTerminal in dnaTerminals)
        {
            if (dnaTerminal != null)
            {
                terminalStates.Add(dnaTerminal.GetSaveData());
            }
        }
        
        // Save logic gate terminals
        foreach (LogicGateTerminal logicGateTerminal in logicGateTerminals)
        {
            if (logicGateTerminal != null)
            {
                terminalStates.Add(logicGateTerminal.GetSaveData());
            }
        }
        
        return terminalStates;
    }

    public void LoadGame()
    {
        SaveData saveData = null;
        
        if (useModernSaveSystem && SaveSystemManager.Instance != null)
        {
            // Try to load using the modern save system first
            saveData = SaveSystemManager.Instance.LoadGame<SaveData>();
            if (saveData != null)
            {
                Debug.Log("Game loaded using SaveSystemManager (auto-detected platform)");
            }
        }
        
        // Fallback to legacy system if modern system failed or is disabled
        if (saveData == null && File.Exists(saveLocation))
        {
            Debug.Log($"Loading game from legacy system: {saveLocation}");
            saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
        }
        
        if (saveData != null)
        {

            // Load player position with null check
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = saveData.playerPosition;
            }
            else
            {
                Debug.LogError("SaveController: Player GameObject not found!");
            }

            // Load camera boundary with null checks
            if (!string.IsNullOrEmpty(saveData.mapBoundary))
            {
                GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
                if (boundaryObject != null)
                {
                    PolygonCollider2D savedMapBoundry = boundaryObject.GetComponent<PolygonCollider2D>();
                    CinemachineConfiner2D confiner = FindAnyObjectByType<CinemachineConfiner2D>();
                    if (savedMapBoundry != null && confiner != null)
                    {
                        confiner.BoundingShape2D = savedMapBoundry;
                    }
                }
            }

            // Load inventory with null check
            if (inventoryController != null)
            {
                inventoryController.SetInventoryItems(saveData.inventorySaveData);
            }
            else
            {
                Debug.LogError("SaveController: InventoryController is null!");
            }

            // Load hotbar with null check
            if (hotbarController != null)
            {
                hotbarController.SetHotbarItems(saveData.hotbarSaveData);
            }
            else
            {
                Debug.LogError("SaveController: HotbarController is null!");
            }

            LoadBoxStates(saveData.boxSaveData);
            LoadTerminalStates(saveData.terminalSaveData);
            
            // Load quest data with safety checks
            if (QuestController.Instance != null)
            {
                // Delay quest loading to ensure inventory is fully loaded
                StartCoroutine(LoadQuestProgressDelayed(saveData.questProgressData));
                QuestController.Instance.handInQuestIDS = saveData.handInQuestIDS ?? new List<string>();
            }
            else
            {
                Debug.LogWarning("SaveController: QuestController.Instance is null during load. Quest data will not be loaded.");
            }
            
            // Load dialogue reward data
            if (DialogueRewardManager.Instance != null)
            {
                DialogueRewardManager.Instance.LoadGivenRewards(saveData.givenDialogueRewardIDs ?? new List<string>());
            }
        }
        else
        {
            // No save data found in either system
            bool hasSaveFile = false;
            
            if (useModernSaveSystem && SaveSystemManager.Instance != null)
            {
                hasSaveFile = SaveSystemManager.Instance.HasSaveFile();
            }
            
            if (!hasSaveFile && File.Exists(saveLocation))
            {
                hasSaveFile = true;
            }
            
            if (!hasSaveFile)
            {
                Debug.Log("No save file found in any system. Creating new save file.");
                SaveGame();
            }

            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            hotbarController.SetHotbarItems(new List<InventorySaveData>());
            
            // Reset all terminals for new game
            foreach (SimpleConsolePassword terminal in terminals)
            {
                if (terminal != null)
                {
                    terminal.ResetTerminal();
                }
            }
            
            foreach (MarketingTerminal marketingTerminal in marketingTerminals)
            {
                if (marketingTerminal != null)
                {
                    marketingTerminal.ResetTerminal();
                }
            }
            
            foreach (SimpleDNASequencingTerminal dnaTerminal in dnaTerminals)
            {
                if (dnaTerminal != null)
                {
                    dnaTerminal.ResetTerminal();
                }
            }
            
            // Initialize DialogueRewardManager with empty list for new game
            if (DialogueRewardManager.Instance != null)
            {
                DialogueRewardManager.Instance.LoadGivenRewards(new List<string>());
            }
        }
    }

    private void LoadBoxStates(List<BoxSaveData> boxStates)
    {
        foreach (Box box in boxes)
        {
            BoxSaveData boxSaveData = boxStates.FirstOrDefault(b => b.boxID == box.boxID);
            if(boxSaveData != null)
            {
                box.SetOpened(boxSaveData.isOpened);
            }
        }
    }

    private void LoadTerminalStates(List<TerminalSaveData> terminalStates)
    {
        if (terminalStates == null) return;

        // Load password terminals
        foreach (SimpleConsolePassword terminal in terminals)
        {
            if (terminal != null)
            {
                TerminalSaveData terminalSaveData = terminalStates.FirstOrDefault(t => t.terminalID == terminal.terminalID);
                if (terminalSaveData != null)
                {
                    terminal.LoadSaveData(terminalSaveData);
                }
            }
        }
        
        // Load marketing terminals
        foreach (MarketingTerminal marketingTerminal in marketingTerminals)
        {
            if (marketingTerminal != null)
            {
                TerminalSaveData terminalSaveData = terminalStates.FirstOrDefault(t => t.terminalID == marketingTerminal.TerminalID);
                if (terminalSaveData != null)
                {
                    marketingTerminal.LoadSaveData(terminalSaveData);
                }
            }
        }
        
        // Load DNA terminals
        foreach (SimpleDNASequencingTerminal dnaTerminal in dnaTerminals)
        {
            if (dnaTerminal != null)
            {
                TerminalSaveData terminalSaveData = terminalStates.FirstOrDefault(t => t.terminalID == dnaTerminal.TerminalID);
                if (terminalSaveData != null)
                {
                    dnaTerminal.LoadSaveData(terminalSaveData);
                }
            }
        }
        
        // Load logic gate terminals
        foreach (LogicGateTerminal logicGateTerminal in logicGateTerminals)
        {
            if (logicGateTerminal != null)
            {
                TerminalSaveData terminalSaveData = terminalStates.FirstOrDefault(t => t.terminalID == logicGateTerminal.TerminalID);
                if (terminalSaveData != null)
                {
                    logicGateTerminal.LoadSaveData(terminalSaveData);
                }
            }
        }
    }
    
    private IEnumerator LoadQuestProgressDelayed(List<QuestProgress> questProgressData)
    {
        // Wait a couple frames to ensure inventory is fully loaded
        yield return null;
        yield return null;
        
        if (QuestController.Instance != null)
        {
            QuestController.Instance.LoadQuestProgress(questProgressData);
        }

    }
    
    /// <summary>
    /// Delete save data from both modern and legacy save systems
    /// </summary>
    public void DeleteSaveData()
    {
        // Delete from modern save system
        if (SaveSystemManager.Instance != null)
        {
            SaveSystemManager.Instance.DeleteSave();
            Debug.Log("Save data deleted from SaveSystemManager");
        }
        
        // Delete from legacy save system
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log($"Legacy save file deleted: {saveLocation}");
        }
    }
    
    /// <summary>
    /// Check if save data exists in either system
    /// </summary>
    public bool HasSaveData()
    {
        // Check modern save system first
        if (useModernSaveSystem && SaveSystemManager.Instance != null)
        {
            if (SaveSystemManager.Instance.HasSaveFile())
            {
                return true;
            }
        }
        
        // Check legacy save system
        return File.Exists(saveLocation);
    }
}
