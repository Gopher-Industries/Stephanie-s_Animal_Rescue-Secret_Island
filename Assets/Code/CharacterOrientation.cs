using UnityEngine;

public class CharacterOrientation : MonoBehaviour
{
    // Default as 2D character
    public bool is2D = true;

    // Reference SpriteRenderere and Model Transform components
    private SpriteRenderer spriteRenderer;
    private Transform modelTransform;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        modelTransform = transform;
    }
    /// <summary>
    /// Updates the character orientation based on movement direction
    /// </summary>
    
    public void updateOrientation(Vector3 targetPosition, float moveSpeed)
    {
        // Handling 2D character sprite
        if (is2D && spriteRenderer != null)
        {
            // Flip sprite based on movement direction
            spriteRenderer.flipX = targetPosition.x < transform.position.x;
        }
        // Handling 3D character model
        else if (!is2D && modelTransform !=null)
        {
            // Calculate direction of vector ignoring y Component
            Vector3 direction = (targetPosition - transform.position).normalized;
            // Ensure no vertical rotation
            direction.y = 0;
            // Rotate model if there is movement
            if (direction != Vector3.zero)
            {
                // Compute rotation to face movement direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // allow target to slowly move towards target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
            }
        }
    }

}
