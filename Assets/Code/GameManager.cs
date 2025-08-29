using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {get; private set;}
    public string storyChapter1;
    public string worldHub;


    public AnimationCurve loadingCurve;
    public Slider loadingBar;
    public CanvasGroup loadingScreenGroup;
    public float fadeTime = 1.0f;
    public TextMeshProUGUI loadPercentText;

    private bool isLoadingScene = false;

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
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        if (PlayerPrefs.HasKey("hasPlayed"))
        {
            Debug.Log("Player has completed 'tutorial', loading world hub scene.");
            //SceneManager.LoadSceneAsync(worldHub, LoadSceneMode.Additive);
            LoadSceneWithFade(worldHub, "MainMenu");
        }
        else
        {
            Debug.Log("First time playing. Starting Tutorial");
            //SceneManager.LoadSceneAsync(storyChapter1, LoadSceneMode.Additive);
            LoadSceneWithFade(storyChapter1, "MainMenu");
        }
    }

    /*
    public void LoadNewScene(string sceneToLoad)
    {
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }*/

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

    public void LoadSceneWithFade(string sceneToLoad, string sceneToUnload)
    {
        if (isLoadingScene)
        {
            return;
        }

        // lock the coroutine until it completes
        isLoadingScene = true;
        StartCoroutine(LoadSceneWithFadeRoutine(sceneToLoad, sceneToUnload));
    }

    IEnumerator LoadSceneWithFadeRoutine(string sceneToLoad, string sceneToUnload){
        loadingBar.value = 0f;
        yield return StartCoroutine(SceneFade(true));

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        
        while (async.progress < 0.9f){
            loadingBar.value = async.progress;
            //Debug.Log(async.progress * 100);
            loadPercentText.text = (async.progress * 100).ToString("F0");
            yield return null;
        }

        // Wait 0.2s to finish bar fill
        // This is a fake wait time.
        loadingBar.value = 1f;
        loadPercentText.text = "100";
        yield return new WaitForSeconds(0.2f);

        async.allowSceneActivation = true;
        yield return new WaitUntil(() => async.isDone);

        //** Potentially set new active scene here instead of directly in scene. **
        Scene newlyLoadedScene = SceneManager.GetSceneByName(sceneToLoad);
        if (newlyLoadedScene.IsValid()){
            SceneManager.SetActiveScene(newlyLoadedScene);
        }

        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            yield return SceneManager.UnloadSceneAsync(sceneToUnload);
        }


        yield return StartCoroutine(SceneFade(false));

        // reset the flag
        isLoadingScene = false;
    }


    IEnumerator SceneFade(bool value){
        float alphaStart = value ? 0 : 1;
        float alphaEnd = value ? 1 : 0;

        loadingScreenGroup.alpha = alphaStart;
        loadingScreenGroup.interactable = true;
        loadingScreenGroup.blocksRaycasts = true;

        float currentTime = 0;

        while (currentTime < fadeTime){
            currentTime += Time.deltaTime;
            float t = currentTime / fadeTime;
            float curveValue = loadingCurve.Evaluate(t);
            loadingScreenGroup.alpha = Mathf.Lerp(alphaStart, alphaEnd, curveValue);

            yield return null;
        }

        loadingScreenGroup.alpha = alphaEnd;
        if (!value){ 
            loadingScreenGroup.interactable = false;
            loadingScreenGroup.blocksRaycasts = false;
        }
    }
        
}