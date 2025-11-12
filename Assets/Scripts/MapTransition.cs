using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundary;
    CinemachineConfiner2D confiner;

    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;


    enum Direction { Up, Down, Left, Right, Teleport }

    private void Awake()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (confiner != null && mapBoundary != null)
            {
                confiner.BoundingShape2D = mapBoundary;
            }
            UpdatePlayerPosition(collision.gameObject);
            
            // Clean up duplicate EventSystems after area transition
            EventSystemManager.CleanupDuplicateEventSystems();
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if(direction == Direction.Teleport){
            player.transform.position = teleportTargetPosition.position;
            return;
        }
        Vector3 newPos = player.transform.position;
        switch (direction)
        {
            case Direction.Up:
                newPos.y += 3;
                break;
            case Direction.Down:
                newPos.y -= 3;
                break;
            case Direction.Left:
                newPos.x -= 3;
                break;
            case Direction.Right:
                newPos.x += 3;
                break;
        }
        player.transform.position = newPos;
    }

    // -----------------------------
    // Enhanced methods for dialogue integration
    // -----------------------------
    public void MovePlayerToWaypoint(Transform waypoint)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene!");
            return;
        }

        // Move player to waypoint
        player.transform.position = waypoint.position;

        // Update confiner if assigned
        if (confiner != null && mapBoundary != null)
        {
            confiner.BoundingShape2D = mapBoundary;
        }
    }

    // New method that works with the WaypointTeleportManager
    public void TeleportPlayerHere()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene!");
            return;
        }

        // Stop player movement
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        // Move player to this transition point
        player.transform.position = transform.position;

        // Update confiner
        if (confiner != null && mapBoundary != null)
        {
            Debug.Log($"Updating camera boundary to: {mapBoundary.name}");
            confiner.BoundingShape2D = mapBoundary;
        }
    }

    // Method that can be called from dialogue choices with waypoint name
    public void TeleportToThisArea(string waypointName = "")
    {
        Debug.Log($"Teleporting to area: {gameObject.name}" + (string.IsNullOrEmpty(waypointName) ? "" : $" (waypoint: {waypointName})"));
        TeleportPlayerHere();
    }
}