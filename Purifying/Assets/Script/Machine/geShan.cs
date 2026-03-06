using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GeShan : MonoBehaviour
{
    [Header("输入参数")]
    public float Q =15000f; //最大设计流量
    private float k = 70f; //水头损失增大倍数
    private float beta = 1.79f; //形状系数，取圆形栅条截面形状1.79
    private float n = 41; //栅条间隙数
    private float a = 60f; //格栅倾角

    [Header("可调参数")]
    private float S = 0.02f; //栅条宽度
    private float b = 0.05f; //栅条间隙

    [Header("输出参数")]
    public float h = 0.5f; //栅前水深
    public float v = 0.8f; //过栅水速
    public float B; //栅槽宽度
    public float h1; //水头损失
    public float E; //能耗
    public float Eff; //处理能力(m³/d)
    public float Cost; //处理能力(m³/d)

    [Header("UI控件")]
    public Slider SSlider;    // 栅条间隙调节
    public Slider bSlider;    // 栅条宽度调节
    public Slider QSlider;    // 进水流量调节

    [Header("输出文本")]
    public Text inputParametersText;  // 输入参数文本
    public Text outputMachineParametersText; // 输出设备有关
    public Text outputWaterParametersText; // 输出水源有关
    public Text adjustableParametersText; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    public bool verr = false;
    public bool herr = false;
    public bool Qerr = false;

    public Vector3 pos;

    public ButtonHoverHandler hover;


    public float GetS()
    {
        return S;
    }

    public float Getb()
    {
        return b;
    }

    // Start is called before the first frame update

    void OnEnable()
    {
        WaterData.Instance.SetQ(20000f);
        WaterData.Instance.setCost(0f,0);
        WaterData.Instance.setCost(0f,1);
        WaterData.Instance.setCost(0f,2);
        WaterData.Instance.setCost(0f,3);
        WaterData.Instance.setCost(0f,4);
        WaterData.Instance.setCost(0f,5);
        WaterData.Instance.setCost(0f,6);
        WaterData.Instance.setCost(0f,7);
        /*
        WaterData.Instance.SetTPOUT(5f);
        WaterData.Instance.SetSSOUT(300f);
        WaterData.Instance.SetTNOUT(40f);
        WaterData.Instance.SetBOD5OUT(150f);
        WaterData.Instance.SetCODOUT(300f);
        */

        WaterData.Instance.SetTPOUT(WaterData.Instance.GetTPTemp());
        WaterData.Instance.SetSSOUT(WaterData.Instance.GetSSTemp());
        WaterData.Instance.SetTNOUT(WaterData.Instance.GetTNTemp());
        WaterData.Instance.SetBOD5OUT(WaterData.Instance.GetBOD5Temp());
        WaterData.Instance.SetCODOUT(WaterData.Instance.GetCODTemp());

        // 初始化滑动条
        SSlider.minValue = 0.005f;
        SSlider.maxValue = 0.02f;
        SSlider.value = S;

        bSlider.minValue = 0.05f;
        bSlider.maxValue = 0.15f;
        bSlider.value = b;

        QSlider.maxValue = WaterData.Instance.GetQ();
        QSlider.minValue = 10000f;
        QSlider.value = Q;

        pos = this.transform.localPosition;

        // 添加监听事件
        SSlider.onValueChanged.AddListener(UpdateS);
        bSlider.onValueChanged.AddListener(Updateb);
        QSlider.onValueChanged.AddListener(UpdateQ);
        

        CalculateParameters();
        UpdateParameterDisplay();

    }

    // 更新参数显示
    void UpdateParameterDisplay()
    {

        // 输入参数显示
        string inputStr = 
                        $"最大流量: {20000:F2} m³/s\n\n" +
                        $"栅条数: {n}\n\n" +
                        $"倾角: {a}°";

        // 可调参数显示
        string adjustableStr = 
                            $"栅条宽度: {S * 1000:F0} mm\n\n" +
                            $"栅隙宽度: {b * 1000:F0} mm";

        // 输出设备有关
        string outputMachineStr = 
                        $"栅前水深: {h:F2} m\n\n" +
                        $"过栅流速: {(v > 1.0f ? $"<color=#ff0000>{v:F2} m/s (过栅流速过高)</color>" : $"{v:F2} m/s")}\n\n" +
                        $"槽宽: {B:F2} m\n\n" +
                        $"能耗: {E:F3} kwh/m³\n\n" +
                        //$"水头损失: {h1:F3} m";
                        $"水头损失: {(h1 > 1.0f ? $"<color=#ff0000>{h1:F2} m (水头损失过高)</color>" : $"{h1:F2} m")}";

        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (v < 1.0f && h1 < 1.0f&&Q<Eff)
            {
                WaterData.Instance.SetProgress(10.0f);
                WaterData.Instance.SetCuGeshan(true);
            }
            else
            {
                WaterData.Instance.SetProgress(0.0f);
                WaterData.Instance.SetCuGeshan(false);
            }
        }
        

        //输出水源有关
        string outputWaterStr = 
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0} mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0} mg/L\n\n" +
                        $"出水总氮: {WaterData.Instance.GetTNOUT():F1} mg/L\n\n" +
                        $"出水总磷: {WaterData.Instance.GetTPOUT():F1} mg/L";

        string Qstr = $"进水流量: {(Q > Eff ? $"<color=#ff0000>{Q:F2} m³/d </color>" : $"{Q:F2} m³/d")} \n\n" +
                        $"设备处理能力:  {(Q > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";

        verr = v > 1.0f;
        herr = h1 > 1.0f;
        Qerr = Q > Eff;

        if (verr && (!hover.errMsg.Contains("过栅流速过高")))
        {
            hover.errMsg.Add("过栅流速过高");
        }
        else if (!verr)
        {
            hover.errMsg.Remove("过栅流速过高");
        }

        if (herr && (!hover.errMsg.Contains("水头损失过高")))
        {
            hover.errMsg.Add("水头损失过高");
        }
        else if (!herr)
        {
            hover.errMsg.Remove("水头损失过高");
        }

        // 更新UI组件
        inputParametersText.text = inputStr;
        adjustableParametersText.text = adjustableStr;
        outputMachineParametersText.text = outputMachineStr;
        outputWaterParametersText.text = outputWaterStr;
        QParametersText.text = Qstr;
    }


    void UpdateS(float newValue)
    {
        S = newValue;
        CalculateParameters();
        UpdateParameterDisplay();
    }
    void Updateb(float newValue)
    {
        b = newValue;
        CalculateParameters();
        UpdateParameterDisplay();
    }

    void UpdateQ(float newValue)
    {
        Q = newValue;
        WaterData.Instance.SetQSave(0, Q);
        Debug.Log("QSaved:" + WaterData.Instance.GetQSave(0));
        CalculateParameters();
        UpdateParameterDisplay();
    }

    public float getQ()
    {
        return Q;
    }

    void CalculateParameters()
    {
        // 转换为弧度
        float radians = a * Mathf.Deg2Rad;
        //n = (int)(Mathf.Ceil((Q * Mathf.Sqrt(Mathf.Sin(a)) / (b * h * v))));
        B = S * (n - 1) + (b * n);
        v = ((Q/10000) * Mathf.Sqrt(Mathf.Sin(radians)) / (b * h * n));
        h1 = k * beta * Mathf.Pow((S / b), 4f / 3f) * (Mathf.Pow(v, 2f) / (2f * 9.8f)) * Mathf.Sin(radians);

        E = Mathf.Pow(1.0f/b,0.5f)*0.035f * S / (S + b);
        Eff=n*b*v*Mathf.Sin(a*3.14159f/180f)*7200;

        Cost = E * 0.6f * Q;

        WaterData.Instance.SetQ(Q);
        WaterData.Instance.setCost(Cost,0);

        // 输出调试信息
        Debug.Log($"水头损失: {h1} | " +
                 $"过栅水速: {v}m/s | " +
                 $"栅槽宽度: {B}m ");
    }

}
