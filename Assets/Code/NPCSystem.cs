using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common.Analytics;
public class NPCSystem : MonoBehaviour
{
    public GameObject d_template;
    public GameObject canva;
    
    private bool player_detection = false;
    private bool dialogueTriggered = false;
    private GameObject currentDialogue;
    private NewDialogue dialogueScript;
    // store multiple dialogues for npcs
    public List<InteractionDataType> interactionStages = new List<InteractionDataType>();
    private int interactionCount = 0;
    [System.Serializable]
    public class InteractionDataType{
        public List<string> dialogueLines;
    }


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

    void StartDialogue()
    {
        // Destroy Dialoge from GameObject if exists
        if(currentDialogue != null)
        {
            Destroy(currentDialogue);
        }

        if (interactionStages.Count == 0)
        {
            Debug.LogError("NPC has no dialogue");
            return;
        }

        int stageIndex = Mathf.Clamp(interactionCount, 0, interactionStages.Count - 1);
        List<string> selectedDialogue = interactionStages[stageIndex].dialogueLines;

        // Create new dialogue
        currentDialogue = new GameObject("DialogueContrainer");
        currentDialogue.transform.SetParent(canva.transform, false);

        dialogueScript = currentDialogue.AddComponent<NewDialogue>();

        foreach (string line in selectedDialogue)
        {
            dialogueScript.AddDialogue(line, d_template);
        }

        dialogueScript.StartDialogue();
        interactionCount++;
    }

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
