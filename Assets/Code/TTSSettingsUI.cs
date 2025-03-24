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
        //currentSettings = TTSSettings.LoadSettings();
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
    }
}
