using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMover : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the cube moves
    private Vector3 targetPosition; // The position to move towards
    private bool isMoving = false; // Whether the cube should be moving

    // Referencing CharacterOrientation
    private CharacterOrientation characterOrientation;
    SavePlayerPos playerPosData;
    public Animator animator;

    static public bool dialogue = false;

    private void Awake()
    {
        // Get characterOrientation component
        characterOrientation = GetComponent<CharacterOrientation>();
        // PlayerPosData = FindObjectOfType<SavePlayerPos>();    //Obsolete
        playerPosData = FindFirstObjectByType<SavePlayerPos>();

        playerPosData.PlayerPosLoad();
    }

    void Update()
    {
        // Stop character movement when in dialogue
        if (dialogue)
        {
            isMoving = false;
            animator.SetBool("IsMoving", false);
            return;
        }

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

        if(characterOrientation !=null && isMoving)
        {
            characterOrientation.updateOrientation(targetPosition, moveSpeed);
        }

        // Check if the cube has reached the destination
        if (Vector3.Distance(currentPosition, destination) < 0.1f)
        {
            isMoving = false;
            Debug.Log("Reached the destination.");
            //playerPosData.PlayerPosSave();
        }

        animator.SetBool("IsMoving", isMoving);
    }
}