using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;
    public static InventoryController Instance { get; private set; }
    private bool isRebuildingCounts = false;
    
    // Helper method to properly set up item UI properties for inventory slots
    public static void SetupItemForSlot(GameObject item)
    {
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Set anchors to center first
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            
            // Reset position and scale
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            
            // Set size LAST and don't touch offsetMin/offsetMax as they can override sizeDelta
            rectTransform.sizeDelta = new Vector2(100f, 100f); // Fixed size for slot items
        }
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }

    // Start is called before the first frame update
    void Start()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
        // Initialize empty slots if they don't exist
        InitializeEmptySlots();
        // RebuildItemCounts will be called after inventory is loaded
        // Also call it after a frame to ensure any pre-existing items are counted
        StartCoroutine(InitializeItemCountsAfterFrame());
    }
    
    // Initialize empty slots if the inventory panel is empty
    public void InitializeEmptySlots()
    {
        if (inventoryPanel == null) return;
        
        // Only initialize if there are no slots yet
        if (inventoryPanel.transform.childCount == 0)
        {
            for(int i = 0; i < slotCount; i++)
            {
                Instantiate(slotPrefab, inventoryPanel.transform);
            }
        }
    }
    
    // Setup item for UI display (disable SpriteRenderer, ensure Image has sprite)
    private void SetupItemForUI(GameObject item)
    {
        var spriteRenderer = item.GetComponent<SpriteRenderer>();
        var image = item.GetComponent<UnityEngine.UI.Image>();
        var rectTransform = item.GetComponent<RectTransform>();
        
        // Ensure the item has a RectTransform (required for UI)
        if (rectTransform == null)
        {
            rectTransform = item.AddComponent<RectTransform>();
        }
        
        // Ensure the item has an Image component for UI display
        if (image == null && spriteRenderer != null)
        {
            image = item.AddComponent<UnityEngine.UI.Image>();
        }
        
        if (spriteRenderer != null && image != null)
        {
            // Copy sprite from SpriteRenderer to Image if Image doesn't have one
            if (image.sprite == null && spriteRenderer.sprite != null)
            {
                image.sprite = spriteRenderer.sprite;
            }
            
            // Disable SpriteRenderer for UI items
            spriteRenderer.enabled = false;
        }
    }
    
    private IEnumerator InitializeItemCountsAfterFrame()
    {
        yield return null; // Wait one frame
        if (inventoryPanel != null)
        {
            RebuildItemCounts();
        }
    }

        //for (int i = 0; i < slotCount; i++)
        //{
        //    Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
        //    if(i < itemPrefabs.Length)
        //    {
        //        GameObject item = Instantiate(itemPrefabs[i], slot.transform);
        //        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //        slot.currentItem = item;
        //    }
        //}
    public void RebuildItemCounts()
    {
        if (inventoryPanel == null || isRebuildingCounts)
        {
            return;
        }
        
        isRebuildingCounts = true;
        itemsCountCache.Clear();
        
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }
        
        isRebuildingCounts = false;
        OnInventoryChanged?.Invoke();
    }
    public Dictionary<int,int> GetItemCounts() 
    {
        // If cache is empty and inventory panel exists, rebuild it (but don't trigger events to avoid recursion)
        if (itemsCountCache.Count == 0 && inventoryPanel != null && !isRebuildingCounts)
        {
            RebuildItemCountsSilent();
        }
        
        // If still empty (maybe inventory panel is null), calculate fresh counts
        if (itemsCountCache.Count == 0 && inventoryPanel != null)
        {
            return CalculateItemCountsFresh();
        }
        
        return itemsCountCache;
    }
    
    // Rebuild item counts without triggering OnInventoryChanged event
    private void RebuildItemCountsSilent()
    {
        if (inventoryPanel == null || isRebuildingCounts)
        {
            return;
        }
        
        isRebuildingCounts = true;
        itemsCountCache.Clear();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }
        isRebuildingCounts = false;
        // Don't invoke OnInventoryChanged here to avoid recursion
    }
    
    // Helper method to calculate item counts without using cache
    private Dictionary<int, int> CalculateItemCountsFresh()
    {
        Dictionary<int, int> freshCounts = new Dictionary<int, int>();
        
        if (inventoryPanel == null) return freshCounts;
        
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    freshCounts[item.ID] = freshCounts.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }
        
        return freshCounts;
    }

    public bool AddItem(GameObject itemPrefab, bool showUINotification = true)
    {
        //Look for empty slot
        Item itemToAdd = itemPrefab.GetComponent<Item>(); 
        if (itemToAdd == null)
        {
            return false;
        }
        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack(itemToAdd.quantity);
                    
                    // Show pickup UI for stacked items if requested
                    if (showUINotification && ItemPickupUIController.Instance != null)
                    {
                        Sprite icon = itemPrefab.GetComponent<SpriteRenderer>()?.sprite;
                        ItemPickupUIController.Instance.ShowItemPickup(itemToAdd.itemName, icon);
                    }
                    RebuildItemCounts();
                    return true;
                }
            }
        }
        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTranform);
                SetupItemForSlot(newItem);
                SetupItemForUI(newItem);
                slot.currentItem = newItem;
                // Show pickup UI if requested
                if (showUINotification)
                {
                    Item itemComp = newItem.GetComponent<Item>();
                    Sprite icon = newItem.GetComponent<SpriteRenderer>()?.sprite;
                    if (itemComp != null && ItemPickupUIController.Instance != null)
                    {
                        ItemPickupUIController.Instance.ShowItemPickup(itemComp.itemName, icon);
                    }
                }
                RebuildItemCounts();
                return true;
            }
        }


        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        
        if (inventoryPanel == null)
        {
            return invData;
        }
        
        foreach(Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTranform.GetSiblingIndex(), quantity = item.quantity });
                }
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {

        //Clear inventory panel - avoid duplicates
        foreach(Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        //Create new slots
        for(int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        // Build a direct-child Slot list (skip non-slot children like headers)
        List<Slot> slotList = new List<Slot>();
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            var s = inventoryPanel.transform.GetChild(i).GetComponent<Slot>();
            if (s != null) slotList.Add(s);
        }

        if (slotList.Count == 0)
        {            return;
        }

        //Populate slots with saved items
        //Populate slots with saved items -- use slotList indices (only slot GameObjects)
        foreach(InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < 0)
            {
                continue;
            }

            if (data.slotIndex < slotList.Count)
            {
                Slot slot = slotList[data.slotIndex];
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if(itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    SetupItemForSlot(item);
                    
                    // Ensure proper UI setup for inventory items
                    SetupItemForUI(item);
                    
                    Item itemComponent = item.GetComponent<Item>();
                    if(itemComponent != null && data.quantity > 1)
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDisplay();
                    }
                    slot.currentItem = item;
                }
            }
            
        }
        RebuildItemCounts();
    }
    public void RemoveItemsFromInventory(int itemID, int amountToRemove){
        foreach(Transform slotTranform in inventoryPanel.transform){
            if(amountToRemove<= 0) break;
            Slot slot = slotTranform.GetComponent<Slot>();
            if(slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID){
                int removed = Mathf.Min(amountToRemove,item.quantity);
                item.RemoveFromStack(removed);
                amountToRemove -= removed;
                if(item.quantity == 0){
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
        }
        RebuildItemCounts();
    }
}
