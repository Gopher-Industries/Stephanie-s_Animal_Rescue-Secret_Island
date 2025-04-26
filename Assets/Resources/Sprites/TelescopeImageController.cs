using UnityEngine;
using UnityEngine.UI;

public class TelescopeImageController : MonoBehaviour
{
    private Quaternion originalRotation;

    void Start()
    {
        // Store the original rotation of the image
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Rotate 90 degrees to the left
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Rotate 90 degrees to the right
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Rotate 180 degrees
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Reset to original rotation
            transform.rotation = originalRotation;
        }
    }
}
