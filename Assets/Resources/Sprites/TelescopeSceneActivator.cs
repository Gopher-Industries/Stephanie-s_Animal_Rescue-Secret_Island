using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TelescopeSceneActivator : MonoBehaviour
{
    public Image[] sceneImages; // Drag your 3 scene images into this array in the Inspector
    public float fadeDuration = 1.5f; // Fade duration

    public void ShowAllScenes()
    {
        foreach (Image img in sceneImages)
        {
            StartCoroutine(FadeInImage(img));
        }
    }

    IEnumerator FadeInImage(Image img)
    {
        img.gameObject.SetActive(true);
        Color color = img.color;
        color.a = 0f;
        img.color = color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            img.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        img.color = color;
    }
}
