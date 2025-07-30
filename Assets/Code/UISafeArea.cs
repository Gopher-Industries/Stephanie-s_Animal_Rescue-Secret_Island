using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class UISafeArea : MonoBehaviour
{
    private RectTransform panelRectTransform;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);
    private ScreenOrientation lastScreenOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        panelRectTransform = GetComponent<RectTransform>();

        if (panelRectTransform == null)
        {
            Debug.LogError("UISafeArea.cs requires a RectTransform component.", this);
            enabled = false;
            return;
        }

        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafeArea || Screen.orientation != lastScreenOrientation)
        {
            ApplySafeArea();
        }
    }

    // Calculates and applies the safe area to the RectTransform's anchors.
    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area from pixel coordinates to normalized viewport coordinates (0 to 1)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Apply the calculated anchor points to the RectTransform.
        panelRectTransform.anchorMin = anchorMin;
        panelRectTransform.anchorMax = anchorMax;

        // Cache the current safe area and orientation to check against in the next frame.
        lastSafeArea = safeArea;
        lastScreenOrientation = Screen.orientation;
    }
}
