using UnityEngine;
using UnityEngine.UI;

public class TelescopeController : MonoBehaviour
{
    // The UI Image that will display the picture
    public Image viewImage;

    // The picture to be shown
    public Sprite targetImage;

    // Start is called before the first frame update
    void Start()
    {
        // Initially hide the image
        viewImage.gameObject.SetActive(false);
    }

    // This method will be called when the telescope button is clicked
    public void OnButtonClick()
    {
        // Show the picture
        viewImage.sprite = targetImage;
        viewImage.gameObject.SetActive(true);
    }

    // This method will be called when the back button is clicked
    public void CloseTelescope()
    {
        // Hide the picture
        viewImage.gameObject.SetActive(false);
    }
}
