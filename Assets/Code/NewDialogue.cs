using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NewDialogue : MonoBehaviour
{
    private List<GameObject> dialogueMessages = new List<GameObject>();
    private int index = 0;

    public void AddDialogue(string text, GameObject d_template)
    {
        //GameObject d_template = FindObjectOfType<NPCSystem>().d_template;
        GameObject message = Instantiate(d_template, transform);
        message.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
        message.SetActive(false);
        dialogueMessages.Add(message);
    }

    public void StartDialogue()
    {
        if(dialogueMessages.Count > 0)
        {
            dialogueMessages[0].SetActive(true);
        }
    }
    
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialogueMessages.Count > 0)
        {
            dialogueMessages[index].SetActive(false);
            index++;

            if(index < dialogueMessages.Count)
            {
                dialogueMessages[index].SetActive(true);
            }
            else
            {
                CharacterMover.dialogue = false;
                Destroy(gameObject);
            }
        }
    }
}
