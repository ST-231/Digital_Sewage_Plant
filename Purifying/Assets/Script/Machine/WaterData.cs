using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterData : MonoBehaviour
{
    // 单例模式实现
    private static WaterData _instance;
    public static WaterData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WaterData>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameDataManager");
                    _instance = go.AddComponent<WaterData>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    public int sceneIndex;

    // 公共变量定义
    [Header("水属性输入")]
    private float Q;       // 进水流量max
    private float[] Qsave = new float[8];
    private float[] TNsave = new float[2];//AAO
    private float[] TPsave = new float[2];////AAO,高密度沉淀
    private float[] SSsave = new float[3];//二沉池，高密度沉淀，深床滤池
    private float Qmin;       // 进水流量min
    private float CODIn;     // 进水COD (mg/L)
    private float BOD5In;    // 进水BOD5 (mg/L)
    private float TNIn;       // 进水总氮 (mg/L)
    private float TPIn;        // 进水总磷 (mg/L)
    private float SSIn;        // 进水SS (mg/L)
    private float GermIn = 2000f;        // 进水SS (mg/L)
    private float Qwuni;        // 进水SS (mg/L)

    private float CODTemp;
    private float BOD5Temp;
    private float TNTemp;
    private float TPTemp;
    private float SSTemp;

    [Header("水属性输出")]
    private float CODOUT;    // 出水COD (mg/L)
    private float BOD5OUT;   // 出水BOD5 (mg/L)
    private float TNOUT;     // 出水总氮 (mg/L)
    private float TPOUT;     // 出水总磷 (mg/L)
    private float SSOUT;        // 进水SS (mg/L)
    private float GermOUT = 2000f;        // 进水SS (mg/L)
    private void Start()
    {
        sceneIndex = LoadingScreenManager.nextSceneIndex;
        Debug.Log("index" + sceneIndex);

        if (sceneIndex == 0)
        {
            Q = 20000f;       // 进水流量max
            Qmin = 10000f;       // 进水流量min
            CODIn = 300f;     // 进水COD (mg/L)
            BOD5In = 150f;    // 进水BOD5 (mg/L)
            TNIn = 40f;       // 进水总氮 (mg/L)
            TPIn = 5f;        // 进水总磷 (mg/L)
            SSIn = 300f;        // 进水SS (mg/L)

            CODOUT = 300f;    // 出水COD (mg/L)
            BOD5OUT = 150f;   // 出水BOD5 (mg/L)
            TNOUT = 40f;     // 出水总氮 (mg/L)
            TPOUT = 5f;     // 出水总磷 (mg/L)
            SSOUT = 300f;        // 进水SS (mg/L)

            CODTemp = CODOUT;
            BOD5Temp = BOD5OUT;
            TNTemp = TNOUT;
            TPTemp = TPOUT;
            SSTemp = SSOUT;
        }
        else if (sceneIndex == 1)
        {
            Q = 20000f;       // 进水流量max
            Qmin = 10000f;       // 进水流量min
            CODIn = 1200f;     // 进水COD (mg/L)
            BOD5In = 180f;    // 进水BOD5 (mg/L)
            TNIn = 15f;       // 进水总氮 (mg/L)
            TPIn = 5f;        // 进水总磷 (mg/L)
            SSIn = 150f;        // 进水SS (mg/L)

            CODOUT = 1200f;    // 出水COD (mg/L)
            BOD5OUT = 180f;   // 出水BOD5 (mg/L)
            TNOUT = 15f;     // 出水总氮 (mg/L)
            TPOUT = 5f;     // 出水总磷 (mg/L)
            SSOUT = 150f;        // 进水SS (mg/L)

            CODTemp = CODOUT;
            BOD5Temp = BOD5OUT;
            TNTemp = TNOUT;
            TPTemp = TPOUT;
            SSTemp = SSOUT;
        }
        else if (sceneIndex == 2)
        {
            Q = 20000f;       // 进水流量max
            Qmin = 10000f;       // 进水流量min
            CODIn = 450f;     // 进水COD (mg/L)
            BOD5In = 100f;    // 进水BOD5 (mg/L)
            TNIn = 8f;       // 进水总氮 (mg/L)
            TPIn = 12f;        // 进水总磷 (mg/L)
            SSIn = 500f;        // 进水SS (mg/L)

            CODOUT = 450f;    // 出水COD (mg/L)
            BOD5OUT = 100f;   // 出水BOD5 (mg/L)
            TNOUT = 8f;     // 出水总氮 (mg/L)
            TPOUT = 12f;     // 出水总磷 (mg/L)
            SSOUT = 500f;        // 进水SS (mg/L)

            CODTemp = CODOUT;
            BOD5Temp = BOD5OUT;
            TNTemp = TNOUT;
            TPTemp = TPOUT;
            SSTemp = SSOUT;
        }
    }

    private float progress = 0f;
    private float[] Cost=new float[8];

    private bool cuGeshan = false;
    private bool xiGeshan = false;
    private bool AAO = false;
    private bool erChenChi = false;
    private bool shenChuangLvChi = false;
    private bool gaoMiDu = false;
    private bool ziWaiXiaoDu = false;
    private bool wuNiTuoShui = false;

    private bool isTrip = false;


    

    //输入
    public float GetQ()
    {
        return Q;
    }
    public void SetQ(float value)
    {
        Q = value;
    }
    public float GetQmin()
    {
        return Qmin;
    }
    public float GetCODIn()
    {
        return CODIn;
    }
    public float GetBOD5In()
    {
        return BOD5In;
    }
    public float GetTNIn()
    {
        return TNIn;
    }
    public float GetTPIn()
    {
        return TPIn;
    }

    public float GetSSIn()
    {
        return SSIn;
    }

    public float GetGermIn()
    {
        return GermIn;
    }

    public void SetQwuni(float value)
    {
        Qwuni= value;
    }

    public float GetQwuni()
    {
        return Qwuni;
    }

    // 输出
    public void SetCODOUT(float value)
    {
        CODOUT = Mathf.Clamp(value, 0, 1200);
    }
    public float GetCODOUT()
    {
        return CODOUT;
    }
    public void SetBOD5OUT(float value)
    {
        BOD5OUT = Mathf.Clamp(value, 0, 1000);
    }
    public float GetBOD5OUT()
    {
        return BOD5OUT;
    }
    public void SetTNOUT(float value)
    {
        TNOUT = Mathf.Clamp(value, 0, 1000);
    }
    public float GetTNOUT()
    {
        return TNOUT;
    }
    public void SetTPOUT(float value)
    {
        TPOUT = Mathf.Clamp(value, 0, 1000);
    }
    public float GetTPOUT()
    {
        return TPOUT;
    }
    public void SetSSOUT(float value)
    {
        SSOUT = Mathf.Clamp(value, 0, 1000);
    }
    public float GetSSOUT()
    {
        return SSOUT;
    }
    public void SetGermOUT(float value)
    {
        GermOUT = Mathf.Clamp(value, 0, 2000);
    }
    public float GetGermOUT()
    {
        return GermOUT;
    }


    //进度条
    public void SetProgress(float value)
    {
        progress = Mathf.Clamp(value, 0, 100);
    }

    public float GetProgress()
    {
        return progress;
    }

    public void setCost(float cost,int index)
    {
        Cost[index] = cost;
    }
    public float GetCost()
    {
        float totalCost=0;
        foreach(float cost in Cost)
        {
            totalCost += cost;
        }
        return totalCost;
    }

    public float GetCost(int index)
    {
        return Cost[index];
    }


    //器械bool值
    public void SetCuGeshan(bool value)
    {
        cuGeshan = value;
    }

    public bool GetCuGeshan()
    {
        return cuGeshan;
    }
    public void SetXiGeshan(bool value)
    {
        xiGeshan = value;
    }

    public bool GetXiGeshan()
    {
        return xiGeshan;
    }
    public void SetAAO(bool value)
    {
        AAO = value;
    }

    public bool GetAAO()
    {
        return AAO;
    }

    public void SetErChenChi(bool value)
    {
        erChenChi = value;
    }

    public bool GetErChenChi()
    {
        return erChenChi;
    }
    public void SetShenChuangLvChi(bool value)
    {
        shenChuangLvChi = value;
    }

    public bool GetShenChuangLvChi()
    {
        return shenChuangLvChi;
    }
    public void SetGaoMiDu(bool value)
    {
        gaoMiDu = value;
    }

    public bool GetgaoMiDu()
    {
        return gaoMiDu;
    }
    public void SetZiWaiXiaoDu(bool value)
    {
        ziWaiXiaoDu = value;
    }

    public bool GetZiWaiXiaoDu()
    {
        return ziWaiXiaoDu;
    }

    public void SetWuNiTuoShui(bool value)
    {
        wuNiTuoShui = value;
    }

    public bool GetWuNiTuoShui()
    {
        return wuNiTuoShui;
    }

    public void SetisTrip(bool value)
    {
        isTrip = value;
    }

    public bool GetisTrip()
    {
        return isTrip;
    }

    public float GetCODTemp()
    {
        return CODTemp;
    }
    public float GetBOD5Temp()
    {
        return BOD5Temp;
    }
    public float GetTNTemp()
    {
        return TNTemp;
    }
    public float GetTPTemp()
    {
        return TPTemp;
    }
    public float GetSSTemp()
    {
        return SSTemp;
    }

    public float GetQSave(int i)
    {
        return Qsave[i];
    }
    public void SetQSave(int i, float QValue)
    {
        Qsave[i] = QValue;
    }

    public float GetTNSave(int i)
    {
        return TNsave[i];
    }
    public void SetTNSave(int i, float TNValue)
    {
        TNsave[i] = TNValue;
    }
    public float GetTPSave(int i)
    {
        return TPsave[i];
    }
    public void SetTPSave(int i, float TPValue)
    {
        TPsave[i] = TPValue;
    }
    public float GetSSSave(int i)
    {
        return SSsave[i];
    }
    public void SetSSSave(int i, float SSValue)
    {
        SSsave[i] = SSValue;
    }

}
