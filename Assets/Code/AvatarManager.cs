using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class AvatarManager : MonoBehaviour {
    public string sceneName;
    public string nextSceneToLoad;
    public Transform headLocation;
    private List<GameObject> headParts = new List<GameObject>();
    private int headCount = 0;


    public Transform avatarHeadLocation;
    private List<GameObject> avatarHeadParts = new List<GameObject>();
    private int avatarHeadCount = 0;

    public Transform bodyLocation;
    private List<GameObject> bodyParts = new List<GameObject>();
    private int bodyCount = 0;

    public Transform feetLocation;
    private List<GameObject> feetParts = new List<GameObject>();
    private int feetCount = 0;

    public Transform avatarLocation;
    private List<GameObject> avatarParts = new List<GameObject>();
    private int avatarCount = 0;

    public Transform companionLocation;
    private List<GameObject> companionParts = new List<GameObject>();
    private int companionCount = 0;

    void Start(){
        Init();
    }

    void Init(){
        GetAllSprites();
        InitialiseHair();
        //SetActiveHeadSprite();
        SetActiveBodySprite();
        SetActiveFeetSprite();
        SetActiveAvatarSprite();
        SetActiveCompanionSprite();
    }

    #region GET ALL SPRITES
    /// <summary>
    /// Gets all sprites parented to the respective transform.
    /// Used once during the initialisation process to store all sprites. 
    /// </summary>
    private void GetAllSprites(){
        for(int i = 0; i < headLocation.childCount; i++){
            GameObject g = headLocation.GetChild(i).gameObject;
            headParts.Add(g);
        }
        for (int i = 0; i < avatarHeadLocation.childCount; i++){
            GameObject g = avatarHeadLocation.GetChild(i).gameObject;
            avatarHeadParts.Add(g);
        }
        for (int j = 0; j < bodyLocation.childCount; j++){
            GameObject g = bodyLocation.GetChild(j).gameObject;
            bodyParts.Add(g);
        }
        for(int k = 0; k < feetLocation.childCount; k++){
            GameObject g = feetLocation.GetChild(k).gameObject;
            feetParts.Add(g);
        }
        for(int l = 0; l < avatarLocation.childCount; l++){
            GameObject g = avatarLocation.GetChild(l).gameObject;
            avatarParts.Add(g);
        }
        for(int l = 0; l < companionLocation.childCount; l++){
            GameObject g = companionLocation.GetChild(l).gameObject;
            companionParts.Add(g);
        }
    }
    #endregion

    #region SET CURRENT SPRITE
    private void InitialiseHair(){
        if (PlayerPrefs.HasKey("hairSelection")){
            int hair = PlayerPrefs.GetInt("hairSelection");
            headLocation.GetChild(hair).gameObject.SetActive(true);
            headCount = hair;
        }
        else{
            headLocation.GetChild(0).gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Sets sprite at the current count index to display,
    /// Turns off all other sprites.
    /// </summary>
    private void SetActiveHeadSprite()
    {
        for (int j = 0; j < headParts.Count; j++)
        {
            if (j == headCount)
            {
                headParts[j].SetActive(true);
                avatarHeadParts[j].SetActive(true);
                PlayerPrefs.SetInt("hairSelection", j);
                continue;
            }
            headParts[j].SetActive(false);
            avatarHeadParts[j].SetActive(false);
        }
    }

    private void SetActiveBodySprite(){
        for(int j = 0; j < bodyParts.Count; j++){
            if(j == bodyCount){
                bodyParts[j].SetActive(true);
                continue;
            }
            bodyParts[j].SetActive(false);
        }
    }

    private void SetActiveFeetSprite(){
        for(int j = 0; j < feetParts.Count; j++){
            if(j == feetCount){
                feetParts[j].SetActive(true);
                continue;
            }
            feetParts[j].SetActive(false);
        }
    }

    private void SetActiveAvatarSprite(){
        for(int j = 0; j < avatarParts.Count; j++){
            if(j == avatarCount){
                avatarParts[j].SetActive(true);
                continue;
            }
            avatarParts[j].SetActive(false);
        }
    }

    private void SetActiveCompanionSprite(){
        for(int j = 0; j < companionParts.Count; j++){
            if(j == companionCount){
                companionParts[j].SetActive(true);
                continue;
            }
            companionParts[j].SetActive(false);
        }
    }
    #endregion

    #region UI BUTTON CONTROLS
    /// <summary>
    /// These methods need to be public to be able to be called in the editor.
    /// </summary>
    /// <param name="count"></param>
    public void CycleHeadBTN(int count){
        headCount += count;
        
        if (headCount >= headParts.Count)
        {
            headCount = 0;
        }
        else if (headCount <= -1)
        {
            headCount = headParts.Count - 1;
        }

        avatarHeadCount = headCount;

        SetActiveHeadSprite();
    }

    public void CycleBodyBTN(int count){
        bodyCount += count;

        if(bodyCount >= bodyParts.Count){
            bodyCount = 0;
        }else if(bodyCount <= -1){
            bodyCount = bodyParts.Count -1;
        }

        SetActiveBodySprite();
    }

    public void CycleFeetBTN(int count){
        feetCount += count;

        if(feetCount >= feetParts.Count){
            feetCount = 0;
        }else if(feetCount <= -1){
            feetCount = feetParts.Count -1;
        }

        SetActiveFeetSprite();
    }

    public void CycleAvatarBTN(int count){
        avatarCount += count;

        if(avatarCount >= avatarParts.Count){
            avatarCount = 0;
        }else if(avatarCount <= -1){
            avatarCount = avatarParts.Count -1;
        }

        SetActiveAvatarSprite();
    }

    public void CycleCompanionBTN(int count){
        companionCount += count;

        if(companionCount >= companionParts.Count){
            companionCount = 0;
        }else if(companionCount <= -1){
            companionCount = companionParts.Count -1;
        }

        SetActiveCompanionSprite();
    }

    public void FinishCustomisationBTN()
    {
        //GameManager.Instance.LoadNewScene(nextSceneToLoad);
        //GameManager.Instance.UnloadScene(sceneName);
        GameManager.Instance.LoadSceneWithFade(nextSceneToLoad, sceneName);
    }
    #endregion
}