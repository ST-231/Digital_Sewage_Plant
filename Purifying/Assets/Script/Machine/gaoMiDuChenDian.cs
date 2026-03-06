using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class gaoMiDuChenDian : MonoBehaviour
{

    [Header("输入参数")]  
    public float TPIn;        // 进水总磷 (mg/L)
    public float SSIn;        // 进水SS (mg/L)
    public float Q;        // 进水SS (mg/L)
    public float Qout;        // 进水SS (mg/L)

    public Text inputParametersText;  // 输入参数文本
    public Text outputParametersText; // 输出参数文本
    public Text adjustableParametersText; // 可调参数文本
    public Text machineMsg; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    [Header("可调参数")]
    private float q = 5.6f;       // 表面负荷 (m³/(m²·h))
    private float G = 200f;       // 搅拌强度 (s⁻¹)
    private float HRT = 1.5f;         // 水力停留时间 (h)
    private float C = 10f;       // 药剂投加量 (mg/L)


    [Header("输出参数")]
    public float Rs;        //SS去除率（%）
    public float Rtp;        //TP去除率（%）
    public float E;        //设备能耗（kWh/m³）
    public float TPOut;        // 出水总磷 (mg/L)
    public float SSOut;        // 出水SS (mg/L)
    public float Ccost;        // 药剂费用 (元/m³)
    public float Eff; //处理能力(m³/d)
    public float Cost;
    public float ita;

    [Header("UI控件")]
    public Slider qSlider;    // 设备压力调节
    public Slider GSlider;    // 脱水时间调节
    public Slider HRTSlider;    // 脱水时间调节
    public Slider CSlider;    // 脱水时间调节
    public Slider QSlider;    // 进水流量调节

    public bool Rserr=false;
    public bool Rtperr=false;
    public bool Eerr=false;

    public Vector3 pos;

    public ButtonHoverHandler hover;

    public float Getq()
    {
        return q;
    }

    public float GetG()
    {
        return G;
    }

    public float GetC()
    {
        return C;
    }

    public float GetHRT()
    {
        return HRT;
    }

    void OnEnable()
    {
        TPIn = WaterData.Instance.GetTPSave(0);        // 进水总磷 (mg/L)
        SSIn = WaterData.Instance.GetSSSave(0);

    // 初始化滑动条
    qSlider.minValue = 3f;
        qSlider.maxValue = 6f;
        qSlider.value = q;

        GSlider.minValue = 100f;
        GSlider.maxValue = 300f;
        GSlider.value = G;

        HRTSlider.minValue = 1f;
        HRTSlider.maxValue = 2f;
        HRTSlider.value = HRT;

        CSlider.minValue = 10f;
        CSlider.maxValue = 50f;
        CSlider.value = C;

        QSlider.maxValue = WaterData.Instance.GetQSave(3);
        QSlider.minValue = QSlider.maxValue > 10000f ? 10000f : QSlider.maxValue - 1000f;
        Q = (QSlider.maxValue + QSlider.minValue) / 2;
        QSlider.value = Q;

        pos = this.transform.localPosition;


        // 添加监听事件
        qSlider.onValueChanged.AddListener(Updateq);
        GSlider.onValueChanged.AddListener(UpdateG);
        HRTSlider.onValueChanged.AddListener(UpdateHRT);
        CSlider.onValueChanged.AddListener(UpdateC);
        QSlider.onValueChanged.AddListener(UpdateQ);


        CalculateAAOParameters();

        UpdateParameterDisplay();
    }


    void Updateq(float newValue)
    {
        q = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateG(float newValue)
    {
        G = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateHRT(float newValue)
    {
        HRT = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateC(float newValue)
    {
        C = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateQ(float newValue)
    {
        Q = newValue;
        WaterData.Instance.SetQSave(4, Q);
        Debug.Log("QSaved:" + WaterData.Instance.GetQSave(4));
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    void UpdateParameterDisplay()
    {
        // 输入参数显示
        string inputStr =
                        $"进水COD: {WaterData.Instance.GetCODIn():F2} mg/L\n\n" +
                        $"进水BOD5: {WaterData.Instance.GetBOD5In():F2} mg/L\n\n" +
                        $"进水总氮: {WaterData.Instance.GetTNIn()} mg/L\n\n" +
                        $"进水总磷: {WaterData.Instance.GetTPIn()} mg/L\n\n" +
                        $"进水SS: {SSIn:F2} mg/L\n\n";

        // 可调参数显示
        string adjustableStr =
                            $"表面负荷调节: {q:F1} m³/(m²·h)\n\n" +
                            $"搅拌强度调节: {G:F1} s⁻¹\n\n" +
                            $"水力停留时间调节: {HRT:F1} h\n\n" +
                            $"药剂投加量调节: {C:F1} mg/L";

        //输出水源有关
        string outputStr =
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0} mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0} mg/L\n\n" +
                        $"出水TN: {WaterData.Instance.GetTNOUT():F1} mg/L\n\n" +
                        $"出水TP: {WaterData.Instance.GetTPSave(1):F1} mg/L\n\n" +
                        $"出水SS: {WaterData.Instance.GetSSSave(1):F3} mg/L";

        // 输出参数显示
        string machineStr =
                        $"SS去除率: {(Rs < 60 ? $"<color=#ff0000>{Rs:F2} % (SS去除率较低)</color>" : $"{Rs:F2} % ")} \n\n" +
                        $"能耗: {(E > 0.2 ? $"<color=#cccc00>{E:F3} kwh/m³ (设备能耗较高)</color>" : $"{E:F3} kwh/m³ ")}  \n\n" +
                        $"药剂成本: {Ccost:F3} 元/m³ \n\n" +
                        $"TP去除率: {(Rtp < 70 ? $"<color=#ff0000>{Rtp:F2} % (TP去除率较低)</color>" : $"{Rtp:F2} % ")} " ;


        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (Rs >= 60 && E <= 0.2 && Rtp >= 70 && WaterData.Instance.GetCuGeshan()
                && WaterData.Instance.GetXiGeshan() && WaterData.Instance.GetAAO()
                && WaterData.Instance.GetErChenChi()&&Q < Eff)
            {
                WaterData.Instance.SetProgress(70.0f);
                WaterData.Instance.SetGaoMiDu(true);
            }
            else
            {
                WaterData.Instance.SetProgress(60.0f);
                WaterData.Instance.SetGaoMiDu(false);
            }
        }


        string Qstr = $"进水流量: {(Q > Eff ? $"<color=#ff0000>{Q:F2} m³/d </color>" : $"{Q:F2} m³/d")}\n\n" +
                        $"设备处理能力: {(Q > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";

        Rserr = Rs < 60;
        Eerr = E > 0.2;
        Rtperr = Rtp < 70;

        if (Rserr && (!hover.errMsg.Contains("SS去除率较低")))
        {
            hover.errMsg.Add("SS去除率较低");
        }
        else if (!Rserr)
        {
            hover.errMsg.Remove("SS去除率较低");
        }

        if (Rtperr && (!hover.errMsg.Contains("TP去除率较低")))
        {
            hover.errMsg.Add("TP去除率较低");
        }
        else if (!Rtperr)
        {
            hover.errMsg.Remove("TP去除率较低");
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

    public float getQ()
    {
        return Q;
    }
    void CalculateAAOParameters()
    {

        //SS去除率
        Rs = 85f * Mathf.Pow(HRT / 1.5f, 0.25f) * Mathf.Pow(C / 20f, 0.3f) * Mathf.Pow(4f / q, 0.6f) * Mathf.Pow(G / 500f, 0.05f);
        Rs = Mathf.Clamp(Rs, 0f, 92f);
        //TP去除率
        Rtp = 85f * Mathf.Pow(HRT / 1.5f, 0.15f) * Mathf.Pow(C / 20f, 0.5f) * Mathf.Pow(4f / q, 0.4f) * Mathf.Pow(G / 500f, 0.02f);
        Rtp = Mathf.Clamp(Rtp, 0f, 90f);
        //出水SS
        SSOut = SSIn - (SSIn * Rs * 0.01f);
        WaterData.Instance.SetSSOUT(SSOut);
        WaterData.Instance.SetSSSave(1, SSOut);

        //出水TP
        TPOut = TPIn - (TPIn * Rtp * 0.01f);
        WaterData.Instance.SetTPOUT(TPOut);
        WaterData.Instance.SetTPSave(1, TPOut);

        E = 0.000005f * G * G;

        Ccost = C * 2000f / 1000000f;


        Eff = 2010*q * Mathf.Pow(G / 200f, 0.5f) * Mathf.Pow(C / 10, 0.3f) * (1.5f / HRT);
        Cost = E * 0.6f * Q+Ccost*Q;

        ita = 0.003f*q*Mathf.Sqrt(G)/HRT;

        Qout = Q * (1f - ita/100f);
        WaterData.Instance.SetQ(Qout);
        WaterData.Instance.setCost(Cost, 4);

    }

}
