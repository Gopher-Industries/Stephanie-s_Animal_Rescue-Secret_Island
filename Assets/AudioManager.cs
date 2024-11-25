using UnityEngine;
 
public class AudioManager : MonoBehaviour
{
    // Reference to the AudioSource component
    private AudioSource audioSource;
 
    // Audio clip to play
    [SerializeField]
    private AudioClip audioClip;
 
    // Volume of the sound (optional)
    [Range(0f, 1f)]
    public float volume = 1f;
 
    void Awake()
    {
        // Ensure there is an AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
 
        // Configure the AudioSource settings
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
    }
 
    void Start()
    {
        // Play the audio clip when the scene starts
        PlaySound();
    }
 
    // Function to play the sound
    public void PlaySound()
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: No audio clip assigned!");
        }
    }
}