using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class TeleportPortalsManagement : MonoBehaviour
{
    public GameObject player;
    public GameObject teleportUI;
    public Transform buttonContainer;
    public Transform portalsParent;
    public GameObject buttonPrefab;
    public string findFlashPrompt;
    public float destinationX;
    public float destinationY;
    public float destinationZ;

    private List<TeleportPortal> portals = new List<TeleportPortal>();
    private TeleportPortal selectedPortal;

    public bool playerHasFlash = false;

    void Start()
    {
        // Automatically find all portals under the parent
        foreach (Transform child in portalsParent)
        {
            TeleportPortal portal = child.GetComponent<TeleportPortal>();
            if (portal != null)
            {
                portal.manager = this;
                portals.Add(portal);
                //Debug.Log("Found portal: " + portal.portalName);
            }
        }

        teleportUI.SetActive(false);
    }

    public void OnPortalClicked(TeleportPortal portal)
    {
        //if (!playerHasFlash) return;

        selectedPortal = portal;
        ShowTeleportOptions();
    }

    private void ShowTeleportOptions()
    {
        // Show the teleport UI
        teleportUI.SetActive(true);

        // Clear previous buttons
        foreach (Transform child in buttonContainer.transform)
        {
            Destroy(child.gameObject);
        }
        if (playerHasFlash)
        {
            // Create buttons for other portals
            foreach (var portal in portals)
            {
                if (portal != selectedPortal)
                {
                    // Create a button for each portal
                    GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
                    buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = portal.portalName;

                    TeleportPortal destination = portal;
                    buttonObj.GetComponent<Button>().onClick.AddListener(() => TeleportTo(destination));
                }
            }
        }
        else
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = findFlashPrompt;
        }



    }

    public void CloseTeleportUI()
    {
        teleportUI.SetActive(false);
        CharacterMover.isInteracting = false;
    }

    private void TeleportTo(TeleportPortal destination)
    {
        //player.transform.position = destination.transform.position;
        player.transform.position = new Vector3(destination.transform.position.x + destinationX, destination.transform.position.y + destinationY, destination.transform.position.z + destinationZ);
        CloseTeleportUI();
    }
}
