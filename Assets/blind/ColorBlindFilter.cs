using UnityEngine;

public class ColorBlindFilter : MonoBehaviour
{
    public enum FilterType { Normal, Protanopia, Deuteranopia, Tritanopia }
    public FilterType currentFilter = FilterType.Normal;
    [Range(0f, 1f)] public float strength = 1f;

    private Material material;

    void Start()
    {
        Shader shader = Shader.Find("Hidden/ColorBlindnessFilter");
        if (shader != null)
        {
            material = new Material(shader);
        }
        else
        {
            Debug.LogError("找不到滤镜 Shader（Hidden/ColorBlindnessFilter）！");
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetInt("_FilterType", (int)currentFilter);
            material.SetFloat("_Strength", strength);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
