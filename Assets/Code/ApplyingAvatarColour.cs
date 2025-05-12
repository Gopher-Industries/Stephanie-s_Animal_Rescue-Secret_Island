using UnityEngine;

public class AvatarColor : MonoBehaviour
{
    [Header("Color Settings")]
    public bool toggleColor = false;
    public Color colorOn = Color.red;
    public Color colorOff = Color.white;

    private MaterialPropertyBlock propBlock;

    public GameObject Object;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
    }

    public void ToggleAndApplyColor()
    {
        if (Object == null) return;  // Ensure hairObject is assigned

        Renderer rend = Object.GetComponent<Renderer>();
        if (rend == null) return;  // Ensure it has a Renderer

        rend.GetPropertyBlock(propBlock);  // Get the current material properties
        toggleColor = !toggleColor;  // Toggle between the two colors

        Color targetColor = toggleColor ? colorOn : colorOff;  // Set the color
        propBlock.SetColor("_Color", targetColor);  // Apply the new color
        rend.SetPropertyBlock(propBlock);  // Set the updated material properties
    }
}
