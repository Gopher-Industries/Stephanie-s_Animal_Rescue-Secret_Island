using UnityEngine;

public class LimitCamera : MonoBehaviour
{
    /*public GameObject player; // Reference to the player object

    float storedShadowDistance;

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, 40, transform.position.z);
    }
    void OnPreRender()
    {
        storedShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }

    void OnPostRender()
    {
        Debug.LogWarning("OnPostRender called");
        QualitySettings.shadowDistance = storedShadowDistance;
    }*/
    public string playerTag = "Player"; // Set this in Inspector or hardcode it
    private GameObject player;

    float storedShadowDistance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player with tag '" + playerTag + "' not found!");
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, 40, transform.position.z);
        }
    }

    void OnPreRender()
    {
        storedShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }

    void OnPostRender()
    {
        Debug.LogWarning("OnPostRender called");
        QualitySettings.shadowDistance = storedShadowDistance;
    }


}
