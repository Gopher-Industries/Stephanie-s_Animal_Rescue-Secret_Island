using UnityEngine;
using System.Collections;

public class FrogBehavior : MonoBehaviour
{
    // Movement settings
    public float jumpForce = 5.0f; // Force for jumping
    public float jumpIntervalMin = 1.0f; // Minimum time between jumps
    public float jumpIntervalMax = 3.0f; // Maximum time between jumps
    public float detectionRange = 7.0f; // Range to detect fetch objects or player

    // Tongue settings
    public float tongueSpeed = 15.0f; // Speed of the tongue
    public float tongueCooldown = 3.0f; // Cooldown between tongue attacks
    public float playerPullForce = 2.0f; // Force applied to pull the player closer

    // Tags
    public string fetchTag = "FetchObject"; // Tag for fetchable objects
    public string playerTag = "Player"; // Tag for the player

    // Components and state
    private Rigidbody frogRigidbody;
    private float nextJumpTime = 0.0f; // Timer for the next jump
    private float tongueTimer = 0.0f; // Timer for the tongue cooldown
    private GameObject player; // Player object
    private bool isJumping = false; // Whether the frog is currently jumping
    private bool isUsingTongue = false; // Whether the frog is using its tongue

    // Line Renderer for the tongue visualization
    private LineRenderer tongueRenderer;

    void Start()
    {
        // Ensure the frog has a Rigidbody
        frogRigidbody = GetComponent<Rigidbody>();
        if (frogRigidbody == null)
        {
            frogRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Find the player
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player has the correct tag.");
        }

        // Initialize the Line Renderer
        tongueRenderer = gameObject.AddComponent<LineRenderer>();
        tongueRenderer.startWidth = 0.1f;
        tongueRenderer.endWidth = 0.1f;
        tongueRenderer.material = new Material(Shader.Find("Unlit/Color"));
        tongueRenderer.material.color = Color.green;
        tongueRenderer.positionCount = 0; // Start with no line

        ScheduleNextJump();
    }

    void Update()
    {
        if (isJumping || isUsingTongue) return;

        // Handle random jumping
        if (Time.time >= nextJumpTime)
        {
            RandomJump();
        }

        // Handle tongue cooldown
        if (tongueTimer > 0)
        {
            tongueTimer -= Time.deltaTime;
        }

        // Detect nearby objects or player
        if (tongueTimer <= 0)
        {
            GameObject fetchObject = DetectFetchObject();
            if (fetchObject != null)
            {
                StartCoroutine(UseTongue(fetchObject));
                return;
            }

            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                StartCoroutine(UseTongue(player));
            }
        }
    }

    private void RandomJump()
    {
        isJumping = true;

        // Choose a random direction to jump
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 1.0f; // Ensure upward movement
        randomDirection.Normalize();

        // Apply jump force
        frogRigidbody.AddForce(randomDirection * jumpForce, ForceMode.Impulse);

        ScheduleNextJump();
    }

    private void ScheduleNextJump()
    {
        nextJumpTime = Time.time + Random.Range(jumpIntervalMin, jumpIntervalMax);
        isJumping = false;
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

    private IEnumerator UseTongue(GameObject target)
    {
        isUsingTongue = true;
        tongueTimer = tongueCooldown;

        Debug.Log("Frog is using its tongue!");

        // Simulate tongue extending
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.transform.position;

        tongueRenderer.positionCount = 2; // Activate the line
        tongueRenderer.SetPosition(0, startPosition);

        float elapsedTime = 0f;
        while (elapsedTime < 0.3f) // Simulate tongue reaching out
        {
            elapsedTime += Time.deltaTime * tongueSpeed;
            Vector3 currentTonguePosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            tongueRenderer.SetPosition(1, currentTonguePosition); // Update the tongue's end position
            yield return null;
        }

        if (target.CompareTag(fetchTag))
        {
            // "Eat" the fetch object
            Debug.Log("Frog ate the fetch object: " + target.name);
            Destroy(target);
        }
        else if (target.CompareTag(playerTag))
        {
            // Pull the player closer
            Debug.Log("Frog pulled the player closer!");
            Rigidbody playerRigidbody = target.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 pullDirection = (transform.position - target.transform.position).normalized;
                playerRigidbody.AddForce(pullDirection * playerPullForce, ForceMode.Impulse);
            }
        }

        // Reset the tongue
        tongueRenderer.positionCount = 0;
        isUsingTongue = false;
    }
}
