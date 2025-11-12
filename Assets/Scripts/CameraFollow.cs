using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // Player transform
    public float smoothSpeed = 5f;   // How smoothly camera follows
    public Vector3 offset = new Vector3(0, 2, -10); // Manual offset (top-down feel)

    void LateUpdate()
    {
        if (target != null)
        {
            // Desired position = player position + offset
            Vector3 desiredPosition = target.position + offset;

            // Smooth movement
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
