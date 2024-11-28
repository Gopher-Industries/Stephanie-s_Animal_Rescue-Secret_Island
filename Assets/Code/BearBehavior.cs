using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BearBehavior : MonoBehaviour
{
    // Movement settings
    public float moveSpeed = 2.0f; // Movement speed
    public float jumpForce = 5.0f; // Force for jumping towards the target
    public float upwardJumpForce = 7.0f; // Upward force when jumping into the air
    public float wanderRadius = 10.0f; // Radius within which the bear can wander
    public float jumpProximity = 3.0f; // Distance at which the bear jumps toward the target
    public float launchDistance = 1.0f; // Distance at which the bear jumps up and launches away

    // Idle behavior
    public float idleDuration = 2.0f; // How long the bear stays idle
    public float idleChance = 0.3f; // Chance to idle during wandering

    // Targeting settings
    public string[] targetTags = { "Tree", "Player" }; // Tags for valid targets
    public float forgetTargetTime = 5.0f; // Time before the bear forgets its target
    private float targetTimer = 0f; // Timer for forgetting the target

    // State control
    private bool isIdle = false; // Whether the bear is idle
    private Vector3 targetPosition; // Target position for wandering
    private GameObject currentTarget; // Current target object
    private float idleTimer = 0.0f; // Timer for idle duration
    private bool isJumping = false; // Whether the bear is currently jumping

    // Rigidbody for physics-based jumping
    private Rigidbody bearRigidbody;

    // Line Renderer for visualizing the path
    private LineRenderer lineRenderer;

    void Start()
    {
        // Ensure the bear has a Rigidbody
        bearRigidbody = GetComponent<Rigidbody>();
        if (bearRigidbody == null)
        {
            bearRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Initialize the Line Renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
        lineRenderer.positionCount = 0;

        // Set the initial target position
        SetRandomTargetPosition();
    }

    void Update()
    {
        if (isJumping) return;

        // Timer to forget the target after a set time
        targetTimer += Time.deltaTime;
        if (targetTimer >= forgetTargetTime)
        {
            SetRandomTargetPosition(); // Choose a new target
            targetTimer = 0f; // Reset the timer
        }

        if (isIdle)
        {
            // Count down the idle timer
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                // Stop idling and resume wandering
                isIdle = false;
                SetRandomTargetPosition();
            }
        }
        else
        {
            HandleMovement();
        }

        // Update the line renderer to visualize the target
        UpdateTargetPath();
    }

    private void HandleMovement()
    {
        if (currentTarget != null)
        {
            // Calculate the distance to the target
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distanceToTarget <= jumpProximity && !isJumping)
            {
                JumpTowardsTarget();
            }
            else if (distanceToTarget <= launchDistance && !isJumping)
            {
                LaunchAwayFromTarget();
            }
            else
            {
                MoveTowardsTarget();
            }
        }
        else
        {
            MoveTowardsTarget(); // Continue wandering if no specific target
        }
    }

    private void MoveTowardsTarget()
    {
        // Calculate the direction to the target or random position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Ignore the Y component to keep movement horizontal
        direction.y = 0;

        // Move forward in the target direction
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void JumpTowardsTarget()
    {
        if (currentTarget == null) return;

        isJumping = true;

        // Calculate jump direction
        Vector3 jumpDirection = (currentTarget.transform.position - transform.position).normalized;
        jumpDirection.y = 0.5f; // Add upward force for jumping

        // Apply jump force
        bearRigidbody.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

        // Reset jump state after a short delay
        Invoke(nameof(ResetJumpState), 1.0f);
    }

    private void LaunchAwayFromTarget()
    {
        isJumping = true;

        // Apply a random upward and directional force
        Vector3 launchDirection = Random.insideUnitSphere;
        launchDirection.y = 1.0f; // Ensure upward force

        bearRigidbody.AddForce(launchDirection.normalized * upwardJumpForce, ForceMode.Impulse);

        // Reset jump state after a short delay
        Invoke(nameof(ResetJumpState), 1.0f);
    }

    private void ResetJumpState()
    {
        isJumping = false;
        SetRandomTargetPosition();
    }

    private void SetRandomTargetPosition()
    {
        // Find the 5 closest objects with valid tags
        GameObject[] potentialTargets = FindClosestObjects(targetTags, 5);

        if (potentialTargets.Length > 0)
        {
            // Pick a random target from the closest objects
            currentTarget = potentialTargets[Random.Range(0, potentialTargets.Length)];
            targetPosition = currentTarget.transform.position;
        }
        else
        {
            // If no valid targets are found, pick a random position within the wander radius
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0; // Keep the movement on the same plane
            targetPosition = transform.position + randomDirection;
            currentTarget = null;
        }
    }

    private GameObject[] FindClosestObjects(string[] tags, int maxCount)
    {
        // Collect all objects with the specified tags
        List<GameObject> foundObjects = new List<GameObject>();
        foreach (string tag in tags)
        {
            foundObjects.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        // Sort by distance and take the closest ones
        return foundObjects
            .OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position))
            .Take(maxCount)
            .ToArray();
    }

    private void UpdateTargetPath()
    {
        if (currentTarget != null)
        {
            // Enable and update the line renderer to point to the target
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position); // Start at the bear
            lineRenderer.SetPosition(1, currentTarget.transform.position); // End at the target
        }
        else
        {
            // Clear the line renderer if no target
            lineRenderer.positionCount = 0;
        }
    }
}
