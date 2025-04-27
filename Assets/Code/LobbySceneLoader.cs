using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("WorldHub");
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        Debug.LogWarning("Loading scene by index: " + sceneIndex);
        SceneManager.LoadScene(sceneIndex); // Loads the scene by index
    }
}