using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public string portalName;
    public TeleportPortalsManagement manager;

    private void OnMouseDown()
    {
        if (manager != null)
        {
            manager.OnPortalClicked(this);
        }
    }
}
