using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager GameManagerInstance {get; private set;}
    public SceneAsset storyChapter1;
    public SceneAsset worldHub;

    void Awake(){
        if(GameManagerInstance != null && GameManagerInstance != this){
            Destroy(this);
        }else{
            GameManagerInstance = this;
        }

        if(SceneManager.sceneCount == 1){
            Debug.Log("Only GameManager loaded. Loading Main menu.");
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        }
    }

    //Async allows some type of feedback on percent complete, Will allow a loading bar 
    public void MainMenuPlayButon(){
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        if(PlayerPrefs.HasKey("hasPlayed")){
            Debug.Log("Player has completed 'tutorial', loading world hub scene.");
            SceneManager.LoadSceneAsync(worldHub.name, LoadSceneMode.Additive);
        }else{
            Debug.Log("First time playing. Starting Tutorial");
            SceneManager.LoadSceneAsync(storyChapter1.name, LoadSceneMode.Additive);
        }
    }

    public void LoadNewSceneUnloadOldScene(SceneAsset sceneToLoad, string sceneToUnload){
        SceneManager.UnloadSceneAsync(sceneToUnload);
        SceneManager.LoadSceneAsync(sceneToLoad.name, LoadSceneMode.Additive);
    }

    public void LoadMiniGame(SceneAsset minigameToLoad){
        SceneManager.LoadSceneAsync(minigameToLoad.name, LoadSceneMode.Additive);
    }

    public void UnloadMiniGame(){
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void SetActiveScene(string sceneName){
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);
    }

}