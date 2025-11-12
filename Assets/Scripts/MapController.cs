using UnityEngine;
using Unity.Cinemachine; // Needed for CinemachineConfiner2D

public class MapController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineConfiner2D confiner; // Reference to your confiner

    public void TeleportTo(Transform destination, PolygonCollider2D newBoundary)
    {
        if (player != null && destination != null)
        {
            // Move player
            player.position = destination.position;

            // Reset velocity if using Rigidbody2D
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // Update camera boundary
            if (confiner != null && newBoundary != null)
            {
                confiner.BoundingShape2D = newBoundary;
                confiner.InvalidateBoundingShapeCache(); // Force confiner to recalc path
            }

            Debug.Log($"Teleported player to {destination.name} with new boundary {newBoundary.name}");
        }
    }
}
