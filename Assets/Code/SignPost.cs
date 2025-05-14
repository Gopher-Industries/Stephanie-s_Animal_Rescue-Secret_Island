using UnityEditor;
using UnityEngine;

public class SignPost : MonoBehaviour{
    public string sceneName;

    public void TravelTo(string sceneToLoad)
    {
        //GameManager.Instance.LoadNewScene(sceneToLoad);
        //GameManager.Instance.UnloadScene("WorldHub");
        GameManager.Instance.LoadSceneWithFade(sceneToLoad, "WorldHub");
    }
}
