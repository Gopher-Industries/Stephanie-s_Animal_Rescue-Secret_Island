using UnityEngine;

public class ChickBehavior : MonoBehaviour
{
    // Movement settings
    public float dashSpeed = 15.0f; // Speed for dashing
    public float dashDuration = 10.5f; // How long the dash lasts
    public float jumpForce = 17.0f; // Force for jumping after dashing
    public float floatDownSpeed = 11.5f; // Speed for floating gently down
    public float dashIntervalMin = 12.0f; // Minimum time between dashes
    public float dashIntervalMax = 15.0f; // Maximum time between dashes

    // State control
    private bool isDashing = false; // Whether the chick is dashing
    private bool isJumping = false; // Whether the chick is jumping
    private Vector3 dashDirection; // Direction of the current dash
    private Rigidbody chickRigidbody; // Rigidbody for physics

    private float nextDashTime; // Timer to control when the next dash happens

    void Start()
    {
        // Ensure the chick has a Rigidbody
        chickRigidbody = GetComponent<Rigidbody>();
        if (chickRigidbody == null)
        {
            chickRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Schedule the first dash
        ScheduleNextDash();
    }

    void Update()
    {
        // Handle dashing
        if (isDashing)
        {
            HandleDashing();
            return;
        }

        // Handle floating down after jumping
        if (isJumping)
        {
            HandleFloating();
            return;
        }

        // Check if it's time to dash
        if (Time.time >= nextDashTime)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;

        // Choose a random direction to dash
        dashDirection = Random.insideUnitSphere;
        dashDirection.y = 0; // Keep the movement on the ground
        dashDirection.Normalize();

        // Set the dash duration timer
        Invoke(nameof(EndDash), dashDuration);

        Debug.Log("Chick is dashing!");
    }

    private void HandleDashing()
    {
        // Move the chick in the dash direction
        transform.position += dashDirection * dashSpeed * Time.deltaTime;
    }

    private void EndDash()
    {
        isDashing = false;

        // Start jumping immediately after the dash
        StartJumping();
    }

    private void StartJumping()
    {
        isJumping = true;

        // Add upward force for the jump
        chickRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Chick is jumping after dash!");
    }

    private void HandleFloating()
    {
        // Gradually slow the descent to simulate floating
        if (chickRigidbody.velocity.y < 0)
        {
            chickRigidbody.velocity = new Vector3(chickRigidbody.velocity.x, -floatDownSpeed, chickRigidbody.velocity.z);
        }

        // Check if the chick has landed
        if (chickRigidbody.velocity.y == 0)
        {
            isJumping = false;
            Debug.Log("Chick has landed.");

            // Schedule the next dash
            ScheduleNextDash();
        }
    }

    private void ScheduleNextDash()
    {
        // Randomly decide when the next dash will happen
        nextDashTime = Time.time + Random.Range(dashIntervalMin, dashIntervalMax);
        Debug.Log("Next dash scheduled in: " + (nextDashTime - Time.time) + " seconds.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop jumping if the chick hits the ground or an object
        if (collision.contacts[0].normal.y > 0.9f)
        {
            isJumping = false;
            Debug.Log("Chick landed from collision.");
        }
    }
}
