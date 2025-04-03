using UnityEngine;
using UnityEditor;

public class PlayerPrefsManager : MonoBehaviour {

    [MenuItem("PlayerPrefs Manager/Delete All PlayerPrefs", priority = 1)]
    static void DeleteAll(){
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been deleted.");
    }

    [MenuItem("PlayerPrefs Manager/Delete 'hasPlayed' key")]
    static void DeleteHasPlayed(){
        if(PlayerPrefs.HasKey("hasPlayed")){
            PlayerPrefs.DeleteKey("hasPlayed");
            Debug.Log("Deleted the 'hasPlayed' key.");
        }else{
            Debug.Log("No key 'hasPlayed' found.");
        }
    }

}