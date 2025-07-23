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
        // Create new dialgoue message using template
        GameObject message = Instantiate(d_template, transform);
        // Finds DialogueText child Within instantiated message
        Transform dialogueTextTransform = message.transform.Find("DialogueText");

        if(dialogueTextTransform != null)
        {
            // Gets the TextMeshProUGUI compoenent to set dialogue text
            TextMeshProUGUI dialogueText = dialogueTextTransform.GetComponent<TextMeshProUGUI>();
            if(dialogueText != null)
            {
                // If dialogue text isnt null sets the text as the provided string
                dialogueText.text = text;
            }
        }    
        else
        {
            // Sets logged warning if Dialogue Text is missing
            Debug.LogWarning("DialogueText missing");
        }
        // Set new message to inactive
        message.SetActive(false);
        // Add message to dialogue message for future reference
        dialogueMessages.Add(message);
    }
    // Start dialogue and display message
    public void StartDialogue()
    {
        if(dialogueMessages.Count > 0)
        {
            // Actiove dialogue message in list
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
                CharacterMover.isInteracting = false;
                Destroy(gameObject);
            }
        }
    }
}
