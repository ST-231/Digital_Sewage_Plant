using UnityEngine;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour
{
    public Button toggleButton;  // 关联你的按钮
    public GameObject imageObject; // 关联你的图片（Image组件的GameObject）

    private bool isImageVisible = false; // 记录图片的状态

    private void Start()
    {
        // 确保图片初始是隐藏的
        imageObject.SetActive(false);

        // 绑定按钮点击事件
        toggleButton.onClick.AddListener(ToggleImageVisibility);
    }

    private void ToggleImageVisibility()
    {
        isImageVisible = !isImageVisible; // 取反状态
        imageObject.SetActive(isImageVisible); // 切换图片显示状态
    }
}