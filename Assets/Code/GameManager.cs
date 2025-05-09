using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {get; private set;}
    public string storyChapter1;
    public string worldHub;

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(this);
        }else{
            Instance = this;
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
            SceneManager.LoadSceneAsync(worldHub, LoadSceneMode.Additive);
        }else{
            Debug.Log("First time playing. Starting Tutorial");
            SceneManager.LoadSceneAsync(storyChapter1, LoadSceneMode.Additive);
        }
    }

    public void LoadNewScene(string sceneToLoad){
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }

    public void UnloadScene(string sceneToUnload){
        SceneManager.UnloadSceneAsync(sceneToUnload);
    }

    public void LoadMiniGame(string minigameToLoad){
        SceneManager.LoadSceneAsync(minigameToLoad, LoadSceneMode.Additive);
    }

    public void UnloadMiniGame(){
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void SetActiveScene(string sceneName){
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);
    }

}