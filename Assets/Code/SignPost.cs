using UnityEditor;
using UnityEngine;

public class SignPost : MonoBehaviour{
    public string sceneName;

    public void TravelTo(SceneAsset scene){
        GameManager.GameManagerInstance.LoadNewSceneUnloadOldScene(scene, "WorldHub");
    }
}
