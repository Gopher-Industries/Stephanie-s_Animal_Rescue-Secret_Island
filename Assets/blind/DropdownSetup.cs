using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Dropdown))] // 自动添加必要组件
public class DropdownSetup : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Dropdown组件会自动获取，也可手动拖拽")]
    public Dropdown colorBlindDropdown;

    [Header("Dropdown Options")]
    [SerializeField] 
    private List<string> options = new List<string> { 
        "Normal Vision", 
        "Deuteranopia (Green-Blind)", 
        "Protanopia (Red-Blind)", 
        "Tritanopia (Blue-Blind)" 
    };

    void Awake()
    {
        // 自动获取组件（如果未手动赋值）
        if (colorBlindDropdown == null)
            colorBlindDropdown = GetComponent<Dropdown>();
        
        // 安全校验
        if (colorBlindDropdown == null)
        {
            Debug.LogError("Dropdown component not found!", this);
            return;
        }

        SetupDropdown();
    }

    void SetupDropdown()
    {
        // 清空现有选项
        colorBlindDropdown.ClearOptions();

        // 添加本地化选项（实际项目建议使用Localization系统）
        var localizedOptions = new List<string>();
        foreach (var option in options)
        {
            localizedOptions.Add(Localize(option)); // 伪代码，替换为实际本地化方法
        }

        // 添加选项
        colorBlindDropdown.AddOptions(localizedOptions);

        // 设置默认值
        colorBlindDropdown.value = 0;
        colorBlindDropdown.RefreshShownValue();

        // 添加事件监听
        colorBlindDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private string Localize(string key) 
    {
        // 这里应该是你的本地化系统实现
        return key; // 示例直接返回原文本
    }

    void OnDropdownValueChanged(int selectedIndex)
    {
        Debug.Log($"Selected option: {options[selectedIndex]} (Index: {selectedIndex})");

        // 实际项目中这里应该调用色盲滤镜控制器
        // ColorBlindManager.Instance.SetMode((ColorBlindMode)selectedIndex);
    }

    // 外部可通过代码修改选项
    public void UpdateOptions(List<string> newOptions)
    {
        options = newOptions;
        SetupDropdown();
    }

    void OnDestroy()
    {
        // 移除事件监听防止内存泄漏
        if (colorBlindDropdown != null)
            colorBlindDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }
}