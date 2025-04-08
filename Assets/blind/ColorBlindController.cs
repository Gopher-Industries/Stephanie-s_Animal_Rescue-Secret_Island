using UnityEngine;
using UnityEngine.UI;

public class ColorBlindController : MonoBehaviour
{
    public Material material;  // 色盲Shader的材质
    public enum ColorBlindType { None, Deuteranopia, Protanopia, Tritanopia }
    public ColorBlindType colorBlindType;

    // UI 控件
    public Dropdown colorBlindDropdown;
    public Toggle deuteranopiaToggle;
    public Toggle protanopiaToggle;
    public Toggle tritanopiaToggle;

    void Start()
    {
        // 检查材质和UI元素是否已经赋值
        if (material == null)
        {
            Debug.LogError("Material is not assigned in the inspector!");
            return;
        }

        if (colorBlindDropdown == null)
        {
            Debug.LogError("Dropdown is not assigned in the inspector!");
            return;
        }

        if (deuteranopiaToggle == null || protanopiaToggle == null || tritanopiaToggle == null)
        {
            Debug.LogError("One or more Toggles are not assigned in the inspector!");
            return;
        }

        // 添加 UI 监听事件
        colorBlindDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        deuteranopiaToggle.onValueChanged.AddListener(OnToggleValueChanged);
        protanopiaToggle.onValueChanged.AddListener(OnToggleValueChanged);
        tritanopiaToggle.onValueChanged.AddListener(OnToggleValueChanged);

        // 初始化状态
        SetColorBlindMode(ColorBlindType.None);
    }

    void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0: SetColorBlindMode(ColorBlindType.None); break;
            case 1: SetColorBlindMode(ColorBlindType.Deuteranopia); break;
            case 2: SetColorBlindMode(ColorBlindType.Protanopia); break;
            case 3: SetColorBlindMode(ColorBlindType.Tritanopia); break;
        }
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (deuteranopiaToggle.isOn) SetColorBlindMode(ColorBlindType.Deuteranopia);
        else if (protanopiaToggle.isOn) SetColorBlindMode(ColorBlindType.Protanopia);
        else if (tritanopiaToggle.isOn) SetColorBlindMode(ColorBlindType.Tritanopia);
        else SetColorBlindMode(ColorBlindType.None);
    }

    void SetColorBlindMode(ColorBlindType type)
    {
        colorBlindType = type;
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (material != null)
        {
            material.SetFloat("_ColorBlindType", (float)colorBlindType);
        }
    }
}
