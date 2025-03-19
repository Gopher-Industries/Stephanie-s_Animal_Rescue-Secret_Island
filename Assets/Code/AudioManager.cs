using UnityEngine;
using System.Collections.Generic;
 
public class AudioManager : MonoBehaviour {
    public static AudioManager audioManagerInstance {get; private set;}
    public AudioSource musicSource;
    public AudioSource sfxSource;
    private float musicVolume = 1;
    private float sfxVolume = 1;

    [SerializeField]
    private List<AudioClip> musicClips = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> sfxClips = new List<AudioClip>();

    void Awake(){
        if(audioManagerInstance != null && audioManagerInstance != this){
            Destroy(this);
        }else{
            audioManagerInstance = this;
        }
    }

    void Start(){
        Init();

        //PlayMusic(musicClips[0]);                     //Plays the first clip in the list
    }

    void Init(){
        musicSource.volume = musicVolume;               //Initialise both audio sources to have full volume.
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Sets the music clip to play.
    /// Method will remain public to allow easy access.
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusic(AudioClip clip){                    
        if(clip != null){
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Sets the effect to play.
    /// 'OneShot' means that the effect will not be cut off if another is triggered.
    /// Method will remain public to allow easy access.
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySFX(AudioClip clip){
        if(clip != null){
            sfxSource.clip = clip;
            sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Sets the volume of the respective audiosource using the sliders in scene.
    /// Must be a public method to be assignable in the editor.
    /// </summary>
    /// <param name="vol"></param>
    public void SetMusicVolume(float vol){
        musicSource.volume = vol;
    }

    public void SetSFXVolume(float vol){
        sfxSource.volume = vol;
    }
}