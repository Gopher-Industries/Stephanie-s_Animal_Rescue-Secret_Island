using UnityEngine;

public class MainMenu : MonoBehaviour {
    public AudioClip mainMenuMusic;
    void Start(){
        AudioManager.audioManagerInstance.PlayMusic(mainMenuMusic);
        GameManager.GameManagerInstance.SetActiveScene(name);
    }
    
    public void ClickPlay(){
        GameManager.GameManagerInstance.MainMenuPlayButon();
    }
    
}