using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    // Speed at which the character moves
    public float moveSpeed = 5f;

    void Update()
    {
        // Get input from keyboard (WASD or arrow keys)
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Create a direction vector based on input
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        // Move the character in the direction input
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
}
