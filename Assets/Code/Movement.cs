using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the character moves
    private Vector3 targetPosition; // The position to move towards
    private bool isMoving = false; // Whether the character should be moving

    public Animator animator;
    private SavePlayerPos playerPosData;

    private void Awake()
    {
        playerPosData = FindObjectOfType<SavePlayerPos>();
        playerPosData.PlayerPosLoad();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    /// <summary>
    /// Handles user input to set the movement destination.
    /// </summary>
    void HandleInput()
    {
        // Check if the left mouse button was clicked
        if (Input.GetMouseButtonDown(0))
        {
            SetDestination(Input.mousePosition);
        }

        // For touch input (mobile devices)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                SetDestination(touch.position);
            }
        }
    }

    /// <summary>
    /// Sets the target position based on the clicked point.
    /// </summary>
    void SetDestination(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // Perform the raycast without a layer mask to hit any collider
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
            isMoving = true;

            // Optional: Draw a debug line from the player to the target position
            Debug.DrawLine(transform.position, targetPosition, Color.green, 2f);

            Debug.Log($"Moving to Position: X={targetPosition.x}, Y={targetPosition.y}, Z={targetPosition.z}");
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any objects.");
        }
    }

    /// <summary>
    /// Moves the character towards the target position.
    /// </summary>
    void HandleMovement()
    {
        if (!isMoving)
            return;

        // Keep the character's Y position (assuming movement on the XZ plane)
        Vector3 currentPosition = transform.position;
        Vector3 destination = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);

        // Rotate to face the movement direction
        RotateToFace(destination);

        // Move towards the destination
        transform.position = Vector3.MoveTowards(currentPosition, destination, moveSpeed * Time.deltaTime);
        
        // Check if the character has reached the destination
        if (Vector3.Distance(currentPosition, destination) < 0.1f)
        {
            isMoving = false;
            Debug.Log("Reached the destination.");
            playerPosData.PlayerPosSave();
        }

        // Update the animator to reflect whether the character is moving
        animator.SetBool("IsMoving", isMoving);
    }

    /// <summary>
    /// Rotates the character to face the direction of movement on the Y-axis.
    /// </summary>
    void RotateToFace(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0; // Keep the rotation only on the Y-axis

        if (direction.magnitude > 0.1f) // If there's a significant direction to rotate
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }
}
