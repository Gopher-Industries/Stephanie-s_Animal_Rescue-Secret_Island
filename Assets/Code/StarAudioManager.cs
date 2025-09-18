using UnityEngine;

public class StarAudioManager : MonoBehaviour 
{
    [SerializeField] private AudioClip starConstellationMusic;
    [SerializeField] private AudioClip nodeClickedSFX;
    [SerializeField] private AudioClip connectionSFX;
    [SerializeField] private AudioClip disconnectionSFX;
    [SerializeField] private AudioClip levelCompleteSFX;
    [SerializeField] private AudioClip buttonSFX;

    void Start()
    {
        AudioManager.audioManagerInstance.PlayMusic(starConstellationMusic);
    }

    public void PlayConnectionSFX()
    {
        AudioManager.audioManagerInstance.PlaySFX(connectionSFX);
    }

    public void PlayDisconnectionSFX()
    {
        AudioManager.audioManagerInstance.PlaySFX(disconnectionSFX);
    }
    public void PlayNodeClickedSFX()
    {
        AudioManager.audioManagerInstance.PlaySFX(nodeClickedSFX);
    }

    public void PlayLevelCompleteSFX()
    {
        AudioManager.audioManagerInstance.PlaySFX(levelCompleteSFX);
    }

    public void PlayButtonClickedSFX()
    {
        AudioManager.audioManagerInstance.PlaySFX(buttonSFX);
    }
}
