using UnityEngine;
using System.IO;
using static EncryptionHelper;


[System.Serializable]
public class APIKeyData
{
    public string apiKey;
}
public class APIKeyManager : MonoBehaviour
{
    private static string GetFilePath()
    {
        // Use Application.dataPath to get the path to the 'Assets' folder, and then navigate to the root project directory
        string projectPath = Directory.GetParent(Application.dataPath).ToString();
        string assetsPath = Path.Combine(projectPath, "Assets");
        string configPath = Path.Combine(assetsPath, "Config");

        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath); // Create the UserSettings folder if it doesn't exist
        }

        // Return the full path to the json file within the UserSettings folder
        return Path.Combine(configPath, "config.json");
    }

    public static void SaveSettings(string secret)
    {
        string encryptedKey = EncryptionHelper.Encrypt(secret);
        APIKeyData config = new APIKeyData { apiKey = encryptedKey };
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(GetFilePath(), json);
        Debug.Log($"Settings saved to {GetFilePath()}");
    }

    public static string LoadSettings()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ConfigData config = JsonUtility.FromJson<ConfigData>(json);
            string decryptedKey = EncryptionHelper.Decrypt(config.apiKey);
            Debug.Log("✅ API Key Loaded: ");
            return decryptedKey;
        }
        else
        {
            Debug.Log("API Key not found");
            return "";
        }
    }
}
