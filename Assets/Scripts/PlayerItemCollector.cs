using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InventoryController inventoryController;

    // Track recently processed items to avoid duplicate OnTriggerEnter events
    private readonly HashSet<int> recentlyProcessed = new();

    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            // OnTriggerEnter2D for item pickups
            int instanceId = collision.gameObject.GetInstanceID();
            if (recentlyProcessed.Contains(instanceId)) return;

            recentlyProcessed.Add(instanceId);
            StartCoroutine(RemoveProcessedIdAfterDelay(instanceId, 1f)); // keep id for 1 second

            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                // Use the item's Pickup method which handles inventory addition and UI notifications
                item.Pickup();
            }
        }
    }

    private IEnumerator RemoveProcessedIdAfterDelay(int id, float delay)
    {
        yield return new WaitForSeconds(delay);
        recentlyProcessed.Remove(id);
    }
}