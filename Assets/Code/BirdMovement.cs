using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    // Center point for the circular movement
    public Vector3 centerPoint = new Vector3(0, 45, 0);

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

    void Update()
    {
        // Update the angle based on speed
        currentAngle += speed * Time.deltaTime;

        // Calculate the new position on the circular path
        float x = centerPoint.x + Mathf.Cos(currentAngle) * radius;
        float z = centerPoint.z + Mathf.Sin(currentAngle) * radius;

        // Add flapping motion to the y position
        float y = groundHeight + Mathf.Sin(Time.time * flapFrequency) * flapAmplitude;

        // Set the new position
        transform.position = new Vector3(x, y, z);
    }

    private void OnDrawGizmos()
    {
        // Draw the circular path in the editor for visualization
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPoint, radius);
    }
}
