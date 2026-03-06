
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class erChenChi : MonoBehaviour
{
    [Header("输入参数")]
    public float Q;       // 进水流量 (m³/d)
    public float SSIn;        // 进水SS (mg/L)

    public Text inputParametersText;  // 输入参数文本
    public Text outputParametersText; // 输出参数文本
    public Text adjustableParametersText; // 可调参数文本
    public Text machineMsg; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    [Header("可调参数")]
    private float R = 80f;         // 污泥回流比 (%)
    private float Qw = 200f;         // 排泥量 (m³/d)
    private float HRT = 2f;         // 水力停留时间 (h)
    private float q = 1.0f;         //表面负荷，单位时间内单位表面积处理的污水量（m³/m²·h）


    [Header("输出参数")]
    public float Qr;        //回流污泥流量（m³/d）
    public float Vs;        //污泥沉降速度（m/h）
    public float MLSS;        //混合液悬浮固体（mg/L）
    public float SRT;        //污泥龄（d）
    public float H;        //有效水深（m）
    public float E;        //
    public float Eff;        //
    public float Qout;        //
    public float Cost;        //
    public float ita;        //


    public float SSOUT;     // 出水SS (mg/L)
    public float SSRatio;

    public bool Vserr = false;
    public bool Herr = false;
    public bool MLSSerr = false;
    public bool SRTerr = false;
    public bool Eerr = false;
    public bool Rserr = false;

    public Vector3 pos;

    public ButtonHoverHandler hover;

    [Header("UI控件")]

    public Slider RSlider;    // 污泥回流比调节
    public Slider QwSlider;    // 排泥量调节
    public Slider HRTSlider;    // 水力停留时间调节
    public Slider qSlider;    // 表面负荷调节
    public Slider QSlider;    // 进水流量调节

    public float GetR()
    {
        return R;
    }
    public float GetQw()
    {
        return Qw;
    }

    public float GetHRT()
    {
        return HRT;
    }

    public float Getq()
    {
        return q;
    }

    void OnEnable()
    {
        SSIn = WaterData.Instance.GetSSIn();
        // 初始化滑动条
        RSlider.minValue = 50f;
        RSlider.maxValue = 100f;
        RSlider.value = R;

        QwSlider.minValue = 100f;
        QwSlider.maxValue = 300f;
        QwSlider.value = Qw;

        HRTSlider.minValue = 1.5f;
        HRTSlider.maxValue = 4f;
        HRTSlider.value = HRT;

        qSlider.minValue = 0.6f;
        qSlider.maxValue = 1.5f;
        qSlider.value = q;

        QSlider.maxValue = WaterData.Instance.GetQSave(2);
        QSlider.minValue = QSlider.maxValue>10000f ? 10000f : QSlider.maxValue-1000f;
        Q = (QSlider.maxValue + QSlider.minValue) / 2;
        QSlider.value = Q;

        pos = this.transform.localPosition;

        // 添加监听事件
        RSlider.onValueChanged.AddListener(UpdateR);
        QwSlider.onValueChanged.AddListener(UpdateQw);
        HRTSlider.onValueChanged.AddListener(UpdateHRT);
        qSlider.onValueChanged.AddListener(Updateq);
        QSlider.onValueChanged.AddListener(UpdateQ);

        CalculateAAOParameters();
        UpdateParameterDisplay();
    }


    void UpdateParameterDisplay()
    {
        // 输入参数显示
        string inputStr = 
                        $"进水COD: {WaterData.Instance.GetCODOUT():F2} mg/L\n\n" +
                        $"进水BOD5: {WaterData.Instance.GetBOD5OUT():F2} mg/L\n\n" +
                        $"进水TN: {WaterData.Instance.GetTNOUT():F2} mg/L\n\n" +
                        $"进水TP: {WaterData.Instance.GetTPOUT():F2} mg/L\n\n" +
                        $"进水SS: {WaterData.Instance.GetSSIn():F2} mg/L\n\n" ;

        // 可调参数显示
        string adjustableStr = 
                            $"污泥回流比: {R:F1} %\n\n" +
                            $"排泥量: {Qw:F1} m³/d\n\n" +
                            $"水力停留时间: {HRT:F1} h\n\n" +
                            $"表面负荷: {q:F1} m³/m²·h" ;

        //输出水源有关
        string outputStr =
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0} mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0} mg/L\n\n" +
                        $"出水TN: {WaterData.Instance.GetTNOUT():F1} mg/L\n\n" +
                        $"出水TP: {WaterData.Instance.GetTPOUT():F1} mg/L\n\n" +
                        $"出水SS: {( SSRatio< 80 ? $"<color=#ff0000>{WaterData.Instance.GetSSSave(0):F2} mg/L (出水SS较高)</color>" : $"{WaterData.Instance.GetSSOUT():F2} mg/L ")}";

        // 输出参数显示
        string machineStr = 
                       
                        $"污泥沉降速度: {(Vs > 0.7 ? $"<color=#ff0000>{Vs:F2} m/h (污泥沉降速度异常)</color>" : $"{Vs:F2} m/h")} \n\n" +
                        $"混合液悬浮固体: {(MLSS > 5000 || MLSS < 3000 ? $"<color=#ff0000>{MLSS:F2} mg/L (混合液悬浮固体异常)</color>" : $"{MLSS:F2} mg/L ")} \n\n" +
                        $"有效水深: {(H > 5 || H < 3 ? $"<color=#ff0000>{H:F2} m (有效水深异常)</color>" : $"{H:F2} m")} \n\n" +
                        $"能耗: {(E > 0.08 ? $"<color=#cccc00>{E:F3} kwh/m³ (设备能耗较高)</color>" : $"{E:F3} kwh/m³ ")}\n\n" +
                        $"污泥龄: {(SRT > 20 ||SRT<10 ? $"<color=#ff0000>{SRT:F2} d (污泥龄异常)</color>" : $"{SRT:F2} d")} ";


        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (Vs <= 0.7 && !(MLSS > 5000 || MLSS < 3000) && !(H > 5 || H < 3) && E <= 0.08 && !(SRT > 20 || SRT < 10)&& SSRatio > 80
                && WaterData.Instance.GetCuGeshan() && WaterData.Instance.GetXiGeshan() && WaterData.Instance.GetAAO()&&Q<Eff)
            {
                WaterData.Instance.SetProgress(60.0f);
                WaterData.Instance.SetErChenChi(true);
            }
            else
            {
                WaterData.Instance.SetProgress(40.0f);
                WaterData.Instance.SetErChenChi(false);
            }
        }





        string Qstr= $"进水流量: {(Q > Eff ? $"<color=#ff0000>{Q:F2} m³/d </color>" : $"{Q:F2} m³/d")}\n\n" +
                        $"设备处理能力:  {(Q > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";

        Vserr = Vs > 0.7;
        MLSSerr = MLSS > 5000 || MLSS < 3000;
        Herr = H > 5 || H < 3;
        Eerr = E > 0.08;
        SRTerr = SRT > 20 || SRT < 10;
        Rserr = SSRatio < 80;

        if (Vserr&& (!hover.errMsg.Contains("污泥沉降速度异常")))
        {
            hover.errMsg.Add("污泥沉降速度异常");
        }
        else if(!Vserr)
        {
            hover.errMsg.Remove("污泥沉降速度异常");
        }
        if (MLSSerr&& (!hover.errMsg.Contains("混合液悬浮固体异常")))
        {
            hover.errMsg.Add("混合液悬浮固体异常");
        }
        else if(!MLSSerr)
        {
            hover.errMsg.Remove("混合液悬浮固体异常");
        }
        if (Herr&& (!hover.errMsg.Contains("有效水深异常")))
        {
            hover.errMsg.Add("有效水深异常");
        }
        else if (!Herr)
        {
            hover.errMsg.Remove("有效水深异常");
        }

        if (SRTerr&& (!hover.errMsg.Contains("污泥龄异常")))
        {
            hover.errMsg.Add("污泥龄异常");
        }
        else if (!SRTerr)
        {
            hover.errMsg.Remove("污泥龄异常");
        }

        if (Rserr && (!hover.errMsg.Contains("SS去除率较低")))
        {
            hover.errMsg.Add("SS去除率较低");
        }
        else if (!Rserr)
        {
            hover.errMsg.Remove("SS去除率较低");
        }

        if (Eerr&& (!hover.errMsg.Contains("设备能耗较高")))
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


    void UpdateR(float newValue)
    {
        R = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateHRT(float newValue)
    {
        HRT = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateQw(float newValue)
    {
        Qw = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void Updateq(float newValue)
    {
        q = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateQ(float newValue)
    {
        Q = newValue;
        WaterData.Instance.SetQSave(3, Q);
        Debug.Log("QSaved:" + WaterData.Instance.GetQSave(3));
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    public float getQ()
    {
        return Q;
    }
    void CalculateAAOParameters()
    {
        //------------------------- 回流污泥流量 -------------------------
        Qr = Q * R / 100f;

        //有效水深
        H = HRT * q;

        //Vs计算
        Vs = q*R/100f;

        //MLSS计算
        float temp = (Q + Qr) / (Qr + Qw);
        float V = Q *(1+R/100f)* HRT / 24f;
        SRT=4f*(V*(Qr + Qw))/(Qw*(Q+Qr));
        MLSS = (SRT * Qw * (1+R/100f) * 1000) / (V*R/100f)*1.12f;


        //SS变化公式
       
        float tempup = q * (1 + R/100f) * 0.001f;
        float tempdown = HRT * Qw / (Q * 6);
        SSOUT = 0.93f*SSIn * tempup / tempdown;
        SSRatio = (SSIn-SSOUT) * 100 / SSIn;
        SSRatio= Mathf.Clamp(SSRatio, 0f, 95f);
        SSOUT = SSIn - SSIn * SSRatio/100f;
        WaterData.Instance.SetSSSave(0, SSOUT);
        WaterData.Instance.SetSSOUT(SSOUT);

        E = 0.9f * Mathf.Pow(R / 100f, 0.8f) * (Qw / 1000f) * Mathf.Pow(HRT / 24f, 0.5f) * Mathf.Pow(q / 1.0f, -0.6f);
        Eff = 2010 * q * (1 + R / 100f) * Mathf.Pow(Qw, 0.5f) / HRT;
        Cost = E * 0.6f * Q;


        ita = Qw * (1f + R / 100f) / (30f * q * HRT);

        Qout = Q * (1f - ita/100f);
        WaterData.Instance.SetQ(Qout);
        WaterData.Instance.setCost(Cost, 3);

        WaterData.Instance.SetQwuni(Qw);

        Debug.Log($"回流污泥流量: {Qr:F1}m³/d | " +
                 $"污泥沉降速度: {Vs:F1}m/h | " +
                 $"有效水深: {H:F1}m" +
                 $"MLSS: {MLSS:F1}mg/L | " +
                 $"出水SS: {SSOUT:F1}mg/L" +
                 $"SS去除率: {SSRatio:F1}%" +
                 $"污泥龄: {SRT:F1}d | " );
    }
}
