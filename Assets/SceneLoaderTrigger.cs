using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderTrigger : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player is tagged as "Player"
        {
            SceneManager.LoadScene(sceneToLoad);
            Debug.LogWarning("Loading scene: " + sceneToLoad);
        }
    }
}

