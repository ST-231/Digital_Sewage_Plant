using UnityEngine;
using UnityEngine.UI;

public class ResolutionUI : MonoBehaviour
{
    public Dropdown resolutionDropdown;

    void Start()
    {
        // 获取可用分辨率
        Resolution[] resolutions = Screen.resolutions;

        // 清空现有的下拉框选项
        resolutionDropdown.ClearOptions();

        // 添加分辨率选项到下拉框
        var options = new System.Collections.Generic.List<string>();
        foreach (var res in resolutions)
        {
            options.Add(res.width + "x" + res.height);
        }
        resolutionDropdown.AddOptions(options);

        // 设置默认选项
        resolutionDropdown.value = options.IndexOf(Screen.currentResolution.width + "x" + Screen.currentResolution.height);

        // 添加事件监听
        resolutionDropdown.onValueChanged.AddListener(delegate {
            SetResolution(resolutions[resolutionDropdown.value]);
        });
    }

    void SetResolution(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
