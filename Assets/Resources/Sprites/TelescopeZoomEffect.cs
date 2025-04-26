using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class TelescopeEffect : MonoBehaviour
{
    public Button telescopeButton; // Telescope button to trigger zoom effect
    public PostProcessVolume postProcessVolume; // Post-processing volume to access vignette
    private Vignette vignette; // To control the vignette effect
    private bool isZoomedIn = false; // Is the camera zoomed in?
    private float targetIntensity = 0.5f; // Intensity of vignette when zoomed in
    private float normalIntensity = 0f; // Intensity of vignette in normal view (no vignette)
    private float lerpSpeed = 2f; // Speed at which vignette intensity changes

    void Start()
    {
        // Get the Vignette effect from the post-process volume
        postProcessVolume.profile.TryGetSettings(out vignette);
        
        // Set initial vignette intensity to 0 (no vignette effect)
        vignette.intensity.value = normalIntensity;
        
        // Add listener to the telescope button to toggle zoom effect
        telescopeButton.onClick.AddListener(ToggleTelescopeEffect);
    }

    void Update()
    {
        // Smoothly adjust the vignette intensity when zoomed in
        if (isZoomedIn)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetIntensity, Time.deltaTime * lerpSpeed);
        }
        else
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, normalIntensity, Time.deltaTime * lerpSpeed);
        }
    }

    // Toggle the zoom effect when the telescope button is clicked
    public void ToggleTelescopeEffect()
    {
        isZoomedIn = !isZoomedIn; // Toggle zoom state
    }
}
