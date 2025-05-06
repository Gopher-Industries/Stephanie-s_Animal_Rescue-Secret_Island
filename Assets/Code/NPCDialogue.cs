using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common.Analytics;
public class NPCDialogueSystem : MonoBehaviour
{
    public GameObject d_template;
    public GameObject canva;
    public string characterName;
    public Sprite characterImage;
    
    private bool player_detection = false;
    private bool dialogueTriggered = false;
    private GameObject currentDialogue;
    private NewDialogue dialogueScript;
    // Store multiple dialogues for npcs
    private int interactionCount = 0;
    // Serializable class storing multiple lines of dialogue in the interaction stage
    [System.Serializable]
    public class InteractionDataType
    {
        public List<string> dialogueLines;
    }

    public List<InteractionDataType> interactionStages = new List<InteractionDataType>();
    

    // Function to detect if player is detected and no dialogue is playing to start a new dialogue
    void Update()
    {
        if (player_detection && !CharacterMover.dialogue && !dialogueTriggered)
        {
            dialogueTriggered = true;
            canva.SetActive(true);
            CharacterMover.dialogue = true;
            StartDialogue();
        }

    }

    void CharacterUI(GameObject templateInstance)
    {
        // Find and update characters display name text
        TMP_Text nameText = templateInstance.transform.Find("CharacterNameText")?.GetComponent<TMP_Text>();
        if(nameText!=null)
        {
            // Set character name text as character name
            nameText.text = characterName;
            nameText.alignment = TextAlignmentOptions.Center; // Center the text name in ui
        }
        // Find and update the characters image
        UnityEngine.UI.Image charImage = templateInstance.transform.Find("CharacterImage")?.GetComponent<UnityEngine.UI.Image>();
        if(charImage != null && characterImage !=null)
        {
            // Sets the chracters image as the image
            charImage.sprite = characterImage;
        }
    }

    void StartDialogue()
    {
        // Destroy Dialoge from GameObject if exists
        if(currentDialogue != null)
        {
            Destroy(currentDialogue);
        }
        // Check and log if no interaction stages
        if (interactionStages.Count == 0)
        {
            Debug.LogError("Npc has no current dialogue");
            return;
        }
        // Ensure interaction stage index has a valid range
        int stageIndex = Mathf.Clamp(interactionCount, 0,  interactionStages.Count - 1);
        // Get dialogue lines for current stage
        List<string> selectedDialogue = interactionStages[stageIndex].dialogueLines;
        // Create new gameobject to hold dialogue UI
        currentDialogue = new GameObject("DialogueContrainer");
        currentDialogue.transform.SetParent(canva.transform, false);
        dialogueScript = currentDialogue.AddComponent<NewDialogue>();
        // Instantiate new UI template for dialogue display
        GameObject templateInstance = Instantiate(d_template, currentDialogue.transform);
        // Call characteUI to set up name and image
        CharacterUI(templateInstance);
        // Add lines for dialogue to system
        foreach (string line in selectedDialogue)
        {
            dialogueScript.AddDialogue(line, templateInstance);
        }
        //Begin dispaly dialogue and move to next dialogue for next interaction
        dialogueScript.StartDialogue();
        interactionCount++;
    }
    // Detect when player enters npc bubble
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "3D Character")
        {
            player_detection = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        player_detection = false;
        // Retrigger same dialogue
        dialogueTriggered = false;
    }
}
