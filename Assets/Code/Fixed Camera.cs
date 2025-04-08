using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    public Transform player; 
    private float distance = 10f; // Distance camera is from player
    private float height = 5f; // How heigh camera is
    private float angle = 15f; // Angle at which the camera looks down at the player (in degrees)

    void LateUpdate(){
        // Get the target position for the camera
        Vector3 targetPosition = new Vector3(player.position.x, height, player.position.z - distance);

        // move the camera to the target position to follow player 
        transform.position = Vector3.Lerp(transform.position, targetPosition, 1);

        // Make the camera look down at the character at a specific angle
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }
}