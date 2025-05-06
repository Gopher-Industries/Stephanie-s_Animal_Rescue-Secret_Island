										📖Storyline NPC interaction📖

1. Adding character profiles

- this step is to add any characters to the storyline scripts to allow different characters to talk within a single dialogue. Additional characters must be added to the CharacterProfiles.json file located in:

assets ---> Resources ---> StoryDialogue ---> CharacterProfiles.json

a) adding characters using the format:

      "characterID": "Reference for code",
      "displayName": "name to be displayed",
      "characterImage": "CharacterImages/.png name"

If additional file is added to sort and store character image reference, start from resource file e.g "CharacterImages/NewFile/.png name" if a new file is added into characterImages

b) *** In order to display image ENSURE image Texture Type is set to "Sprite (2D and UI)" and Sprite mode is set to "Single" can be done in inspector tab of unity (clicking on image in unity)***

2. Adding storyline script

a) add Json file script within the StoryDialogue located in 

assets ---> Resources ---> StoryDialogue

b) Json file should be formatted such as 

{
  "interactions": [
    {
      "dialogueLines": [
        {"characterID": "CharacterID of first speaker", "text": "message to be displayed on dialogue box"},
        {"characterID": "CharacterID of second speaker", "text": "message to be displayed on dialogue box"}
      ]
    }
  ]
}


3. Referencing Dialogue to NPC
- in order to assign what dialogue is given to the NPC the script should be added to an interaction sphere allowing the player to interact to the npc when walking near the model.

a) within the inspector tab in unity the NPC Story Dialogue script should be assigned as following:

      D_template: Dialogue Template, (The dialogue template to be used can be dragged from the scene connected to "Canvas dialogue")
      canva: "Canvas dialogue", 
      Json File Name: "Name of dialogue located in the StoryDialogue"

a) if additional files are added to format dialogue files locating the file would be changed in order to access the script. this would be done by adding FileName/ before the dialogue.json file in the Json File Name must be done in the inspector tab.


