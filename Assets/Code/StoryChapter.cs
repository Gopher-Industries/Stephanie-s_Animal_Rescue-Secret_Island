using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class StoryChapter : MonoBehaviour {
    public GameObject dialogueCanvas;
    public Image backgroundImage;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image speakerImage;
    public GameObject speakerMask;
    public Button goBtn;
    public List<Dialogue> dialogue = new List<Dialogue>();
    public string textHighlightColor = "FF0000";
    public bool chapterIsComplete = false;
    public bool minigameRunning = false;
    public bool autoPlayDialogue = true;
    public bool waitForPlayerInput = false;
    public string sceneToLoadOnCompletion;
    public string minigameToRun;
    


    void Start(){
        PlayerPrefs.SetInt("hasPlayed", 1);

        GameManager.Instance.SetActiveScene(name);

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop(){
        yield return DialogueFeeder();

        GameManager.Instance.LoadNewScene(sceneToLoadOnCompletion);
        GameManager.Instance.UnloadScene(this.name);
    }

    IEnumerator DialogueFeeder(){
        while(!chapterIsComplete){
            backgroundImage.sprite = dialogue[0].backgroundImage;
            for(int i = 0; i < dialogue.Count; i++){
                if(dialogue[i].backgroundImage != backgroundImage.sprite){
                    backgroundImage.sprite = dialogue[i].backgroundImage;
                }

                string[] words = dialogue[i].speakerLine.Split(' ');

                float baseTimePerWord = 0.3f; // Base time in seconds per word
                float minDisplayTime = 2.0f;  // Minimum time to display short lines
                float maxDisplayTime = 6.0f;  // Cap for long sentences

                float rawTotalTime = words.Length * baseTimePerWord;
                float clampedTotalTime = Mathf.Clamp(rawTotalTime, minDisplayTime, maxDisplayTime);
                float timePerWord = clampedTotalTime / words.Length;

                //float timePerWord = 5 / words.Length;

                speakerNameText.text = dialogue[i].speakerName;
                dialogueText.text = dialogue[i].speakerLine;
                speakerImage.sprite = dialogue[i].speakerSprite;
                
                if(!speakerMask.activeSelf){
                    speakerMask.SetActive(true);
                }
                
                yield return new WaitForSeconds(0.5f);

                for (int j = 0; j < words.Length; j++){
                    string word = words[j];
                    dialogueText.text = dialogueText.text.Replace(word, $"<color=#{textHighlightColor}>{word}</color>");
                    yield return new WaitForSeconds(timePerWord);
                }

                yield return new WaitForSeconds(1.0f);

                if(dialogue[i].playsMinigame){
                    Debug.Log("Playing minigame");
                    dialogueCanvas.SetActive(false);
                    minigameRunning = true;
                    GameManager.Instance.LoadMiniGame(minigameToRun);
                    ClearDialogue();
                    ClearBackground();
                    yield return new WaitUntil(() => !minigameRunning);

                    dialogueCanvas.SetActive(true);
                    GameManager.Instance.UnloadMiniGame();
                    yield return new WaitForSeconds(1.0f);
                }

                if(!autoPlayDialogue){
                    waitForPlayerInput = true;
                    yield return new WaitUntil(() => !waitForPlayerInput);
                }
            }
            chapterIsComplete = true;
        }
        Debug.Log("Dialogue Complete");
        yield return null;
    }

    void ClearDialogue(){
        speakerNameText.text = "";
        dialogueText.text = "";
        speakerImage.sprite = null;
        speakerMask.SetActive(false);
    }

    void ClearBackground(){
        backgroundImage.sprite = null;
    }

    /// <summary>
    /// Player must press the button in the scene to allow the scene to continue
    /// </summary>
    public void ContinueDialogue(){
        if(!autoPlayDialogue){
            waitForPlayerInput = false;
        }
    }

    public void UpdateAutoPlay(){
        autoPlayDialogue = !autoPlayDialogue;
        goBtn.interactable = !autoPlayDialogue;
    }

}

[System.Serializable]
public class Dialogue{
    public Sprite backgroundImage;
    public Sprite speakerSprite;
    public string speakerName;
    public string speakerLine;
    public bool playsMinigame = false;
}