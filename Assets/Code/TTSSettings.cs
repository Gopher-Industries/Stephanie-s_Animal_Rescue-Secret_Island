using UnityEngine;
using System.IO;

[System.Serializable]
public class TTSSettings
{
    public bool enableTTS = false;
    public string highlightColor = "#FFD700"; // Default color (Gold)
    public string voiceId = "Xb7hH8MSUJpSbSDYk0k2"; // Default voice

    private static string GetFilePath()
    {
        // Use Application.dataPath to get the path to the 'Assets' folder, and then navigate to the root project directory
        string projectPath = Directory.GetParent(Application.dataPath).ToString();
        string userSettingsPath = Path.Combine(projectPath, "UserSettings");

        if (!Directory.Exists(userSettingsPath))
        {
            Directory.CreateDirectory(userSettingsPath); // Create the UserSettings folder if it doesn't exist
        }

        // Return the full path to the json file within the UserSettings folder
        return Path.Combine(userSettingsPath, "tts_settings.json");
    }

    public static void SaveSettings(TTSSettings settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(GetFilePath(), json);
        Debug.Log($"Settings saved to {GetFilePath()}");
    }

    public static TTSSettings LoadSettings()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<TTSSettings>(json);
        }
        else
        {
            Debug.Log("Settings file not found, using defaults.");
            return new TTSSettings(); // Return default settings
        }
    }
}
