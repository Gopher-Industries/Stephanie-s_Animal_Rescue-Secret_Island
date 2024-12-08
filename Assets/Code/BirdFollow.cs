using UnityEngine;

public class BirdFollow : MonoBehaviour
{
    // Radius of the circular path
    public float radius = 5.0f;

    // Speed of the bird movement
    public float speed = 2.0f;

    // Height from the ground
    public float groundHeight = 2.0f;

    // Flapping motion variables
    public float flapAmplitude = 0.5f; // How much it moves up and down
    public float flapFrequency = 2.0f; // How fast it flaps

    // The current angle of movement
    private float currentAngle = 0f;

    // Reference to the player GameObject
    private GameObject player;

    void Start()
    {
        // Find the player GameObject by tag
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No GameObject with the tag 'Player' found!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Update the angle based on speed
            currentAngle += speed * Time.deltaTime;

            // Get the player's position
            Vector3 centerPoint = player.transform.position;

            // Calculate the new position on the circular path
            float x = centerPoint.x + Mathf.Cos(currentAngle) * radius;
            float z = centerPoint.z + Mathf.Sin(currentAngle) * radius;

            // Add flapping motion to the y position
            float y = centerPoint.y + groundHeight + Mathf.Sin(Time.time * flapFrequency) * flapAmplitude;

            // Set the new position
            transform.position = new Vector3(x, y, z);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the circular path in the editor for visualization
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, radius);
        }
    }
}
