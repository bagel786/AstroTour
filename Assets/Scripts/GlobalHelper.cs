using UnityEngine;

public static class GlobalHelper 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static string GeneratUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}"; // box_3_4 - unique id
    }  
}
