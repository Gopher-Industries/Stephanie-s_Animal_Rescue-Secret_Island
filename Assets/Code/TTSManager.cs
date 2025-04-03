using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.UI;
[System.Serializable]
public class ConfigData
{
    public string apiKey;
}

public class TTSManager : MonoBehaviour
{
    private string apiKey = "";
    private string voiceId = "Xb7hH8MSUJpSbSDYk0k2";   // Default voice ID
    public TMP_Text displayText;
    public AudioSource audioSource;
    public string highlightColor = "FF0000"; // Default color (Red)
    private TTSSettings settings;
    private APIKeyManager apiKeyManager;

    private void Awake()
    {
        string apiKey;
        if (this.apiKey != "")
        {
            apiKey = this.apiKey;
            APIKeyManager.SaveSettings(apiKey);
        }
        else
        {
            apiKey = APIKeyManager.LoadSettings();
            if (apiKey == "")
            {
                Debug.LogError("API Key not found. Please enter your API Key in the Config window.");
            }
            else
            {
                this.apiKey = apiKey;
            }
        }
    }
    private void Start()
    {
        settings = TTSSettings.LoadSettings();
        ApplySettings();
    }

    private void ApplySettings()
    {
        if (settings.enableTTS)
        {
            highlightColor = settings.highlightColor;
            voiceId = settings.voiceId;
            StartCoroutine(ConvertTextToSpeech(displayText.text, voiceId, highlightColor));
        }
    }

    public IEnumerator ConvertTextToSpeech(string text, string voiceId, string color)
    {
        highlightColor = color;
        this.voiceId = voiceId;
        string apiUrl = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}?output_format=pcm_24000";
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
                Debug.LogError($"Error: {request.error}\nResponse: {request.downloadHandler.text}");
            }
            else
            {
                Debug.Log("TTS Request Successful");
                byte[] audioData = request.downloadHandler.data;
                //Debug.Log($"Audio Data Length: {audioData.Length}");
                PlayAudio(audioData, text);
            }
        }
    }

    private void PlayAudio(byte[] audioData, string text)
    {
        int sampleRate = 24000; // Assuming 44.1 kHz sample rate
        int channels = 1; // Assuming mono audio

        // Convert byte array to float array (PCM 16-bit)
        float[] samples = ConvertByteArrayToFloatArray(audioData);

        // Create an AudioClip
        AudioClip clip = AudioClip.Create("TTSClip", samples.Length, channels, sampleRate, false);
        clip.SetData(samples, 0);

        // Play the audio
        audioSource.clip = clip;
        audioSource.Play();

        // Highlight text as it is spoken
        StartCoroutine(HighlightTextAsSpoken(text, clip.length));
    }

    // Converts byte array (16-bit PCM) to float array (-1.0 to 1.0 range)
    private float[] ConvertByteArrayToFloatArray(byte[] byteArray)
    {
        int numSamples = byteArray.Length / 2; // 2 bytes per sample (16-bit)
        float[] samples = new float[numSamples];

        for (int i = 0; i < numSamples; i++)
        {
            short sample = (short)(byteArray[i * 2] | (byteArray[i * 2 + 1] << 8)); // Convert bytes to short (PCM 16-bit)
            samples[i] = sample / 32768.0f;  // Normalize to -1.0 to 1.0
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
            displayText.text = displayText.text.Replace(word, $"<color=#{highlightColor}>{word}</color>"); // Background color

            // Wait before moving to the next word
            yield return new WaitForSeconds(timePerWord);
        }
    }


}




