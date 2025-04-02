using UnityEngine;

public class ColorBlindMode : MonoBehaviour
{
    private bool isColorBlindModeActive = false;

    void Update()
    {
        // Toggle colorblind mode by pressing the "C" key
        if (Input.GetKeyDown(KeyCode.C))
        {
            isColorBlindModeActive = !isColorBlindModeActive;
            ApplyColorBlindFilter(isColorBlindModeActive);
        }
    }

    void ApplyColorBlindFilter(bool isActive)
    {
        // Get all the child objects of the player and apply color changes
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        foreach (var renderer in renderers)
        {
            if (isActive)
            {
                // Activate colorblind mode: grayscale
                renderer.material.color = Color.gray;
            }
            else
            {
                // Deactivate colorblind mode: restore original color
                renderer.material.color = Color.white;
            }
        }
    }
}
