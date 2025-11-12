using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{

    public bool isOpened { get; private set; }
    public string boxID { get; private set; }
    
    [Header("Item Configuration")]
    [Tooltip("Optional: Item prefab to drop when box is opened. Leave empty for boxes that don't contain items.")]
    public GameObject itemPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxID ??= GlobalHelper.GeneratUniqueID(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool canInteract()
    {
        return !isOpened;
    }

    public void Interact() 
    {
        if (!canInteract()) return;
        OpenBox();
    }
    private void OpenBox()
    {
        // Set opened state
        SetOpened(true);
        
        // Drop item if one is configured
        if (itemPrefab != null)
        {
            try
            {
                GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity); // quaternion.identity = no rotation
                
                // Validate the dropped item has required components
                if (droppedItem != null)
                {
                    Item itemComponent = droppedItem.GetComponent<Item>();
                    if (itemComponent != null)
                    {
                        Debug.Log($"Box '{boxID}': Successfully dropped item '{itemComponent.itemName}'");
                    }
                    else
                    {
                        Debug.LogWarning($"Box '{boxID}': Dropped item prefab does not have an Item component");
                    }
                }
                else
                {
                    Debug.LogError($"Box '{boxID}': Failed to instantiate item prefab");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Box '{boxID}': Exception occurred while dropping item: {ex.Message}");
            }
        }
        else
        {
            Debug.Log($"Box '{boxID}': No item prefab configured - box opened without dropping an item");
        }
    }
    public void SetOpened(bool opened)
    {
        bool previous = isOpened;
        isOpened = opened;
        // change sprite if you have one here
    }

    /// <summary>
    /// Validates the box configuration for debugging purposes
    /// </summary>
    /// <returns>True if configuration is valid, false if there are issues</returns>
    public bool ValidateConfiguration()
    {
        bool isValid = true;

        // Check if boxID is properly set
        if (string.IsNullOrEmpty(boxID))
        {
            Debug.LogWarning($"Box at {transform.position}: boxID is null or empty");
            isValid = false;
        }

        // Validate item prefab if it's set
        if (itemPrefab != null)
        {
            Item itemComponent = itemPrefab.GetComponent<Item>();
            if (itemComponent == null)
            {
                Debug.LogError($"Box '{boxID}': Item prefab does not have an Item component");
                isValid = false;
            }
            else
            {
                Debug.Log($"Box '{boxID}': Configuration valid - will drop '{itemComponent.itemName}' when opened");
            }
        }
        else
        {
            Debug.Log($"Box '{boxID}': No item prefab configured - this box will not drop any items");
        }

        return isValid;
    }
}
