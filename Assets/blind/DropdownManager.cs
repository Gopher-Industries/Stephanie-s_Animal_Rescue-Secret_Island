using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Dropdown))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class DropdownManager : MonoBehaviour
{
    [Header("Dropdown Options")]
    public List<string> options = new List<string>
    {
        "Normal Mode",
        "Deuteranopia (Green-Blind)",
        "Protanopia (Red-Blind)",
        "Tritanopia (Blue-Yellow)"
    };

    [Header("Visual Feedback")]
    public Image backgroundFeedback;
    public bool enableColorFeedback = true;
    public bool logSelection = true;

    [Header("Font Settings")]
    public Font fallbackFont; // 在Inspector中拖入任意字体

    private Dropdown _dropdown;
    private RectTransform _rectTransform;
    private Image _dropdownImage;
    private Text _captionText;

    void Awake()
    {
        // 安全获取组件
        _dropdown = GetComponent<Dropdown>() ?? gameObject.AddComponent<Dropdown>();
        _rectTransform = GetComponent<RectTransform>();
        _dropdownImage = GetComponent<Image>();

        InitializeRequiredComponents();
        SetupDropdown();
    }

    void InitializeRequiredComponents()
    {
        // 创建或获取Caption Text
        if (_dropdown.captionText == null)
        {
            GameObject captionObj = new GameObject("Caption");
            captionObj.transform.SetParent(transform, false);
            
            _captionText = captionObj.AddComponent<Text>();
            _captionText.font = GetSafeFont();
            _captionText.alignment = TextAnchor.MiddleLeft;
            _captionText.color = Color.black;
            _dropdown.captionText = _captionText;
        }
        else
        {
            _captionText = _dropdown.captionText;
        }

        // 创建或获取箭头图标
        if (_dropdown.transform.Find("Arrow") == null)
        {
            CreateDropdownArrow();
        }
    }

    Font GetSafeFont()
    {
        // 1. 优先使用Inspector指定的字体
        if (fallbackFont != null) return fallbackFont;

        // 2. 尝试获取默认字体
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont != null) return defaultFont;

        // 3. 使用当前场景中的第一个字体（应急方案）
        Font[] sceneFonts = Resources.FindObjectsOfTypeAll<Font>();
        if (sceneFonts.Length > 0) return sceneFonts[0];

        // 4. 最终回退：使用默认UI字体
        return Resources.GetBuiltinResource<Font>("Arial.ttf"); // 某些Unity版本仍支持
    }

    void CreateDropdownArrow()
    {
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.SetParent(transform, false);
        
        Image arrowImage = arrowObj.AddComponent<Image>();
        arrowImage.rectTransform.sizeDelta = new Vector2(20, 20);
        arrowImage.rectTransform.anchorMin = new Vector2(1, 0.5f);
        arrowImage.rectTransform.anchorMax = new Vector2(1, 0.5f);
        arrowImage.rectTransform.pivot = new Vector2(1, 0.5f);
        arrowImage.rectTransform.anchoredPosition = new Vector2(-10, 0);

        // 使用简单三角形作为箭头
        arrowImage.sprite = CreateTriangleSprite();
    }

    Sprite CreateTriangleSprite()
    {
        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        // 绘制简单三角形
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                bool inTriangle = (x > y) && (x < 32 - y);
                pixels[y * 32 + x] = inTriangle ? Color.black : Color.clear;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), Vector2.zero);
    }

    void SetupDropdown()
    {
        // 基础设置
        _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _rectTransform.sizeDelta = new Vector2(200, 40);

        // 设置反馈目标
        if (backgroundFeedback == null) 
            backgroundFeedback = _dropdownImage;

        // 填充选项
        _dropdown.ClearOptions();
        _dropdown.AddOptions(options);
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateVisualFeedback(0, true);
    }

    void UpdateVisualFeedback(int index, bool forceUpdate = false)
    {
        if (!forceUpdate && !gameObject.activeInHierarchy)
            return;

        // 安全更新文本
        if (_captionText != null)
            _captionText.text = $"<b>{options[index]}</b>";
        else if (_dropdown.captionText != null)
            _dropdown.captionText.text = $"<b>{options[index]}</b>";

        // 安全更新箭头
        Transform arrow = _dropdown.transform.Find("Arrow");
        if (arrow != null)
        {
            Image arrowImage = arrow.GetComponent<Image>();
            if (arrowImage != null)
                arrowImage.color = index == 0 ? Color.gray : Color.white;
        }
    }

    void OnDropdownValueChanged(int index)
    {
        UpdateVisualFeedback(index);

        if (enableColorFeedback && backgroundFeedback != null)
        {
            Color[] feedbackColors = new Color[]
            {
                Color.white,
                new Color(0.6f, 1f, 0.6f, 1f),
                new Color(1f, 0.6f, 0.6f, 1f),
                new Color(0.6f, 0.6f, 1f, 1f)
            };
            backgroundFeedback.color = feedbackColors[index];
        }

        if (logSelection)
            Debug.Log($"Selected: {options[index]} (Index: {index})");
    }

    void OnDestroy()
    {
        if (_dropdown != null)
            _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/UI/Color Blind Dropdown")]
    static void CreateDefaultDropdown()
    {
        GameObject obj = new GameObject("ColorBlind Dropdown");
        obj.AddComponent<DropdownManager>();
        if (UnityEditor.Selection.activeTransform != null)
            obj.transform.SetParent(UnityEditor.Selection.activeTransform);
    }
    #endif
}