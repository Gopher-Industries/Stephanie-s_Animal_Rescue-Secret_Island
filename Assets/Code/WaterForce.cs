using UnityEngine;

public class WaterForce : MonoBehaviour
{
    public float upwardForce = 10f; // Adjust this to control the strength of the water force.
    private Rigidbody rb;

    private void Start()
    {
        // Cache the Rigidbody component.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("WaterForce: No Rigidbody found on the player!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object the player collided with is tagged "Water."
        if (other.CompareTag("Water") && rb != null)
        {
            // Apply an upward force to the player's Rigidbody.
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && rb != null)
        {
            Debug.Log("Player entered water zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water") && rb != null)
        {
            Debug.Log("Player exited water zone.");
        }
    }
}
