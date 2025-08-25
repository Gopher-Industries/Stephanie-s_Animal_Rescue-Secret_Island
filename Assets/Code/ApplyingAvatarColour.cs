using UnityEngine;

public class ApplyingAvatarColor : MonoBehaviour
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
        if (Object == null) return; 

        Renderer rend = Object.GetComponent<Renderer>();
        if (rend == null) return;

        rend.GetPropertyBlock(propBlock); 
        //Toggle between the two colors for now as a prototype (just toggling between turning selected color on and off)
        toggleColor = !toggleColor; 
        //Seting new color
        Color targetColor = toggleColor ? colorOn : colorOff; 
        //Apply the new color
        propBlock.SetColor("_Color", targetColor); 
        rend.SetPropertyBlock(propBlock);
    }
}
