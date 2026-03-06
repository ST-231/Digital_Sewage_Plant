using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public GameObject tooltip;           // 悬浮提示框的 GameObject
    public Text tooltipText;             // 提示框的 Text 组件
    public Vector2 offset = new Vector2(-50, 50); // 文本框相对于鼠标的偏移
    // Start is called before the first frame update
    private bool isHovering = false;     // 是否悬停中
    public Slider slider;

    private List<string> tips;

    void Start()
    {
        tips = new List<string>()
        {
        "调节进水流量，决定当前设备的每日净水量，值越大则设备<color=#ff0000>每日净水量</color>越高，但<color=#ff0000>成本</color>也会随之增大。" +
        "无法超过上一环节出水流量以及当前参数下<color=#ff0000>设备最大处理能力</color>",

        "调节栅条宽度，增加会使当前设备<color=#ff0000>能耗及成本</color>增大，<color=#ff0000>水头损失</color>增大。" +
        "值过小会使较大颗粒物通过，可能增大后续环节处理压力及故障风险",

        "调节栅条间隙宽度，增加会使当前设备<color=#ff0000>能耗及成本</color>减小，<color=#ff0000>水头损失</color>减小,<color=#ff0000>过栅流速</color>减小。" +
        "值过大会使较大颗粒物通过，可能增大后续环节处理压力及故障风险",

        "调节栅条宽度，增加会使当前设备<color=#ff0000>能耗及成本</color>增大，<color=#ff0000>水头损失</color>增大。" +
        "值过小会使较大颗粒物通过，可能增大后续环节处理压力及故障风险",

        "调节栅条间隙宽度，增加会使当前设备<color=#ff0000>能耗及成本</color>减小，<color=#ff0000>水头损失</color>减小,<color=#ff0000>过栅流速</color>减小。" +
        "值过大会使较大颗粒物通过，可能增大后续环节处理压力及故障风险",

        "调节硝化液回流到缺氧池的比例，增大会使设备<color=#ff0000>脱氮率</color>增加，但会使<color=#ff0000>能耗及成本</color>增大。过大可能破坏缺氧环境，过小可能抑制硝化菌活性",

        "调节污水在好氧池的停留时间，增大会提高<color=#ff0000>COD</color>，<color=#ff0000>BOD5</color>和<color=#ff0000>总磷</color>去除率，但会使<color=#ff0000>能耗</color>增大且<color=#ff0000>设备处理效率</color>降低。" +
        "过小可能导致系统抗冲击能力减弱",

        "调节好氧池溶解氧浓度，增大会使设备<color=#ff0000>脱氮率</color>增加，但会使<color=#ff0000>能耗及成本</color>增大。过大可能反而导致脱氮率降低",

        "调节活性污泥在污水处理系统中的平均停留时间，增大会使设备<color=#ff0000>脱氮率</color>增加，<color=#ff0000>脱磷率</color>降低，<color=#ff0000>成本</color>降低。若小于硝化菌世代时间会使脱氮率大幅降低",

        "调节回流污泥量，增大会使<color=#ff0000>污泥沉降速度</color>与<color=#ff0000>污泥龄</color>增加，<color=#ff0000>能耗及成本</color>增加。",

        "调节排出污泥量，增大会使<color=#ff0000>污泥龄</color>下降，<color=#ff0000>能耗及成本</color>增加，<color=#ff0000>SS去除率</color>增加。",

        "调节污水在二沉池中的停留时间，增大会使设备<color=#ff0000>有效水深</color>及<color=#ff0000>污泥龄</color>增加，<color=#ff0000>SS去除率</color>增加，<color=#ff0000>能耗及成本</color>增加；减小会提高<color=#ff0000>设备处理效率</color>。",

        "调节单位表面积处理的污水量，增大会节省<color=#ff0000>成本</color>，加速<color=#ff0000>污泥沉降</color>，但会使<color=#ff0000>SS去除率</color>降低。",

        "调节单位表面积处理的污水量，增大会使<color=#ff0000>设备处理效率</color>提高，但会使<color=#ff0000>SS去除率</color>降低。",

        "调节絮凝区搅拌强度，增大会提高<color=#ff0000>SS</color>和<color=#ff0000>总磷</color>的去除率，但会增加<color=#ff0000>能耗及成本</color>。",

        "调节污水的停留时间,增大会使设备<color=#ff0000>SS去除率</color>增加，<color=#ff0000>能耗及成本</color>增加；减小会提高<color=#ff0000>设备处理效率</color>。",

        "调节助凝剂等化学药剂投加量，增大会提高<color=#ff0000>SS</color>和<color=#ff0000>总磷</color>的去除率，但会增加<color=#ff0000>成本</color>。",

        "调节滤料层厚度，增大会提高<color=#ff0000>SS去除率</color>和<color=#ff0000>设备处理效率</color>。",

        "调节过水速度，增大会降低<color=#ff0000>SS去除率</color>和<color=#ff0000>设备处理效率</color>。",

        "调节反冲洗时的冲洗流量，增大会提高<color=#ff0000>SS去除率</color>和<color=#ff0000>设备处理效率</color>，但会增大<color=#ff0000>能耗及成本</color>。" ,

        "调节滤料粒径，增大会降低<color=#ff0000>SS去除率</color>和<color=#ff0000>设备处理效率</color>。",

        "调节两次反冲洗间的时间间隔，增大会减小设备<color=#ff0000>能耗及成本</color>，但也会减小<color=#ff0000>设备处理效率</color>。",

        "调节紫外线灯管功率，增大会使设备<color=#ff0000>粪大肠菌群去除率</color>增加，但也会增加设备<color=#ff0000>能耗及成本</color>。",

        "调节污水受紫外线照射时间，增大会使设备<color=#ff0000>粪大肠菌群去除率</color>增加，但也会增加设备<color=#ff0000>能耗及成本</color>。",

        "调节脱水机的脱水压力，增大会使设备<color=#ff0000>脱水率</color>提高，但也会增加设备<color=#ff0000>能耗及成本</color>。",

        "调节污泥脱水时间，增大会使设备<color=#ff0000>脱水率</color>提高，但也会增加设备<color=#ff0000>能耗及成本</color>。"

        };

        // 初始隐藏文本框
        tooltip.SetActive(false);
    }

    void Update()
    {
        if (isHovering)
        {
            // 实时更新文本框位置到鼠标附近
            UpdateTooltipPosition();
            // 更新文本框内容（例如显示 Slider 的值）
            tooltipText.text = tips[index];
        }
    }

    // 鼠标进入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        tooltip.SetActive(true);
    }

    // 鼠标离开时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        tooltip.SetActive(false);
    }

    // 更新文本框位置
    private void UpdateTooltipPosition()
    {
        // 获取鼠标的屏幕坐标
        Vector2 mousePos = Input.mousePosition;
        // 设置文本框位置（鼠标位置 + 偏移）
        tooltip.transform.position = mousePos + offset;
    }
}
