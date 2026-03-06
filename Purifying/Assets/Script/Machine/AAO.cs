using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AAO : MonoBehaviour
{
    [Header("输入参数")]
    public float Q;       // 进水流量max
    public float CODIn;     // 进水COD (mg/L)
    public float BOD5In;    // 进水BOD5 (mg/L)
    public float TNIn;       // 进水总氮 (mg/L)
    public float TPIn;        // 进水总磷 (mg/L)
    private float temperature = 20f;// 水温 (°C)

    [Header("可调参数")]
    private float r = 100f;        // 内回流比 (%)
    private float MLSS = 3000f;     // 污泥浓度 (mg/L)
    private float HRT_Oxic = 4f;    // 好氧池HRT (h)
    private float DO = 2.5f;        // 好氧池DO (mg/L)
    private float SRT = 15f;        // 污泥龄 (d)

    [Header("输出参数")]
    public float CODOUT;    // 出水COD (mg/L)
    public float BOD5OUT;   // 出水BOD5 (mg/L)
    public float TNOUT;     // 出水总氮 (mg/L)
    public float TPOUT;     // 出水总磷 (mg/L)
    public float eta_N;     // 脱氮效率 (%)
    public float eta_P;     // 除磷效率 (%)
    public float energy;    // 曝气能耗 (kWh/m³)
    public float Eff; //处理能力(m³/d)
    public float Cost; //处理能力(m³/d)

    public float Rtn;
    public float Rbod;
    public float Rcod;
    public float Rtp;

    [Header("UI控件")]
    public Slider rSlider;    // 内回流比调节
    public Slider HRTSlider;    // 好氧池HRT调节
    public Slider DOSlider;    // 好氧池DO调节
    public Slider SRTSlider;    // 污泥龄调节
    public Slider QSlider;    // 进水流量调节

    [Header("输出文本")]
    public Text inputParametersText;  // 输入参数文本
    public Text outputMachineParametersText; // 输出设备有关
    public Text outputWaterParametersText; // 输出水源有关
    public Text adjustableParametersText; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    public bool tperr=false;
    public bool tnerr=false;
    public bool Eerr=false;
    public bool Qerr=false;

    public ButtonHoverHandler hover;

    public Vector3 pos;

    public float Getr()
    { 
        return r; 
    }
    public float GetMLSS()
    {
        return MLSS;
    }
    public float GetHRT()
    {
        return HRT_Oxic;
    }
    public float GetDO()
    {
        return DO;
    }
    public float GetSRT()
    {
        return SRT;
    }

    void OnEnable()
    {
        CODIn = WaterData.Instance.GetCODIn();
        BOD5In = WaterData.Instance.GetBOD5In();
        TNIn = WaterData.Instance.GetTNIn();
        TPIn = WaterData.Instance.GetTPIn();

        // 初始化滑动条
        rSlider.minValue = 100f;
        rSlider.maxValue = 400f;
        rSlider.value = r;

        HRTSlider.minValue = 4f;
        HRTSlider.maxValue = 8f;
        HRTSlider.value = HRT_Oxic;

        DOSlider.minValue = 1f;
        DOSlider.maxValue = 5f;
        DOSlider.value = DO;

        SRTSlider.minValue = 5f;
        SRTSlider.maxValue = 25f;
        SRTSlider.value = SRT;

        QSlider.maxValue = WaterData.Instance.GetQSave(1);
        QSlider.minValue = 10000f;
        Q = (QSlider.maxValue + QSlider.minValue) / 2;
        QSlider.value = Q;

        pos = this.transform.localPosition;

        // 添加监听事件
        rSlider.onValueChanged.AddListener(Updater);
        HRTSlider.onValueChanged.AddListener(UpdateHRT);
        DOSlider.onValueChanged.AddListener(UpdateDO);
        SRTSlider.onValueChanged.AddListener(UpdateSRT);
        QSlider.onValueChanged.AddListener(UpdateQ);

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
                        $"进水总磷: {WaterData.Instance.GetTPIn()} mg/L";

        // 可调参数显示
        string adjustableStr = 
                            $"内回流比调节: {r:F2} m\n\n" +
                            $"好氧池HRT调节: {HRT_Oxic:F2} m\n\n" +
                            $"好氧池DO调节: {DO:F2} m\n\n" +
                            $"污泥龄调节: {SRT:F1} d";

        // 输出设备有关
        string outputMachineStr = 
                        //$"脱氮效率: {eta_N:F2} %\n\n" +
                        $"脱氮率: {(eta_N < 60 ? $"<color=#ff0000>{eta_N:F2} % (脱氮率过低)</color>" : $"{eta_N:F2} %")}\n\n" +
                        //$"脱磷效率: {eta_P:F2} %\n\n" +
                        $"脱磷率: {(eta_P < 75 ? $"<color=#ff0000>{eta_P:F2} % (脱磷率过低)</color>" : $"{eta_P:F2} %")}\n\n" +
                        $"能耗: {(energy > 0.7f ? $"<color=#ff0000>{energy:F2} kWh/m³ (设备能耗较高)</color>" : $"{energy:F2} kWh/m³")}";
        //$"曝气能耗: {energy:F2} kWh/d";

        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (eta_N >= 60 && eta_P >= 75 && energy <= 0.7f && WaterData.Instance.GetCuGeshan() && WaterData.Instance.GetXiGeshan()&&Q<Eff)
            {
                WaterData.Instance.SetProgress(40.0f);
                WaterData.Instance.SetAAO(true);
            }
            else
            {
                WaterData.Instance.SetProgress(20.0f);
                WaterData.Instance.SetAAO(false);
            }
        }

        //输出水源有关
        string outputWaterStr =
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0}mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0}mg/L\n\n" +
                        $"出水TN: {WaterData.Instance.GetTNSave(0):F1}mg/L\n\n" +
                        $"出水TP: {WaterData.Instance.GetTPSave(0):F1}mg/L";

        string Qstr = $"进水流量: {(Q > Eff ? $"<color=#ff0000>{Q:F2} m³/d </color>" : $"{Q:F2} m³/d")}\n\n" +
                        $"设备处理能力: {(Q > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";


        tperr = eta_P < 75;
        tnerr = eta_N < 60;
        Eerr = energy > 0.7f;
        Qerr = Q > Eff;

        if (tperr && (!hover.errMsg.Contains("脱磷率过低")))
        {
            hover.errMsg.Add("脱磷率过低");
        }
        else if (!tperr)
        {
            hover.errMsg.Remove("脱磷率过低");
        }

        if (tnerr && (!hover.errMsg.Contains("脱氮率过低")))
        {
            hover.errMsg.Add("脱氮率过低");
        }
        else if (!tnerr)
        {
            hover.errMsg.Remove("脱氮率过低");
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
        outputMachineParametersText.text = outputMachineStr;
        outputWaterParametersText.text = outputWaterStr;
        QParametersText.text = Qstr;
    }

    void Updater(float newValue)
    {
        r = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateHRT(float newValue)
    {
        HRT_Oxic = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateDO(float newValue)
    {
        DO = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateSRT(float newValue)
    {
        SRT = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void UpdateQ(float newValue)
    {
        Q = newValue;
        WaterData.Instance.SetQSave(2, Q);
        Debug.Log("QSaved:" + WaterData.Instance.GetQSave(2));
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    public float getQ()
    {
        return Q;
    }
    void CalculateAAOParameters()
    {

        // ------------------------- 有机物去除 -------------------------
        // BOD5去除率公式：η_BOD = (BOD5In - BOD5OUT)/BOD5In
        float k_BOD = 0.3f * Mathf.Pow(1.05f, temperature - 20f);
        BOD5OUT = BOD5In * Mathf.Exp(-k_BOD * HRT_Oxic);
        float BOD5Removed = BOD5In - BOD5OUT;

        Rbod = 100f*(BOD5In - BOD5OUT) / BOD5In;

        // COD去除率与BOD5去除率相关（经验公式）
        float COD_RemovalRatio = 0.15f + 0.25f * (BOD5Removed / BOD5In);  // COD去除率15%~40%
        CODOUT = CODIn * (1 - COD_RemovalRatio);

        Rcod = 100f*(CODIn - CODOUT) / CODIn;

        // ------------------------- 脱氮计算 -------------------------
        // 理论脱氮效率公式：η_N = r/(1+r) * 100%
        float theoreticalEtaN = (r / 100f) / (1 + r / 100f) * 100f;

        // 实际脱氮效率（受温度修正）：η_N = η_max * θ^(T-20)
        float theta = 1.08f; // 温度修正系数
        eta_N = Mathf.Min(
        theoreticalEtaN
        * Mathf.Pow(theta, temperature - 20f)  // 温度修正项
        * ((1.0f + 0.1f * Mathf.Max(DO - 2, 0f)))  // DO抑制项
        *Mathf.Exp(-0.001f * Mathf.Pow(SRT - 5f, 2f)),
        95f
        );
        //eta_N = theoreticalEtaN;

        // 出水TN公式：TNOUT = TNIn * (1 - eta_N/100)
        TNOUT = TNIn * (1 - eta_N / 100f);

        Rtn= 100f * (TNIn - TNOUT) / TNIn;

        // ------------------------- 除磷计算 -------------------------
        // 生物除磷效率公式：η_P = (0.03~0.06)*BOD5去除量 - 排泥除磷

        // 生物除磷效率公式（动态系数）
        float k_P = (temperature > 20) ? 0.8f : 0.6f;
        float sludgeP_Contribution = (MLSS * 0.07f) / SRT; // 污泥含磷量按7%计
        eta_P = Mathf.Clamp(k_P * BOD5Removed + sludgeP_Contribution, 50, 97);

        // 出水TP公式：TPOUT = TPIn * (1 - eta_P/100)
        TPOUT = TPIn * (1 - eta_P / 100f);

        Rtp = 100f*(TPIn - TPOUT) / TPIn;

        // ------------------------- 能耗计算 -------------------------
        // 曝气能耗公式：E = Q*DO*K / 10000 （K=1.5kWh/m³空气）
        float K = 1.5f;
        energy = (0.01f * DO * HRT_Oxic) +        // 曝气能耗项
                 (0.0005f * r) +                    // 内回流泵能耗项（优化后公式）
                 (5f / SRT);                   // 污泥处理能耗项

        Eff = 10f * MLSS * BOD5In / (HRT_Oxic * SRT);

        Cost = energy * 0.6f * Q;

        WaterData.Instance.SetQ(Q);
        WaterData.Instance.setCost(Cost, 2);

        WaterData.Instance.SetCODOUT(CODOUT);
        WaterData.Instance.SetBOD5OUT(BOD5OUT);
        WaterData.Instance.SetTNOUT(TNOUT);
        WaterData.Instance.SetTNSave(0, TNOUT);
        WaterData.Instance.SetTPOUT(TPOUT);
        WaterData.Instance.SetTPSave(0, TPOUT);

        Debug.Log($"脱氮效率: {eta_N:F1}% | " +
                 $"除磷效率: {eta_P:F1}% | " +
                 $"能耗: {energy:F0}kWh/m³" +
                 $"出水COD: {CODOUT:F0}mg/L | " +
                 $"出水CODinstance: {WaterData.Instance.GetCODOUT():F0}mg/L | " +
                 $"出水BOD5: {BOD5OUT:F0}mg/L" +
                 $"出水TN: {TNOUT:F1}mg/L | " +
                 $"出水TP: {TPOUT:F1}mg/L");

        
    }
}
