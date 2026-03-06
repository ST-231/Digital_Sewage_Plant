using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using static UnityEngine.EventSystems.EventTrigger;

public class shenChuangLvChi : MonoBehaviour
{
    [Header("输入参数")]
    public float SSIn;        // 进水SS (mg/L)
    public float Q;        // 进水SS (mg/L)
    public float Qout;        // 进水SS (mg/L)

    public Text inputParametersText;  // 输入参数文本
    public Text outputParametersText; // 输出参数文本
    public Text adjustableParametersText; // 可调参数文本
    public Text machineMsg; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    [Header("可调参数")]
    private float H = 2f;       // 滤料层厚度 (m)
    private float v = 6f;       // 过滤速度 (m/h)
    private float qb = 10f;       // 反冲洗强度 (L/(m²·s))
    private float d = 2f;       // 滤料粒径 (mm)
    private float T = 24f;       // 反冲洗周期 (h)

    [Header("输出参数")]
    public float Rs;        //SS去除率（%）
    public float SSOut;        // 出水SS (mg/L)
    public float E;       // 设备能耗 (kwh/m³)
    public float Eff; //处理能力(m³/d)
    public float Cost; //处理能力(m³/d)
    public float ita; //处理能力(m³/d)

    [Header("UI控件")]
    public Slider HSlider;    // 设备压力调节
    public Slider vSlider;    // 脱水时间调节
    public Slider qbSlider;    // 脱水时间调节
    public Slider dSlider;    // 脱水时间调节
    public Slider TSlider;    // 脱水时间调节
    public Slider QSlider;    // 进水流量调节

    public bool Rserr=false;
    public bool Eerr=false;

    public Vector3 pos;

    public ButtonHoverHandler hover;

    public float GetH()
    {
        return H;
    }

    public float Getv()
    {
        return v;
    }

    public float Getqb()
    {
        return qb;
    }

    public float Getd()
    {
        return d;
    }

    public float GetT()
    {
        return T;
    }

    void OnEnable()
    {
        SSIn = WaterData.Instance.GetSSSave(1);
        // 初始化滑动条
        HSlider.minValue = 1f;
        HSlider.maxValue = 3f;
        HSlider.value = H;

        vSlider.minValue = 4f;
        vSlider.maxValue = 8f;
        vSlider.value = v;

        qbSlider.minValue = 6f;
        qbSlider.maxValue = 20f;
        qbSlider.value = qb;

        dSlider.minValue = 0.8f;
        dSlider.maxValue = 2.0f;
        dSlider.value = d;

        TSlider.minValue = 24f;
        TSlider.maxValue = 48f;
        TSlider.value = T;

        QSlider.maxValue = WaterData.Instance.GetQSave(4);
        QSlider.minValue = QSlider.maxValue > 10000f ? 10000f : QSlider.maxValue - 1000f;
        Q = (QSlider.maxValue + QSlider.minValue) / 2;
        QSlider.value = Q;

        pos = this.transform.localPosition;


        // 添加监听事件
        HSlider.onValueChanged.AddListener(UpdateH);
        vSlider.onValueChanged.AddListener(Updatev);
        qbSlider.onValueChanged.AddListener(Updateqb);
        dSlider.onValueChanged.AddListener(Updated);
        TSlider.onValueChanged.AddListener(UpdateT);
        QSlider.onValueChanged.AddListener(UpdateQ);


        CalculateAAOParameters();

        UpdateParameterDisplay();
    }


    void UpdateH(float newValue)
    {
        H = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void Updatev(float newValue)
    {
        v = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void Updateqb(float newValue)
    {
        qb = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void Updated(float newValue)
    {
        d = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    void UpdateT(float newValue)
    {
        T = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    void UpdateQ(float newValue)
    {
        Q = newValue;
        WaterData.Instance.SetQSave(5, Q);
        Debug.Log("QSaved:" + WaterData.Instance.GetQSave(5));
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
                            $"滤料层厚度调节: {H:F1} m\n\n" +
                            $"过滤速度调节: {v:F1} m/h\n\n" +
                            $"反冲洗强度调节: {qb:F1} L/(m²·s)\n\n" +
                            $"滤料粒径: {d:F1} mm\n\n" +
                            $"反冲洗周期: {T:F1} h\n\n";


        //输出水源有关
        string outputStr =
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0} mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0} mg/L\n\n" +
                        $"出水TN: {WaterData.Instance.GetTNOUT():F1} mg/L\n\n" +
                        $"出水TP: {WaterData.Instance.GetTPOUT():F1} mg/L\n\n" +
                        $"出水SS: {WaterData.Instance.GetSSSave(2):F3} mg/L";
                        //$"出水SS: {WaterData.Instance.GetSSOUT():F3} mg/L";

        // 输出参数显示
        string machineStr =
        $"SS去除率: {(Rs < 80 ? $"<color=#ff0000>{Rs:F2} % (SS去除率较低)</color>" : $"{Rs:F2} % ")} \n\n" +
                        $"设备能耗: {(E > 0.15 ? $"<color=#cccc00>{E:F2} kwh/m³ (设备能耗较高)</color>" : $"{E:F2} kwh/m³ ")}\n\n";

        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (Rs >= 80 && E <= 0.15 && WaterData.Instance.GetCuGeshan()
                && WaterData.Instance.GetXiGeshan() && WaterData.Instance.GetAAO()
                && WaterData.Instance.GetErChenChi() && WaterData.Instance.GetgaoMiDu()&&Q < Eff)
            {
                WaterData.Instance.SetProgress(80.0f);
                WaterData.Instance.SetShenChuangLvChi(true);
            }
            else
            {
                WaterData.Instance.SetProgress(70.0f);
                WaterData.Instance.SetShenChuangLvChi(false);
            }
        }


        string Qstr = $"进水流量: {(Q > Eff ? $"<color=#ff0000>{Q:F2} m³/d </color>" : $"{Q:F2} m³/d")}\n\n" +
                        $"设备处理能力:  {(Q > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";


        Rserr = Rs < 80;
        Eerr = E > 0.15;

        if (Rserr && (!hover.errMsg.Contains("SS去除率较低")))
        {
            hover.errMsg.Add("SS去除率较低");
        }
        else if (!Rserr)
        {
            hover.errMsg.Remove("SS去除率较低");
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
        Rs = 90f * Mathf.Pow(H / 2f, 0.5f) * Mathf.Pow(6f / v, 0.3f) * Mathf.Pow(qb / 10f, 0.2f) * Mathf.Pow(1f / d, 0.4f);
        Rs = Mathf.Clamp(Rs, 0f, 95f);
        //出水SS
        SSOut =SSIn- (SSIn * Rs * 0.01f);
        WaterData.Instance.SetSSOUT(SSOut);
        WaterData.Instance.SetSSSave(2, SSOut);

        //能耗
        E = 4.54f * qb / T*0.05f;

        Eff = 30f*720f * v * H * Mathf.Pow(qb, 0.5f)/ (d*T);
        Cost = E * 0.6f * Q;

        ita = 0.05f * qb *v/(H*d);

        Qout = Q * (1f - ita/100f);
        WaterData.Instance.SetQ(Qout);
        WaterData.Instance.setCost(Cost, 5);
    }
}
