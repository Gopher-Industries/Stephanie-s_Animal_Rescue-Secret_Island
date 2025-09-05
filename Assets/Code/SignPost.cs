using UnityEngine;
using UnityEngine.SceneManagement;

public class SignPost : MonoBehaviour
{
    private string sceneName;

    private void Start() 
    {
        // get current scene on load
        sceneName = gameObject.scene.name;
    }

    public void TravelTo(string sceneToLoad)
    {
        if (sceneToLoad != sceneName)
        {
            GameManager.Instance.LoadSceneWithFade(sceneToLoad, sceneName);
        }
    }
}
