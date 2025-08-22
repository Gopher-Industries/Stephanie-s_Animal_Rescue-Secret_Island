using UnityEngine;
using UnityEngine.UI;

// A 16:9 screen has an aspect ratio of 1.777
// A 4:3 screen has an aspect ratio of 1.333
// A slightly higher value than 1.777 will switch to a tall mode cropping the top half of the background
// This script might only be useful for resource centre specifcally as the buttons are anchored 
// within the bottom half of the background image.
[RequireComponent(typeof(Image), typeof(AspectRatioFitter))]
public class BackgroundScaler : MonoBehaviour
{
    // The aspect ratio threshold
    // Horizontal screens wider than this will zoom and anchor to the bottom
    private float wideAspectRatioThreshold = 1.8f;

    private RectTransform rectTransform;
    private AspectRatioFitter aspectRatioFitter;
    private Image image; 

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        aspectRatioFitter = GetComponent<AspectRatioFitter>();
        image = GetComponent<Image>(); 
        AdaptBackground();
    }

    private void AdaptBackground()
    {
        if (image == null || rectTransform.parent == null)
        {
            Debug.LogError("BackgroundScaler is missing an Image, or a parent object.");
            return;
        }

        float screenAspectRatio = (float)Screen.width / (float)Screen.height;

        // This is for devices with a wider width aspect ratio like the iPhone 10+ (19.5:9)
        if (screenAspectRatio > wideAspectRatioThreshold)
        {
            // Disable the AspectRatioFitter component so it doesn't fight the manual changes
            aspectRatioFitter.enabled = false;

            // Set anchors to fill full width and anchor to the bottom centre
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            // Move image to new anchor point
            rectTransform.anchoredPosition = Vector2.zero;

            float imageAspectRatio = image.sprite.rect.width / image.sprite.rect.height;

            // Get the parent's width (which is now our image's width due to the anchors).
            RectTransform parentRect = rectTransform.parent as RectTransform;
            float parentWidth = parentRect.rect.width;

            // Calculate the new height required to maintain the aspect ratio
            float newHeight = parentWidth / imageAspectRatio;

            // Set the size directly. With our current anchors, sizeDelta.y controls the height.
            rectTransform.sizeDelta = new Vector2(0, newHeight);
        }
        else
        {
            // This is for iPad (4:3) or standard monitors (16:9)
            // We do nothing and let the AspectRatioFitter work as configured in the Inspector,
            // which should be "Fit In Parent" to complete fill 1080p, 1440p, 2160p standard monitors
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        }
    }
}