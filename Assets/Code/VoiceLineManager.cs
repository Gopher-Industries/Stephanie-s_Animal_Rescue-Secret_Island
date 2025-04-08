using UnityEngine;

public class VoicelineManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip playVoiceline;
    [SerializeField] private AudioClip[] movementVoicelines;
    [SerializeField] private CharacterMover characterMoverScript;

    private const float VOICELINE_COOLDOWN = 10f;
    private float lastPlayedTime = 0f;


    void Start(){
        lastPlayedTime = Time.time - VOICELINE_COOLDOWN;

        audioSource = GetComponent<AudioSource>();

        if (characterMoverScript == null)
        {
            //Debug.LogError("ClickToMove script not assigned!");
        }

        //PlayInitialVoiceline();
    }

    // Check if the voiceline is on cooldown
    public bool CanPlayVoiceline()
    {
        return (Time.time - lastPlayedTime) >= VOICELINE_COOLDOWN;
    }

    // Play the initial voiceline when the player presses play
    public void PlayInitialVoiceline()
    {
        Debug.Log("PlayInitialVoiceline method called.");
        if (CanPlayVoiceline())
        {
            Debug.Log("Playing initial voiceline.");
            audioSource.PlayOneShot(playVoiceline);
            lastPlayedTime = Time.time;
        }
        else
        {
            Debug.Log("Initial voiceline failed to play");
        }
    }

    // Plays a random voiceline if cooldown has passed and the player is moving
    public void PlayMovementVoiceline()
    {
        if (movementVoicelines == null || movementVoicelines.Length == 0) { return; }

        if (characterMoverScript != null && characterMoverScript.isMoving && CanPlayVoiceline())
        {
            int randomIndex = Random.Range(0, movementVoicelines.Length);
            audioSource.PlayOneShot(movementVoicelines[randomIndex]);

            lastPlayedTime = Time.time;
        }
    }


    void Update()
    {
        PlayMovementVoiceline();
    }
}
