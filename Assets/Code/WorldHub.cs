using UnityEngine;

public class WorldHub : MonoBehaviour {

    public AudioClip worldHubMusic;

    void Start(){
        AudioManager.audioManagerInstance.PlayMusic(worldHubMusic);
    }

}