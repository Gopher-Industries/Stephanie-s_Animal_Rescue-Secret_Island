using UnityEngine;

public class TelescopeTrigger : MonoBehaviour
{
    public GameObject telescopePanel; 

    void OnMouseDown()
    {
        if (telescopePanel != null)
        {
            telescopePanel.SetActive(true);
        }
    }
}
