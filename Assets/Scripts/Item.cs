using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int ID;
    public string itemName;
    public int quantity = 1;
    private TMP_Text quantityText;
    public void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }
    public virtual void UseItem()
    {
        Debug.Log("Using item: " + itemName);
    }
    public virtual void Pickup()
    {
        // Handle the pickup logic here to allow for item-specific behavior
        InventoryController inventoryController = InventoryController.Instance;
        if (inventoryController != null)
        {
            bool wasAdded = inventoryController.AddItem(gameObject, false); // false = don't show UI notification in AddItem
            
            if (wasAdded)
            {
                // Show pickup UI notification here (single location, no duplicates)
                if (ItemPickupUIController.Instance != null)
                {
                    Sprite icon = GetComponent<SpriteRenderer>()?.sprite;
                    ItemPickupUIController.Instance.ShowItemPickup(itemName, icon);
                }
                
                // Allow subclasses to override pickup behavior
                OnPickupSuccess();
                
                // Destroy the world item
                Destroy(gameObject);
            }
        }
    }
    
    // Virtual method that subclasses can override for custom pickup behavior
    protected virtual void OnPickupSuccess()
    {
        // Default implementation does nothing
        // Subclasses can override this for special pickup effects, sounds, etc.
    }
    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }
    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }
    public int RemoveFromStack(int amount = 1)
    {
        int removedAmount = Mathf.Min(amount, quantity);
        quantity -= removedAmount;
        UpdateQuantityDisplay();
        return removedAmount;
    }
    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }


}
