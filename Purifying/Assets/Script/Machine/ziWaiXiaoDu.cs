using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class ziWaiXiaoDu : MonoBehaviour
{

    [Header("输入参数")]
    public float M0 = 2000f;       // 粪大肠菌群数 (CFU/L)
    public float Q;       // 粪大肠菌群数 (CFU/L)

    public Text inputParametersText;  // 输入参数文本
    public Text outputParametersText; // 输出参数文本
    public Text adjustableParametersText; // 可调参数文本
    public Text machineMsg; // 可调参数文本
    public Text QParametersText; // 可调参数文本

    [Header("可调参数")]
    private float I = 40f;         //  紫外线强度(mW/cm²)
    private float t = 0.004f;         //  接触时间(h)


    [Header("输出参数")]
    public float M;       // 粪大肠菌群数 (CFU/L)
    public float E;       // 设备能耗 (kwh/m³)
    public float Eff;       // 设备能耗 (kwh/m³)
    public float Cost; //处理能力(m³/d)
    public float Rm; //处理能力(m³/d)


    [Header("UI控件")]
    public Slider ISlider;    // 设备压力调节
    public Slider tSlider;    // 脱水时间调节
    public Slider QSlider;    // 进水流量调节

    public bool Merr=false;
    public bool Eerr=false;

    public Vector3 pos;

    public ButtonHoverHandler hover;

    public float GetI() { return I; }
    public float Gett() { return t; }

    void OnEnable()
    {
        M0 = WaterData.Instance.GetGermIn();
        // 初始化滑动条
        ISlider.minValue = 30f;
        ISlider.maxValue = 60f;
        ISlider.value = I;

        tSlider.minValue = 0.003f;
        tSlider.maxValue = 0.008f;
        tSlider.value = t;

        QSlider.maxValue = WaterData.Instance.GetQSave(5);
        QSlider.minValue = QSlider.maxValue > 10000f ? 10000f : QSlider.maxValue - 1000f;
        Q = (QSlider.maxValue + QSlider.minValue) / 2;
        QSlider.value = Q;

        pos = this.transform.localPosition;

        // 添加监听事件
        ISlider.onValueChanged.AddListener(UpdateI);
        tSlider.onValueChanged.AddListener(Updatet);
        QSlider.onValueChanged.AddListener(UpdateQ);


        CalculateAAOParameters();

        UpdateParameterDisplay();
    }


    void UpdateI(float newValue)
    {
        I = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }
    void Updatet(float newValue)
    {
        t = newValue;
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    void UpdateQ(float newValue)
    {
        Q = newValue;
        WaterData.Instance.SetQSave(6, Q);
        Debug.Log("QSaved:" + WaterData.Instance.GetQSave(6));
        CalculateAAOParameters();
        UpdateParameterDisplay();
    }

    void UpdateParameterDisplay()
    {
        // 输入参数显示
        string inputStr =
                        $"粪大肠菌群数: {M0:F2} CFU/L\n\n" ;

        // 可调参数显示
        string adjustableStr =
                            $"紫外线强度: {I:F2} mW/cm²\n\n" +
                            $"接触时间: {t:F3} h\n\n";

        //输出水源有关
        string outputStr =
                        $"出水COD: {WaterData.Instance.GetCODOUT():F0}mg/L\n\n" +
                        $"出水BOD5: {WaterData.Instance.GetBOD5OUT():F0}mg/L\n\n" +
                        $"出水TN: {WaterData.Instance.GetTNOUT():F1}mg/L\n\n" +
                        $"出水TP: {WaterData.Instance.GetTPOUT():F1}mg/L\n\n" +
                        $"出水SS: {WaterData.Instance.GetSSOUT():F3} mg/L";

        // 输出参数显示
        string machineStr =
                        $"出水粪大肠菌群数: {(M > 1000 ? $"<color=#ff0000>{M:F2} CFU/L (粪大肠菌群数较高)</color>" : $"{M:F2} CFU/L ")} \n\n" +
                        $"设备能耗: {(E > 0.08 ? $"<color=#cccc00>{E:F3} kwh/m³ (设备能耗较高)</color>" : $"{E:F3} kwh/m³ ")} ";

        if (WaterData.Instance.GetisTrip())
        {
            WaterData.Instance.SetProgress(0.0f);
        }
        else
        {
            if (M <= 1000 && E <= 0.08 && WaterData.Instance.GetCuGeshan()
                && WaterData.Instance.GetXiGeshan() && WaterData.Instance.GetAAO()
                && WaterData.Instance.GetErChenChi() && WaterData.Instance.GetShenChuangLvChi()
                && WaterData.Instance.GetgaoMiDu()&&Q<Eff)
            {
                WaterData.Instance.SetProgress(90.0f);
                WaterData.Instance.SetZiWaiXiaoDu(true);
            }
            else
            {
                WaterData.Instance.SetProgress(80.0f);
                WaterData.Instance.SetZiWaiXiaoDu(false);
            }
        }




        string Qstr = $"进水流量: {(Q > Eff ? $"<color=#ff0000>{Q:F2} m³/d </color>" : $"{Q:F2} m³/d")}\n\n" +
                        $"设备处理能力:  {(Q > Eff ? $"<color=#ff0000>{Eff:F2} m³/d </color>" : $"{Eff:F2} m³/d")}\n\n" +
                        $"处理成本: {Cost:F2} 元/d";

        Merr = M > 1000 ;
        Eerr = E > 0.08;

        if (Merr && (!hover.errMsg.Contains("粪大肠菌群数较高")))
        {
            hover.errMsg.Add("粪大肠菌群数较高");
        }
        else if (!Merr)
        {
            hover.errMsg.Remove("粪大肠菌群数较高");
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

        //设备能耗
        E = 1.6f*I*2500f*t/20000f;

        //粪大肠菌群数
        M = 0.96f*M0 * Mathf.Exp(-0.23f * t*I*10);
        WaterData.Instance.SetGermOUT(M);

        Eff = 35 * I * Mathf.Pow(t, -0.5f);
        Cost = E * 0.6f * Q;

        Rm = 100f*(M0 - M) / M0;

        WaterData.Instance.SetQ(Q);
        WaterData.Instance.setCost(Cost, 6);

    }

}
