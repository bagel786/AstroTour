using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        //AutoIncrementIds
        for(int i = 0; i < itemPrefabs.Count; i++)
        {
            if(itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1;
            }
        }

        foreach(Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        if (itemID <= 0)
        {
            Debug.LogError($"ItemDictionary: Invalid itemID '{itemID}'. ItemID must be positive.");
            return null;
        }

        if (itemDictionary == null)
        {
            Debug.LogError("ItemDictionary: Dictionary not initialized. Make sure Awake() has been called.");
            return null;
        }

        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if (prefab == null)
        {
            Debug.LogError($"ItemDictionary: Item with ID {itemID} not found in dictionary. Available IDs: {string.Join(", ", itemDictionary.Keys)}");
        }
        else
        {
            // Validate that the prefab has an Item component
            Item itemComponent = prefab.GetComponent<Item>();
            if (itemComponent == null)
            {
                Debug.LogError($"ItemDictionary: Item prefab with ID {itemID} does not have an Item component");
                return null;
            }
        }
        
        return prefab;
    }

    /// <summary>
    /// Checks if an item with the given ID exists in the dictionary
    /// </summary>
    /// <param name="itemID">The item ID to check</param>
    /// <returns>True if the item exists, false otherwise</returns>
    public bool HasItem(int itemID)
    {
        if (itemID <= 0 || itemDictionary == null)
        {
            return false;
        }
        
        return itemDictionary.ContainsKey(itemID);
    }

    /// <summary>
    /// Gets all available item IDs in the dictionary
    /// </summary>
    /// <returns>Array of available item IDs</returns>
    public int[] GetAvailableItemIDs()
    {
        if (itemDictionary == null)
        {
            Debug.LogWarning("ItemDictionary: Dictionary not initialized");
            return new int[0];
        }

        int[] ids = new int[itemDictionary.Keys.Count];
        itemDictionary.Keys.CopyTo(ids, 0);
        return ids;
    }
}
