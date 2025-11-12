using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class WaypointTeleportManager : MonoBehaviour
{
    public static WaypointTeleportManager Instance { get; private set; }
    
    private static Dictionary<string, WaypointRegistryEntry> waypointRegistry = new Dictionary<string, WaypointRegistryEntry>();
    
    [Header("Player Reference")]
    public Transform playerTransform;

    [Header("Cinemachine")]
    public CinemachineConfiner2D confiner;

    [Header("Waypoints")]
    public WaypointData[] waypoints;

    [Header("Teleport Settings")]
    public bool fadeOnTeleport = true;
    public float fadeDuration = 0.5f;

    [System.Serializable]
    public class WaypointRegistryEntry
    {
        public Vector3 position;
        public PolygonCollider2D mapBoundary;
        public bool setPlayerDirection;
        public Vector2 playerDirection;
        public string sourceManagerName;

        public WaypointRegistryEntry(Vector3 pos, PolygonCollider2D boundary, bool setDir, Vector2 dir, string sourceName)
        {
            position = pos;
            mapBoundary = boundary;
            setPlayerDirection = setDir;
            playerDirection = dir;
            sourceManagerName = sourceName;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AutoAssignPlayer();
        AutoAssignConfiner();
        RegisterWaypoints();
    }

    private void OnDestroy()
    {
        if (waypoints != null)
        {
            foreach (var waypoint in waypoints)
            {
                if (waypointRegistry.ContainsKey(waypoint.waypointName))
                {
                    var entry = waypointRegistry[waypoint.waypointName];
                    if (entry.sourceManagerName == gameObject.name)
                    {
                        waypointRegistry.Remove(waypoint.waypointName);
                    }
                }
            }
        }
    }

    private void RegisterWaypoints()
    {
        if (waypoints == null) return;

        foreach (var waypoint in waypoints)
        {
            RegisterWaypoint(waypoint);
        }
    }

    private void RegisterWaypoint(WaypointData waypoint)
    {
        if (string.IsNullOrEmpty(waypoint.waypointName))
            return;

        var entry = new WaypointRegistryEntry(
            waypoint.position,
            waypoint.mapBoundary,
            waypoint.setPlayerDirection,
            waypoint.playerDirection,
            gameObject.name
        );

        waypointRegistry[waypoint.waypointName] = entry;
    }

    public static bool HasWaypoint(string waypointName)
    {
        return waypointRegistry.ContainsKey(waypointName);
    }

    public static Vector3? GetWaypointPosition(string waypointName)
    {
        if (waypointRegistry.TryGetValue(waypointName, out WaypointRegistryEntry entry))
        {
            return entry.position;
        }
        return null;
    }

    public static void TeleportToRegisteredWaypoint(string waypointName)
    {
        if (Instance == null)
            return;

        if (waypointRegistry.TryGetValue(waypointName, out WaypointRegistryEntry entry))
        {
            Instance.PerformTeleportFromRegistry(waypointName, entry);
        }
    }

    public static Dictionary<string, Vector3> GetAllWaypointPositions()
    {
        var positions = new Dictionary<string, Vector3>();
        foreach (var kvp in waypointRegistry)
        {
            positions[kvp.Key] = kvp.Value.position;
        }
        return positions;
    }

    private void AutoAssignPlayer()
    {
        if (playerTransform != null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void AutoAssignConfiner()
    {
        if (confiner != null)
            return;

        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    public void TeleportToWaypoint(string waypointName)
    {
        TeleportToRegisteredWaypoint(waypointName);
    }

    private void PerformTeleportFromRegistry(string waypointName, WaypointRegistryEntry entry)
    {
        if (playerTransform == null)
        {
            AutoAssignPlayer();
            if (playerTransform == null) return;
        }

        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        playerTransform.position = entry.position;

        if (confiner != null && entry.mapBoundary != null)
        {
            confiner.BoundingShape2D = entry.mapBoundary;
            confiner.InvalidateBoundingShapeCache();
        }

        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.Follow = playerTransform;
            vcam.PreviousStateIsValid = false;
        }

        if (entry.setPlayerDirection)
        {
            Animator animator = playerTransform.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("Look X", entry.playerDirection.x);
                animator.SetFloat("Look Y", entry.playerDirection.y);
            }
        }

        SnapCameraToPlayer();
    }

    public void SnapCameraToPlayer()
    {
        if (playerTransform == null)
            return;

        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            Vector3 newCameraPos = new Vector3(playerTransform.position.x, playerTransform.position.y, vcam.transform.position.z);

            vcam.enabled = false;
            vcam.transform.position = newCameraPos;
            vcam.enabled = true;
            vcam.PreviousStateIsValid = false;
        }
    }

    [ContextMenu("Test Teleport to First Waypoint")]
    public void TestTeleportToFirst()
    {
        if (waypoints.Length > 0)
            TeleportToWaypoint(waypoints[0].waypointName);
    }
}

[System.Serializable]
public class WaypointData
{
    [Header("Basic Info")]
    public string waypointName;

    [Header("Position")]
    public Vector3 position;

    [Header("Cinemachine Camera Boundary")]
    public PolygonCollider2D mapBoundary;

    [Header("Player Direction (Optional)")]
    public bool setPlayerDirection = false;
    public Vector2 playerDirection = Vector2.down;
}
