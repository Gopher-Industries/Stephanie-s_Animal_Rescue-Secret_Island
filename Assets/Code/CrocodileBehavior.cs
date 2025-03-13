using UnityEngine;

public class CrocodileBehavior : MonoBehaviour
{
    // Movement settings
    public float walkSpeed = 1.5f; // Speed for patrolling
    public float lungeSpeed = 8.0f; // Speed for lunging at a target
    public float lungeDuration = 1.0f; // Duration of the lunge
    public float patrolRadius = 5.0f; // Radius for patrolling when idle

    // Target detection
    public string targetTag = "Player"; // Tag to identify targets
    public float ambushRange = 5.0f; // Range to detect and lunge at targets

    // Resting behavior
    public float restChance = 0.1f; // Chance per second to rest
    public float restDuration = 3.0f; // Duration of resting

    // State control
    private bool isLunging = false; // Whether the crocodile is lunging
    private bool isResting = false; // Whether the crocodile is resting
    private Vector3 patrolTarget; // Target position for patrolling
    private Rigidbody crocodileRigidbody; // Rigidbody for movement

    void Start()
    {
        // Ensure the crocodile has a Rigidbody for physics
        crocodileRigidbody = GetComponent<Rigidbody>();
        if (crocodileRigidbody == null)
        {
            crocodileRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Set the first patrol target
        SetNewPatrolTarget();
    }

    void Update()
    {
        if (isLunging || isResting) return;

        // Check for nearby targets to ambush
        GameObject target = DetectTarget();
        if (target != null)
        {
            StartLunge(target);
            return;
        }

        // Randomly decide to rest
        if (Random.value < restChance * Time.deltaTime)
        {
            StartResting();
            return;
        }

        // Patrol around when idle
        Patrol();
    }

    private GameObject DetectTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject target in potentialTargets)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= ambushRange)
            {
                Debug.Log("Target detected: " + target.name);
                return target;
            }
        }
        return null;
    }

    private void StartLunge(GameObject target)
    {
        isLunging = true;

        // Calculate lunge direction
        Vector3 lungeDirection = (target.transform.position - transform.position).normalized;

        // Lunge towards the target
        crocodileRigidbody.linearVelocity = lungeDirection * lungeSpeed;
        Debug.Log("Crocodile lunging at target!");

        // End lunge after the duration
        Invoke(nameof(EndLunge), lungeDuration);
    }

    private void EndLunge()
    {
        isLunging = false;
        crocodileRigidbody.linearVelocity = Vector3.zero; // Stop moving after lunge
        Debug.Log("Crocodile ended lunge.");
    }

    private void StartResting()
    {
        isResting = true;
        crocodileRigidbody.linearVelocity = Vector3.zero; // Stop movement during rest
        Debug.Log("Crocodile is resting.");
        Invoke(nameof(EndResting), restDuration);
    }

    private void EndResting()
    {
        isResting = false;
        Debug.Log("Crocodile finished resting.");
        SetNewPatrolTarget(); // Resume patrolling after resting
    }

    private void Patrol()
    {
        // Move towards the patrol target
        Vector3 direction = (patrolTarget - transform.position).normalized;
        direction.y = 0; // Keep movement horizontal
        transform.position += direction * walkSpeed * Time.deltaTime;

        // If close to the patrol target, set a new one
        if (Vector3.Distance(transform.position, patrolTarget) < 1.0f)
        {
            SetNewPatrolTarget();
        }
    }

    private void SetNewPatrolTarget()
    {
        // Choose a random position within the patrol radius
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0; // Keep the target on the ground level
        patrolTarget = transform.position + randomDirection;

        Debug.Log("New patrol target set: " + patrolTarget);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop lunging if the crocodile hits something
        if (isLunging)
        {
            EndLunge();
        }
    }
}
