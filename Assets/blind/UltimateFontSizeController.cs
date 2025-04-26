using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UltimateFontSizeController : MonoBehaviour
{
    [Header("大小设置")]
    public float normalSize = 24f;
    public float largeSize = 36f;
    
    [Header("调试模式")]
    public bool debugMode = true;
    
    private bool isLargeMode = false;
    
    // 按钮点击事件（绑定到UI按钮）
    public void OnToggleFontSize()
    {
        isLargeMode = !isLargeMode;
        float targetSize = isLargeMode ? largeSize : normalSize;
        
        StartCoroutine(ChangeAllTextSizes(targetSize));
        
        if (debugMode) 
            Debug.Log($"切换模式: {(isLargeMode ? "大字" : "正常")} 大小={targetSize}");
    }
    
    // 实际修改字体大小的协程
    private IEnumerator ChangeAllTextSizes(float size)
    {
        yield return null; // 等待一帧确保稳定性
        
        // 修改所有普通UI Text
        foreach (Text text in FindObjectsOfType<Text>(true))
        {
            text.fontSize = Mathf.RoundToInt(size);
            if (debugMode) Debug.Log($"修改 {text.name} 大小: {size}", text);
        }
        
        // 修改所有TextMeshPro
        foreach (TMP_Text tmp in FindObjectsOfType<TMP_Text>(true))
        {
            tmp.fontSize = size;
            if (debugMode) Debug.Log($"修改TMP {tmp.name} 大小: {size}", tmp);
        }
    }
}