using UnityEngine;

public class ClickPositionPrinter : MonoBehaviour
{
    void Update()
    {
        // Check if the left mouse button was clicked
        if (Input.GetMouseButtonDown(0))
        {
            PrintClickPosition(Input.mousePosition);
        }
    }

    /// <summary>
    /// Casts a ray from the camera to the clicked position and prints the world coordinates.
    /// </summary>
    void PrintClickPosition(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        // Draw the ray in the Scene view for debugging purposes
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

        RaycastHit hit;

        // Perform the raycast without a layer mask to hit any collider
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPosition = hit.point;
            Debug.Log($"Clicked Position: X={hitPosition.x}, Y={hitPosition.y}, Z={hitPosition.z}");
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any objects.");
        }
    }

}
