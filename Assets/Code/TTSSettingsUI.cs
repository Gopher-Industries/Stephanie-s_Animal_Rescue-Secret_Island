using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public class TTSSettingsUI : MonoBehaviour
{
    public TTSManager ttsManager;
    public Toggle ttsToggle;
    public TMP_Dropdown colorDropdown;
    public TMP_Dropdown voiceDropdown;
    public Button previewButton;
    public Button saveButton;
    //public TTSSettings ttsSettings;

    public TMP_Text statusText;
    //public AudioSource audioSource;
    //public TMP_Text displayText;

    private bool isTTSEnabled = false;
    private string selectedColor = "FFD700";  // Default color (Gold)
    private string selectedVoiceId = "Xb7hH8MSUJpSbSDYk0k2"; // Default voice
    private TTSSettings currentSettings = new TTSSettings();
    //private string apiKey = "sk_bbbcdf8007afc946e14b66b073fa9497d005a265f538fa41"; // Load securely in production

    private void Start()
    {
        // Set default options
        colorDropdown.AddOptions(new System.Collections.Generic.List<string> { "Gold", "Red", "Blue", "Green" });
        voiceDropdown.AddOptions(new System.Collections.Generic.List<string> { "Voice A", "Voice B", "Voice C" });

        // Assign default voice IDs
        voiceDropdown.onValueChanged.AddListener(delegate { UpdateVoiceID(); });
        colorDropdown.onValueChanged.AddListener(delegate { UpdateHighlightColor(); });

        // Enable/Disable components based on Toggle
        ttsToggle.onValueChanged.AddListener(delegate { ToggleTTS(ttsToggle.isOn); });

        //Save Settings
        saveButton.onClick.AddListener(() => SaveSettings());

        // Assign preview button action
        previewButton.onClick.AddListener(() => PreviewVoice());
    }

    private void ToggleTTS(bool isOn)
    {
        isTTSEnabled = isOn;
        colorDropdown.interactable = isOn;
        voiceDropdown.interactable = isOn;
        previewButton.interactable = isOn;
    }

    private void UpdateVoiceID()
    {
        string[] voiceIds = { "Xb7hH8MSUJpSbSDYk0k2", "9BWtsMINqrJLrRacOk9x", "pqHfZKP75CvOlQylNhV4" }; // Replace with actual voice IDs
        selectedVoiceId = voiceIds[voiceDropdown.value];
    }

    private void UpdateHighlightColor()
    {
        string[] colors = { "FFD700", "FF0000", "0000FF", "008000" }; // Hex codes for Gold, Red, Blue, Green
        selectedColor = colors[colorDropdown.value];
    }

    private void SaveSettings()
    {
        currentSettings.enableTTS = ttsToggle.isOn;
        currentSettings.highlightColor = selectedColor;
        currentSettings.voiceId = selectedVoiceId;

        TTSSettings.SaveSettings(currentSettings);
        Debug.Log("Settings Saved!");
    }
    private void PreviewVoice()
    {
        string text = "I sound like this";
        StartCoroutine(ttsManager.ConvertTextToSpeech(text, selectedVoiceId, selectedColor)); ;
        statusText.text = "Playing preview...";
        /*string text = "I sound like this";
        string apiUrl = $"https://api.elevenlabs.io/v1/text-to-speech/{selectedVoiceId}?output_format=pcm_24000";
        string jsonPayload = $"{{ \"text\": \"{text}\", \"model_id\": \"eleven_multilingual_v2\" }}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                statusText.text = $"Error: {request.error}";
            }
            else
            {
                byte[] audioData = request.downloadHandler.data;
                PlayAudio(audioData,text);
                statusText.text = "Playing preview...";
            }
        }*/
    }

    /*private void PlayAudio(byte[] audioData, string text)
    {
        int sampleRate = 24000;
        int channels = 1;

        float[] samples = ConvertByteArrayToFloatArray(audioData);
        AudioClip clip = AudioClip.Create("TTSClip", samples.Length, channels, sampleRate, false);
        clip.SetData(samples, 0);

        audioSource.clip = clip;
        audioSource.Play();

        StartCoroutine(HighlightTextAsSpoken(text, clip.length));
    }

    private float[] ConvertByteArrayToFloatArray(byte[] byteArray)
    {
        int numSamples = byteArray.Length / 2;
        float[] samples = new float[numSamples];

        for (int i = 0; i < numSamples; i++)
        {
            short sample = (short)(byteArray[i * 2] | (byteArray[i * 2 + 1] << 8));
            samples[i] = sample / 32768.0f;
        }

        return samples;
    }

    private IEnumerator HighlightTextAsSpoken(string text, float duration)
    {
        string[] words = text.Split(' ');
        float timePerWord = duration / words.Length;

        // Display the full text first
        displayText.text = text;

        // Wait before starting the highlighting
        yield return new WaitForSeconds(0.5f);

        // Loop through each word and add a background color
        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            displayText.text = displayText.text.Replace(word, $"<color=#{selectedColor}>{word}</color>"); // Text color

            // Wait before moving to the next word
            yield return new WaitForSeconds(timePerWord);
        }
        displayText.text = "";
    }*/

}
