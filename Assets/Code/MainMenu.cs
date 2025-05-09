using UnityEngine;

public class MainMenu : MonoBehaviour {
    public AudioClip mainMenuMusic;
    void Start(){
        AudioManager.audioManagerInstance.PlayMusic(mainMenuMusic);
        GameManager.Instance.SetActiveScene(name);
    }
    
    public void ClickPlay(){
        GameManager.Instance.MainMenuPlayButon();
    }
    
}