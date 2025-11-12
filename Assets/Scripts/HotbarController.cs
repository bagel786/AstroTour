using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10; //1-0 on keyboard

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();

        //Hotbar keys based on slot count
        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }
    
    void Start()
    {
        // Initialize empty slots if they don't exist
        InitializeEmptySlots();
    }
    
    // Initialize empty slots if the hotbar panel is empty
    public void InitializeEmptySlots()
    {
        if (hotbarPanel == null) return;
        
        // Only initialize if there are no slots yet
        if (hotbarPanel.transform.childCount == 0)
        {
            for (int i = 0; i < slotCount; i++)
            {
                Instantiate(slotPrefab, hotbarPanel.transform);
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


    void Update()
    {
        //Check for key press
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemInHotbar(i);
            }
        }
    }

    void UseItemInHotbar(int index)
    {
        var slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if (slot.currentItem != null)
        {
            var item = slot.currentItem.GetComponent<Item>();
            item.UseItem();
        }
    }

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        
        if (hotbarPanel == null)
        {
            return hotbarData;
        }
        
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    hotbarData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex(), quantity = item.quantity });
                }
            }
        }
        
        return hotbarData;
    }

    public void SetHotbarItems(List<InventorySaveData> hotbarSaveData)
    {
        // Ensure ItemDictionary is available
        if (itemDictionary == null)
        {
            itemDictionary = FindAnyObjectByType<ItemDictionary>();
            if (itemDictionary == null)
            {
                return;
            }
        }
        
        // Ensure hotbarPanel is available
        if (hotbarPanel == null)
        {
            return;
        }
        
        // Clear existing slots
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new empty slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        // Wait a frame for slot creation to complete, then load items
        StartCoroutine(LoadHotbarItemsDelayed(hotbarSaveData));
    }
    
    private IEnumerator LoadHotbarItemsDelayed(List<InventorySaveData> hotbarSaveData)
    {
        yield return null; // Wait one frame for slots to be created
        
        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex >= 0 && data.slotIndex < slotCount && data.slotIndex < hotbarPanel.transform.childCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                if (slot == null) continue;
                
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    InventoryController.SetupItemForSlot(item);
                    
                    // Ensure proper UI setup for hotbar items
                    SetupItemForUI(item);
                    
                    Item itemComponent = item.GetComponent<Item>();
                    if (itemComponent != null)
                    {
                        if (data.quantity > 1)
                        {
                            itemComponent.quantity = data.quantity;
                            itemComponent.UpdateQuantityDisplay();
                        }
                        slot.currentItem = item;
                    }
                }
            }
        }
    }
}

