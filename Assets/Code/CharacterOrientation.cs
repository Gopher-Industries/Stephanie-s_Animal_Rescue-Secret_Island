using UnityEngine;

public class CharacterOrientation : MonoBehaviour
{
    // Default as 2D character
    public bool is2D = true;

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
        if (is2D && spriteRenderer != null)
        {
            spriteRenderer.flipX = targetPosition.x < transform.position.x;
        }
        else if (!is2D && modelTransform !=null)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
            }
        }
    }

}
