using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuffaloBehavior : MonoBehaviour
{
    // Movement settings
    public float moveSpeed = 3.0f;
    public float chargeSpeed = 8.0f;
    public float herdFollowDistance = 5.0f;
    public float wanderRadius = 15.0f;

    // Targeting and state
    public string threatTag = "Player"; // Tag to identify threats
    public float threatDetectionRange = 10.0f; // Distance to detect threats
    public float chargeCooldown = 5.0f; // Cooldown time before charging again
    private bool isCharging = false;
    private float chargeTimer = 0f;

    // Herd behavior
    public string herdTag = "Buffalo"; // Tag for other buffalo
    private GameObject herdLeader;
    private bool isInHerd = false;

    // Idle behavior
    public float idleChance = 0.2f; // Chance to idle
    public float idleDuration = 2.0f;
    private bool isIdle = false;
    private float idleTimer = 0f;

    // Rigidbody for physics
    private Rigidbody buffaloRigidbody;

    void Start()
    {
        buffaloRigidbody = GetComponent<Rigidbody>();
        if (buffaloRigidbody == null)
        {
            buffaloRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        FindHerdLeader();
    }

    void Update()
    {
        if (isCharging)
        {
            // Handle charging cooldown
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                isCharging = false;
            }
        }

        if (isIdle)
        {
            // Handle idle behavior
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false;
            }
            return;
        }

        // Check for threats
        GameObject threat = DetectThreat();
        if (threat != null)
        {
            ChargeAtThreat(threat);
        }
        else
        {
            if (isInHerd && herdLeader != null)
            {
                FollowHerdLeader();
            }
            else
            {
                Wander();
            }
        }
    }

    private void FindHerdLeader()
    {
        // Find the closest buffalo to act as the leader
        GameObject[] herdMates = GameObject.FindGameObjectsWithTag(herdTag)
            .Where(b => b != this.gameObject).ToArray();

        if (herdMates.Length > 0)
        {
            herdLeader = herdMates[Random.Range(0, herdMates.Length)];
            isInHerd = true;
        }
    }

    private void FollowHerdLeader()
    {
        float distanceToLeader = Vector3.Distance(transform.position, herdLeader.transform.position);

        if (distanceToLeader > herdFollowDistance)
        {
            // Move towards the leader
            Vector3 direction = (herdLeader.transform.position - transform.position).normalized;
            MoveInDirection(direction);
        }
        else
        {
            // Occasionally idle when close to the leader
            if (Random.value < idleChance)
            {
                EnterIdleState();
            }
        }
    }

    private void ChargeAtThreat(GameObject threat)
    {
        if (isCharging) return;

        isCharging = true;
        chargeTimer = chargeCooldown;

        // Calculate charge direction
        Vector3 chargeDirection = (threat.transform.position - transform.position).normalized;

        // Apply force for the charge
        buffaloRigidbody.AddForce(chargeDirection * chargeSpeed, ForceMode.Impulse);
    }

    private GameObject DetectThreat()
    {
        GameObject[] potentialThreats = GameObject.FindGameObjectsWithTag(threatTag);
        foreach (GameObject threat in potentialThreats)
        {
            if (Vector3.Distance(transform.position, threat.transform.position) <= threatDetectionRange)
            {
                return threat;
            }
        }
        return null;
    }

    private void Wander()
    {
        if (Random.value < idleChance)
        {
            EnterIdleState();
            return;
        }

        // Move to a random position within the wander radius
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection.y = 0; // Keep on the ground
        Vector3 wanderTarget = transform.position + randomDirection;

        Vector3 direction = (wanderTarget - transform.position).normalized;
        MoveInDirection(direction);
    }

    private void MoveInDirection(Vector3 direction)
    {
        direction.y = 0; // Ignore vertical movement
        transform.position += direction * moveSpeed * Time.deltaTime;
        
    }

    private void EnterIdleState()
    {
        isIdle = true;
        idleTimer = idleDuration;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop charging if the buffalo hits something
        if (isCharging)
        {
            isCharging = false;
        }
    }
}
