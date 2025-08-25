using UnityEngine;

public class AvatarColorController : MonoBehaviour
{
    [Header("Color Settings")]
    public bool Change = false;
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
        Color targetColor;
        //Check for object (hair)
        if (Object == null) return; 
        //Check for renderer component
        Renderer rend = Object.GetComponent<Renderer>();
        if (rend == null) return;
        //Get the current material properties
        rend.GetPropertyBlock(propBlock); 
        if (Change)
        {
            //Set the color
            targetColor = colorOn;
        }
        else
        {
            //reverse to second colour option (white for default)
            targetColor = colorOff;
        }
        //Apply colour to object
        propBlock.SetColor("_Color", targetColor); 
        rend.SetPropertyBlock(propBlock);
    }

    public void ChildRenderChange()
    {
        if (Object == null) return;
        //Get all renderer component of parent object and its children
        Renderer[] renderers = Object.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Color targetColor = Change ? colorOn : colorOff;
        //Loop through all objects in hierarchy (objects with renderer)
        foreach (Renderer rend in renderers)
        {
            //Only apply colour to children with "ToCustomize" tag
            if (rend.gameObject.CompareTag("ToCustomize"))
            {
                //Get material property for rendere and change to updated colour
                rend.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", targetColor);
                rend.SetPropertyBlock(propBlock);
            }
        }
    }
}
