using UnityEngine;

public class TelescopeManager : MonoBehaviour
{
    // Create GameObject variables for each telescope view
    public GameObject dragonTelescopeView;  // Dragon's telescope view
    public GameObject dinoCaveTelescopeView; // Dinosaur cave's telescope view
    public GameObject lakeTelescopeView;   // Lake's telescope view

    // Open the Dragon's view
    public void OpenDragonView()
    {
        // Close all other views
        CloseAllViews();
        // Open the Dragon's view
        dragonTelescopeView.SetActive(true);
    }

    // Open the Dinosaur cave's view
    public void OpenDinoCaveView()
    {
        // Close all other views
        CloseAllViews();
        // Open the Dinosaur cave's view
        dinoCaveTelescopeView.SetActive(true);
    }

    // Open the Lake's view
    public void OpenLakeView()
    {
        // Close all other views
        CloseAllViews();
        // Open the Lake's view
        lakeTelescopeView.SetActive(true);
    }

    // Close all telescope views
    public void CloseAllViews()
    {
        dragonTelescopeView.SetActive(false);
        dinoCaveTelescopeView.SetActive(false);
        lakeTelescopeView.SetActive(false);
    }
}
