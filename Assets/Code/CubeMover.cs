using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeMover : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the cube moves
    private Vector3 targetPosition; // The position to move towards
    private bool isMoving = false; // Whether the cube should be moving

    public Animator animator;
    private SavePlayerPos playerPosData;

    private SpriteRenderer spriteRenderer; //referencing sprite renderer

    
    private void Awake()
    {
        playerPosData = FindObjectOfType<SavePlayerPos>();
        playerPosData.PlayerPosLoad();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

            FlipSprite(targetPosition);

            // Optional: Draw a debug line from the cube to the target position
            Debug.DrawLine(transform.position, targetPosition, Color.green, 2f);

            Debug.Log($"Moving to Position: X={targetPosition.x}, Y={targetPosition.y}, Z={targetPosition.z}");
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any objects.");
        }
    }

    /// <summary>
    /// Moves the cube towards the target position.
    /// </summary>
    void HandleMovement()
    {
        if (!isMoving)
            return;

        // Keep the cube's Y position (assuming movement on the XZ plane)
        Vector3 currentPosition = transform.position;
        Vector3 destination = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);

        // Move towards the destination
        transform.position = Vector3.MoveTowards(currentPosition, destination, moveSpeed * Time.deltaTime);
        
        // Check if the cube has reached the destination
        if (Vector3.Distance(currentPosition, destination) < 0.1f)
        {
            isMoving = false;
            Debug.Log("Reached the destination.");
            playerPosData.PlayerPosSave();
        }

        animator.SetBool("IsMoving", isMoving);
        
    }

    /// <summary>
    /// Flip the 2D sprite based on the direction
    /// </summary>
    void FlipSprite(Vector3 targetPos)
    {
        if (spriteRenderer != null)
        {
            if (spriteRenderer !=null)
            {
                // If left of the player is clicked flip the sprite
                spriteRenderer.flipX = targetPos.x < transform.position.x;
            }
        }
    }
}
