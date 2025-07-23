using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public string portalName;
    public TeleportPortalsManagement manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop the player's movement
            CharacterMover.isInteracting = true;

            // Tell the manager to open the UI
            if (manager != null)
            {
                manager.OnPortalClicked(this);
            }
        }
    }
}
