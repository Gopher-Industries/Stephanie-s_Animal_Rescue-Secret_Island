using UnityEngine;
using UnityEditor;

public class PlayerPrefsManager : MonoBehaviour {

    [MenuItem("PlayerPrefs Manager/Delete All PlayerPrefs", priority = 1)]
    static void DeleteAll(){
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been deleted.");
    }

    [MenuItem("PlayerPrefs Manager/Set 'hasPlayed' key", priority = 50)]
    static void SetHasPlayed(){
        PlayerPrefs.SetInt("hasPlayer", 1);
        Debug.Log("Player has finished tutorial. Will begin at World hub.");
    }

    [MenuItem("PlayerPrefs Manager/Delete 'hasPlayed' key", priority = 51)]
    static void DeleteHasPlayed(){
        if(PlayerPrefs.HasKey("hasPlayed")){
            PlayerPrefs.DeleteKey("hasPlayed");
            Debug.Log("Deleted the 'hasPlayed' key.");
        }else{
            Debug.Log("No key 'hasPlayed' found.");
        }
    }

}