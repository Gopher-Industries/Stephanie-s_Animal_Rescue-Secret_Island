using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class NPCStoryDialogue : MonoBehaviour
{
    public GameObject d_template;
    public GameObject canva;
    private bool player_detection = false;
    private bool dialogueTriggered = false;
    private GameObject currentDialogue;

    public string JsonFileName;
    private NewDialogue dialogueScript;
    private int interactionCount = 0;

    // Group dialogue lines for interaction stage
    [System.Serializable]
    public class InteractionSet
    {
        public List<DialogueLine> dialogueLines;
    }
    // Dialogue character
    [System.Serializable]
    public class DialogueLine
    {
        public string characterID; 
        public string text;
    }
    [System.Serializable]
    public class StoryDialogueData
    {
        public List<InteractionSet> interactions;
    }
    // Storing character info
    [System.Serializable]
    public class CharacterProfile
    {
        public string characterID; // ID for uniue character
        public string displayName; // Display name for cahracter
        public string characterImage; // Image for character
    }
    // List of character from Json
    [System.Serializable]
    public class CharacterProfileList
    {
        public List<CharacterProfile> characters;
    }
    // Dictionary for character IDs to their profile
    private Dictionary<string, CharacterProfile> characterMap = new Dictionary<string, CharacterProfile>();

    // Load the character profiles into the dictionary
    void LoadCharacterProfiles()
    {
        // Loading Json file of chracter profiles from resources folder
        TextAsset jsontext = Resources.Load<TextAsset>($"StoryDialogue/CharacterProfiles");
        if (jsontext != null)
        {
            // Deserialize Json text into ChracterProfileList 
            CharacterProfileList profileList = JsonUtility.FromJson<CharacterProfileList>(jsontext.text);
            //loop through each character profile
            foreach(var profile in profileList.characters)
            {
                // Add profile if chracter ID isnt already present
                if(!characterMap.ContainsKey(profile.characterID))
                {  
                    characterMap[profile.characterID] = profile;
                }
                else
                {
                    // Log warning for duplicated CharacterID
                    Debug.LogWarning("Duplicate CharacterID detected");
                }
            }
        }
        else
        {
            // Log warning if chracter profile Json file isnt found
            Debug.LogError("Character profile JSON file not found");
        }
    }

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
    // Update dialogue UI with character display name and image based on ID
    void CharacterUI(GameObject templateInstance, string characterID)
    {
        // Find and update characters display name text
        TMP_Text nameText = templateInstance.transform.Find("CharacterNameText")?.GetComponent<TMP_Text>();
        if(nameText != null)
        {
            // Set display name from character profile
            nameText.text = characterMap[characterID].displayName;
            nameText.alignment = TextAlignmentOptions.Center;
        }
        // Find character image
        UnityEngine.UI.Image charImage = templateInstance.transform.Find("CharacterImage")?.GetComponent<UnityEngine.UI.Image>();
        if(charImage != null)
        {
            // Assuming the characterImage in the profile is the path to the sprite
            charImage.sprite = Resources.Load<Sprite>(characterMap[characterID].characterImage);

        }
    }

    void StartDialogue()
    {
        LoadCharacterProfiles();

        // Destroy Dialogue from GameObject if exists
        if(currentDialogue != null)
        {
            Destroy(currentDialogue);
        }
        // Load story dialogue from resource folder
        TextAsset jsontext = Resources.Load<TextAsset>($"StoryDialogue/{JsonFileName}");
        if(jsontext == null)
        {
            // Log that File isnt found
            Debug.LogError("Story Json file not found");
            return;
        }

        // Deserialize Json text into structured object and retrieve dialogue lines for the current interaction stage
        StoryDialogueData storyData = JsonUtility.FromJson<StoryDialogueData>(jsontext.text);
        int stageIndex = Math.Clamp(interactionCount, 0, storyData.interactions.Count - 1);
        List<DialogueLine> selectedDialogue = storyData.interactions[stageIndex].dialogueLines;

        // Create new dialogue
        currentDialogue = new GameObject("DialogueContainer");
        currentDialogue.transform.SetParent(canva.transform, false);
        dialogueScript = currentDialogue.AddComponent<NewDialogue>();

        // Instantiate and set up the template
        GameObject templateInstance = Instantiate(d_template, currentDialogue.transform);

        // Go through the dialogue lines and add to dialogue system
        foreach (var line in selectedDialogue)
        {
            // Set up character info for each dialogue line
            CharacterUI(templateInstance, line.characterID);
            dialogueScript.AddDialogue(line.text, templateInstance);
        }

        // Start displaying the dialogue
        dialogueScript.StartDialogue();
        interactionCount++;
    }

    // Detect when player enters NPC bubble
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "3D Character")
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
