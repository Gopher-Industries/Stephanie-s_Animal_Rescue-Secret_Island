using UnityEngine;
using System.Collections;

public class TelescopicView : MonoBehaviour
{
    // Public variables
    public float ZoomLevel = 2.0f;
    public float ZoomInSpeed = 100.0f;
    public float ZoomOutSpeed = 100.0f;

    // Private variables
    private float initFOV;

    // Called when the script is initialized
    void Start()
    {
        // Get the initial field of view of the main camera
        initFOV = Camera.main.fieldOfView;
    }

    // Called once per frame during the game
    void Update()
    {
        // When the left mouse button is held down
        if (Input.GetKey(KeyCode.Mouse0))
        {
            ZoomView();
        }
        else
        {
            ZoomOut();
        }
    }

    // Zoom in by reducing the camera's field of view
    void ZoomView()
    {
        if (Mathf.Abs(Camera.main.fieldOfView - (initFOV / ZoomLevel)) < 0.5f)
        {
            Camera.main.fieldOfView = initFOV / ZoomLevel;
        }
        else if (Camera.main.fieldOfView - (Time.deltaTime * ZoomInSpeed) >= (initFOV / ZoomLevel))
        {
            Camera.main.fieldOfView -= (Time.deltaTime * ZoomInSpeed);
        }
    }

    // Zoom out by increasing the camera's field of view
    void ZoomOut()
    {
        if (Mathf.Abs(Camera.main.fieldOfView - initFOV) < 0.5f)
        {
            Camera.main.fieldOfView = initFOV;
        }
        else if (Camera.main.fieldOfView + (Time.deltaTime * ZoomOutSpeed) <= initFOV)
        {
            Camera.main.fieldOfView += (Time.deltaTime * ZoomOutSpeed);
        }
    }
}
