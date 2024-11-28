using UnityEngine;
using System.Collections;

public class DogBehavior : MonoBehaviour
{
    // Movement settings
    public float followSpeed = 3.0f; // Speed for following the player
    public float dashSpeed = 7.0f; // Speed for dashing
    public float followDuration = 5.0f; // Time to follow the player before dashing
    public float detectionRange = 10.0f; // Range to detect fetch objects
    public float fetchProximity = 1.0f; // Distance to fetch object
    public float dashDuration = 1.0f; // Time spent dashing

    // Tags
    public string playerTag = "Player"; // Tag for the player
    public string fetchTag = "FetchObject"; // Tag for objects to fetch

    // State control
    private bool isDashing = false; // Whether the dog is dashing
    private bool isFetching = false; // Whether the dog is fetching
    private GameObject fetchTarget; // Current fetch object
    private GameObject player; // Player GameObject
    private Vector3 dashDirection; // Direction for dashing
    private float followTimer = 0.0f; // Timer to track follow duration

    void Start()
    {
        // Find the player GameObject
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player has the correct tag.");
        }
    }

    void Update()
    {
        if (isDashing || isFetching) return;

        // Follow the player for a set duration
        followTimer += Time.deltaTime;
        if (followTimer >= followDuration)
        {
            DashBehavior();
            return;
        }

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > 1.5f) // Maintain a small distance from the player
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            MoveInDirection(direction, followSpeed);
        }
    }

    private void DashBehavior()
    {
        isDashing = true;
        fetchTarget = DetectFetchObject();

        if (fetchTarget != null)
        {
            Debug.Log("Dog detected a fetch object and is dashing to it.");
            dashDirection = (fetchTarget.transform.position - transform.position).normalized;
        }
        else
        {
            // Dash in a random direction if no fetch object is nearby
            Debug.Log("Dog is dashing in a random direction.");
            dashDirection = Random.insideUnitSphere;
            dashDirection.y = 0; // Keep the dash on the ground
        }

        // Perform the dash
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        float elapsedTime = 0;

        while (elapsedTime < dashDuration)
        {
            MoveInDirection(dashDirection, dashSpeed);
            elapsedTime += Time.deltaTime;

            // Check if the dog is close enough to fetch the object
            if (fetchTarget != null && Vector3.Distance(transform.position, fetchTarget.transform.position) <= fetchProximity)
            {
                FetchObject();
                yield break; // Stop dashing and start fetching
            }

            yield return null;
        }

        isDashing = false;

        // If no object was fetched, dash back to the player
        if (!isFetching)
        {
            Debug.Log("Dog is dashing back to the player.");
            dashDirection = (player.transform.position - transform.position).normalized;
            StartCoroutine(DashBackToPlayer());
        }
    }

    private IEnumerator DashBackToPlayer()
    {
        float elapsedTime = 0;

        while (elapsedTime < dashDuration)
        {
            MoveInDirection(dashDirection, dashSpeed);
            elapsedTime += Time.deltaTime;

            // If close enough to the player, stop dashing
            if (Vector3.Distance(transform.position, player.transform.position) <= 1.5f)
            {
                isDashing = false;
                followTimer = 0.0f; // Reset the follow timer
                yield break;
            }

            yield return null;
        }

        isDashing = false;
        followTimer = 0.0f; // Reset the follow timer
    }

    private void FetchObject()
    {
        isFetching = true;
        Debug.Log("Dog fetched the object: " + fetchTarget.name);

        // Simulate carrying the object back to the player
        fetchTarget.transform.position = player.transform.position + Vector3.forward; // Drop the object near the player
        fetchTarget = null; // Reset the fetch target
        isFetching = false;

        followTimer = 0.0f; // Reset the follow timer
    }

    private GameObject DetectFetchObject()
    {
        GameObject[] fetchObjects = GameObject.FindGameObjectsWithTag(fetchTag);

        foreach (GameObject obj in fetchObjects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) <= detectionRange)
            {
                return obj;
            }
        }

        return null;
    }

    private void MoveInDirection(Vector3 direction, float speed)
    {
        direction.y = 0; // Keep movement horizontal
        transform.position += direction * speed * Time.deltaTime;
    }
}
