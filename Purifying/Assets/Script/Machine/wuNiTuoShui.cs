using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class wuNiTuoShui : MonoBehaviour
{
    [Header("输入参数")]
    public float Q0;       // 脱水前污泥量 (m³/d)
    public float P1 = 97f;       // 脱水前污泥含水率 (%)

    public Text inputParametersText;  // 输入参数文本
    public Text outputParametersText; // 输出参数文本
    public Text adjustableParametersText; // 可调参数文本
    public Text machineMsg; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    [Header("可调参数")]
    private float P = 0.5f;         //  设备压力(MPa)
    private float t = 0.1f;         //  脱水时间(h)


    [Header("输出参数")]
    public float P2;        //脱水后污泥含水率（%）
    public float Q1;       // 脱水后污泥量 (m³/d)
    public float M;       // 脱水后干污泥重量 (kg/d)
    public float E;       // 设备能耗 (kwh/m³)

    public float Eff; //处理能力(m³/d)
    public float Cost; //处理能力(m³/d)


    [Header("UI控件")]
    public Slider PSlider;    // 设备压力调节
    public Slider tSlider;    // 脱水时间调节

    public bool Perr;
    public bool Eerr;

    public Vector3 pos;

    public ButtonHoverHandler hover;

    public float GetP()
        { return P; }

    public float Gett()
    { return t; }

    void OnEnable()
    {
        Q0 = WaterData.Instance.GetQwuni();

        // 初始化滑动条
        PSlider.minValue = 0.3f;
        PSlider.maxValue = 0.6f;
        PSlider.value = P;

        tSlider.minValue = 0.1f;
        tSlider.maxValue = 0.3f;
        tSlider.value = t;

        pos = this.transform.localPosition;


        // 添加监听事件
        PSlider.onValueChanged.AddListener(UpdateP);
        tSlider.onValueChanged.AddListener(Updatet);


        CalculateAAOParameters();

        UpdateParameterDisplay();
    }


    void UpdateP(float newValue)
    {
        P = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void Updatet(float newValue)
    {
        t = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    void UpdateParameterDisplay()
    {
        // 输入参数显示
        string inputStr =
                        $"脱水前污泥量: {(Q0 > Eff ? $"<color=#ff0000>{Q0:F2} m³/d </color>" : $"{Q0:F2} m³/d")}\n\n" +
                        $"脱水前污泥含水率: {P1:F2} d\n\n";

        // 可调参数显示
        string adjustableStr =
                            $"设备压力: {P:F1} MPa\n\n" +
                            $"脱水时间: {t:F1} h\n\n" ;

        //输出水源有关
        string outputStr =
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0} mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0} mg/L\n\n" +
                        $"出水TN: {WaterData.Instance.GetTNOUT():F1} mg/L\n\n" +
                        $"出水TP: {WaterData.Instance.GetTPOUT():F1} mg/L\n\n" +
                        $"出水SS: {WaterData.Instance.GetSSOUT():F3} mg/L";

        // 输出参数显示
        string machineStr =
                           $"脱水后污泥量: {Q1:F2} m³/d\n\n" +
                        $"脱水后污泥含水率: {(P2 >85 ? $"<color=#ff0000>{P2:F2} % (脱水后污泥含水率较高)</color>" : $"{P2:F2} % ")} \n\n"+ 
                        $"脱水后干污泥重量: {M:F2} kg/d\n\n" +
                        $"设备能耗: {(E > 1.0 ? $"<color=#cccc00>{E:F3} kwh/m³ (设备能耗较高)</color>" : $"{E:F3} kwh/m³ ")}" ;

        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (P2 <= 85 && E <= 1.0 && WaterData.Instance.GetCuGeshan()
                && WaterData.Instance.GetXiGeshan() && WaterData.Instance.GetAAO()
                && WaterData.Instance.GetErChenChi() && WaterData.Instance.GetShenChuangLvChi()
                && WaterData.Instance.GetgaoMiDu() && WaterData.Instance.GetZiWaiXiaoDu())
            {
                WaterData.Instance.SetProgress(100.0f);
                WaterData.Instance.SetWuNiTuoShui(true);
            }
            else
            {
                WaterData.Instance.SetProgress(90.0f);
                WaterData.Instance.SetWuNiTuoShui(false);
            }
        }



        string Qstr = $"设备处理能力:  {(Q0 > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";

        Perr = P2 > 85;
        Eerr = E > 1.0;

        if (Perr && (!hover.errMsg.Contains("脱水后污泥含水率较高")))
        {
            hover.errMsg.Add("脱水后污泥含水率较高");
        }
        else if (!Perr)
        {
            hover.errMsg.Remove("脱水后污泥含水率较高");
        }


        if (Eerr && (!hover.errMsg.Contains("设备能耗较高")))
        {
            hover.errMsg.Add("设备能耗较高");
        }
        else if (!Eerr)
        {
            hover.errMsg.Remove("设备能耗较高");
        }

        // 更新UI组件
        inputParametersText.text = inputStr;
        adjustableParametersText.text = adjustableStr;
        outputParametersText.text = outputStr;
        machineMsg.text = machineStr;
        QParametersText.text = Qstr;
    }

    void CalculateAAOParameters()
    {
        //脱水后污泥含水率
        float k = 4.71f;
        P2 = P1 * Mathf.Exp(-k * P * t)*1.2f;
        P2=Mathf.Clamp(P2, 70f, 92f);

        //设备能耗
        E = 6.25f * P * P * t*2f;

        //脱水后污泥量
        Q1 = Q0 * (100f - P1) / (100f - P2);
        M = Q1 * (1f - (P2/100f) ) * 1000f;

        Eff = 32 * Mathf.Pow(P, 0.5f) / Mathf.Pow(t, 2f);
        Cost = E * 0.6f * Q0;

        WaterData.Instance.setCost(Cost, 7);

    }
}
