using UnityEngine;

public class UniversalTeleporter : MonoBehaviour
{
    [Header("Teleport Target Coordinates")]
    public float targetX; // X coordinate for teleport
    public float targetY; // Y coordinate for teleport
    public float targetZ; // Z coordinate for teleport

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding is tagged as "Player" or any tag you want
        if (other.CompareTag("Player"))
        {
            // Set the player's position to the target coordinates
            other.transform.position = new Vector3(targetX, targetY, targetZ);

            // Log the teleportation for debugging
            Debug.Log($"Teleported {other.name} to: ({targetX}, {targetY}, {targetZ})");
        }
    }
}
