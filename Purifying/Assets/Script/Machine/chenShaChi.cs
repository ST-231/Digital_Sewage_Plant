using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class chenShaChi : MonoBehaviour
{
    [Header("输入参数")]
    //设计参数
    private float Qmax; //最大流量
    private float Qmin; //最小流量
    private float v = 0.3f; //设计流速
    private int t = 45; //最大流量时停留时间

    private int X = 30; // 一般城市污水沉砂量
    private int T = 2; // 清除沉沙的间隔时间
    private float K = 1.5f; // 生活污水量总变化系数

    private float h = 0.96f; //有效水深

    [Header("可调参数")]
    private float a1 = 0.5f; //尘砂斗底宽
    private float h3 = 0.85f; //斗高
    private float theta = 55; //斗壁与水平面的倾角

    private int nmax = 3; //沉砂池格数，最大流速时启用三格
    private int nmin = 2; //最小流速时启用两格
    private float b = 2; //每格宽度

    [Header("输出参数")]
    private float L; //沉砂池长度
    private float A; //水流断面面积
    private float V; //沉砂室所需容积
    private float B; //池总宽度

    private float h2; //算出来的有效高度

    private float V0; //每个沉砂斗容积
    private float V1; //每个沉砂斗容积(验算)，V1大于V0则合规
    private float a; //沉砂斗上口宽


    private float vmin; //最小流速验证
    private float E; //能耗
    private float Eff; //处理能力(m³/d)

    public bool Verr=false;
    public bool Eerr=false;
    public bool vminerr = false;

    public Vector3 pos;

    [Header("UI控件")]
    public Slider a1Slider;    // 尘砂斗底宽调节
    public Slider h3Slider;    // 斗高调节
    public Slider thetaSlider;    // 斗壁与水平面的倾角调节

    [Header("输出文本")]
    public Text inputParametersText;  // 输入参数文本
    public Text outputMachineParametersText; // 输出设备有关
    public Text outputWaterParametersText; // 输出水源有关
    public Text adjustableParametersText; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    void Start()
    {
        Qmax = WaterData.Instance.GetQ();
        Qmin = WaterData.Instance.GetQmin();
        // 初始化滑动条
        a1Slider.minValue = 0.1f;
        a1Slider.maxValue = 1.0f;
        a1Slider.value = a1;

        h3Slider.minValue = 0.5f;
        h3Slider.maxValue = 1.5f;
        h3Slider.value = h3;

        thetaSlider.minValue = 1f;
        thetaSlider.maxValue = 5f;
        thetaSlider.value = b;

        pos = this.transform.localPosition;

        // 添加监听事件
        a1Slider.onValueChanged.AddListener(Updatea1);
        h3Slider.onValueChanged.AddListener(Updateh3);
        thetaSlider.onValueChanged.AddListener(Updateb);


        CalculateParameters();
        UpdateParameterDisplay();
    }

    // 更新参数显示
    void UpdateParameterDisplay()
    {
        // 输入参数显示
        string inputStr = 
                        $"最大流量: {Qmax:F2} m³/s\n\n" +
                        $"最小流量: {Qmin:F2} m³/s\n\n" +
                        $"设计流速: {v} m/s\n\n" +
                        $"最大流量时停留时间: {t}s";

        // 可调参数显示
        string adjustableStr = 
                            $"尘砂斗底宽调节: {a1:F2} m\n\n" +
                            $"斗高调节: {h3:F2} m\n\n" +
                            $"每格沉砂池宽度调节: {b:F1} m";

        // 输出设备有关
        string outputMachineStr = 
                        $"有效水深: {h2} m\n\n" +
                        $"每个沉砂斗容积: {V0:F2} m³\n\n" +
                        //$"每个沉砂斗容积(验算): {V1:F2} m³\n\n" +
                        $"每个沉砂斗容积(验算): {(V1 < V0 ? $"<color=#ff0000>{V1:F2} m³ (沉砂斗容积小于设计预期)</color>" : $"{V1:F2} m/s")}\n\n" +
                        $"能耗: {(E > 0.15 ? $"<color=#cccc00>{E:F3} kwh/m³ (设备能耗较高)</color>" : $"{E:F3} kwh/m³ ")} \n\n" +
                        //$"最小流速验证: {vmin:F3} m/s";
                        $"最小流速验证: {(vmin < (v / 2) ? $"<color=#ff0000>{vmin:F2} m³ (需大于二分之一的设计流速)</color>" : $"{vmin:F2} m/s")}";
        //输出水源有关
        string outputWaterStr = 
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0} mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0} mg/L\n\n" +
                        $"出水总氮: {WaterData.Instance.GetTNOUT():F1} mg/L\n\n" +
                        $"出水总磷: {WaterData.Instance.GetTPOUT():F1} mg/L";

        string Qstr = $"进水流量: {WaterData.Instance.GetQ():F2} m³/d\n\n" +
                        $"设备处理能力: {Eff:F2} m³/d";

        Verr = V1 < V0;
        Eerr= E > 0.15;
        vminerr= vmin < (v / 2);

        // 更新UI组件
        inputParametersText.text = inputStr;
        adjustableParametersText.text = adjustableStr;
        outputMachineParametersText.text = outputMachineStr;
        outputWaterParametersText.text = outputWaterStr;
    }


    void Updatea1(float newValue)
    {
        a1 = newValue;
        CalculateParameters();
        UpdateParameterDisplay();
    }
    void Updateh3(float newValue)
    {
        h3 = newValue;
        CalculateParameters();
        UpdateParameterDisplay();
    }

    void Updateb(float newValue)
    {
        b = newValue;
        CalculateParameters();
        UpdateParameterDisplay();
    }

    void CalculateParameters()
    {
        L = Mathf.Round(v * t * 100) / 100;
        A = Mathf.Round(Qmax / v * 100) / 100;
        B = nmax * b;
        h2 = Mathf.Round(A / B * 100) / 100;

        V = Mathf.Round((Qmax * X * T * 86400) / (K * 1000000) * 100) / 100;
        V0 = Mathf.Ceil(V / (nmax * b));

        // 转换为弧度
        float radians = theta * Mathf.Deg2Rad;
        a = 2 * h3 / Mathf.Tan(radians) + a1;

        V1 = h3 / 6 * (2 * Mathf.Pow(a, 2f) + 2 * a * a1 + 2 * Mathf.Pow(a1, 2f));//参数需要大于V0

        vmin = Qmin / (nmin * Mathf.Pow(b, 1.5f) * h2);//大于v/2，即二分之一的设计流速即可

        float Geo = (a1 + h3 * Mathf.Tan(theta*3.14159f/180f)) / b;

        Debug.Log("ss:" + WaterData.Instance.GetSSIn());

        float SSf = 1f + 0.002f * (WaterData.Instance.GetSSIn()*10f - 100f);

        E = 0.24f * (nmax / nmin) * Geo * SSf;

        Eff = v * b*nmin * h3 * 86400f;

        // 输出调试信息
        Debug.Log($"沉砂池长度: {L} | " +
                 $"水流断面面积: {A}m/s | " +
                 $"沉砂室所需容积: {V}m " +
                 $"有效水深: {h2}m " +
                 $"每个沉砂池容积: {V0}m " +
                 $"沉砂斗上口宽: {a}m " +
                 $"每个沉砂池容积(验算): {V1}m " +
                 $"最小流速验证: {vmin}m ");
        }
    
}
