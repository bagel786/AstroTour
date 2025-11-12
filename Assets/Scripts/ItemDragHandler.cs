using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;

    private InventoryController inventoryController;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Add CanvasGroup if it doesn't exist (needed for drag transparency)
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        inventoryController = InventoryController.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //Save OG parent
        transform.SetParent(transform.root); //Above other canvas'
        
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f; //Semi-transparent during drag
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; //Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true; //Enables raycasts
            canvasGroup.alpha = 1f; //No longer transparent
        }

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //Slot where item dropped
        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot == originalSlot)
        {
            transform.SetParent(originalParent);
            InventoryController.SetupItemForSlot(gameObject);
            return;
        }

        if (dropSlot != null)
        {
            //Is a slot under drop point
            if (dropSlot.currentItem != null)
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if(draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {
                    //Slot has an item - swap items
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    InventoryController.SetupItemForSlot(dropSlot.currentItem);

                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    InventoryController.SetupItemForSlot(gameObject);
                }
            }
            else
            {
                originalSlot.currentItem = null;
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
                InventoryController.SetupItemForSlot(gameObject);
            }
        }
        else
        {
            //No slot under drop point
            //If where we're dropping is not within the inventory
            if (!IsWithinInventory(eventData.position))
            {
                //Drop our item
                DropItem(originalSlot);
            }
            else
            {
                //Snap back to og slot
                transform.SetParent(originalParent);
                InventoryController.SetupItemForSlot(gameObject);
            }
        }
    }

    bool IsWithinInventory(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        int quantity = item.quantity;

        if(quantity > 1)
        {
            item.RemoveFromStack();

            transform.SetParent(originalParent);
            InventoryController.SetupItemForSlot(gameObject);

            quantity = 1;
        }
        else
        {
            originalSlot.currentItem = null;
        }

        //Find player
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if(playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag");
            return;
        }

        //Find ItemDictionary to get the original world item prefab
        ItemDictionary itemDictionary = FindAnyObjectByType<ItemDictionary>();
        if(itemDictionary == null)
        {
            Debug.LogError("ItemDictionary not found");
            return;
        }

        //Get the original world item prefab (not the UI version)
        GameObject worldItemPrefab = itemDictionary.GetItemPrefab(item.ID);
        if(worldItemPrefab == null)
        {
            Debug.LogError($"World item prefab not found for ID {item.ID}");
            return;
        }

        //Random drop position
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;

        //Instantiate the original world item prefab (not the UI version)
        GameObject dropItem = Instantiate(worldItemPrefab, dropPosition, Quaternion.identity);
        Item droppedItem = dropItem.GetComponent<Item>();
        if(droppedItem != null)
        {
            droppedItem.quantity = 1;
        }

        // Ensure the dropped item has proper world item components
        SetupWorldItem(dropItem);

        //Destroy the UI one
        if(quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);
        }
        InventoryController.Instance.RebuildItemCounts();
    }
    
    void SetupWorldItem(GameObject worldItem)
    {
        // Ensure the world item has the necessary components for pickup
        if(worldItem.GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = worldItem.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }
        
        // Ensure it has the "Item" tag for pickup detection
        if(!worldItem.CompareTag("Item"))
        {
            worldItem.tag = "Item";
        }
        
        // Remove UI components that shouldn't be on world items
        var images = worldItem.GetComponents<UnityEngine.UI.Image>();
        foreach(var image in images)
        {
            if(image != null)
            {
                DestroyImmediate(image);
            }
        }
        
        // Remove CanvasGroup if it exists (UI component)
        var canvasGroup = worldItem.GetComponent<CanvasGroup>();
        if(canvasGroup != null)
        {
            DestroyImmediate(canvasGroup);
        }
        
        // Enable SpriteRenderer for world display
        var spriteRenderer = worldItem.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        // Note: We don't remove RectTransform as Unity doesn't allow it
        // The world item will work fine with RectTransform present
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }
    }

    private void SplitStack()
    {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1) return;

        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount);

        GameObject newItem = item.CloneItem(splitAmount);

        if (inventoryController == null || newItem == null) return;

        foreach(Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                
                // Properly set up the RectTransform for UI inventory items
                InventoryController.SetupItemForSlot(newItem);
                
                return;
            }
        }

        //No empty slot - return to stack
        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}
