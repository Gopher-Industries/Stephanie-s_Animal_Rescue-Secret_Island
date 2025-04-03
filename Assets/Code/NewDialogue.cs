using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NewDialogue : MonoBehaviour
{
    // Store dialogue messages in list
    private List<GameObject> dialogueMessages = new List<GameObject>();
    private int index = 0;
    // Adding dialogue message to list
    public void AddDialogue(string text, GameObject d_template)
    {
        // Creating new dialogue message using template
        GameObject message = Instantiate(d_template, transform);
        // Set text of dialogue message
        message.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
        message.SetActive(false);
        dialogueMessages.Add(message);
    }
    // Start dialogue and display message
    public void StartDialogue()
    {
        if(dialogueMessages.Count > 0)
        {
            dialogueMessages[0].SetActive(true);
        }
    }
    // Handle dialogue progressing
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialogueMessages.Count > 0)
        {
            // Hide current dialogue and move next message
            dialogueMessages[index].SetActive(false);
            index++;
            // Checking if there are more dialogue to move to
            if(index < dialogueMessages.Count)
            {
                dialogueMessages[index].SetActive(true);
            }
            else
            // IF all dialogue are displayed end dialogue and destroy dialogue from scene
            {
                CharacterMover.dialogue = false;
                Destroy(gameObject);
            }
        }
    }
}
