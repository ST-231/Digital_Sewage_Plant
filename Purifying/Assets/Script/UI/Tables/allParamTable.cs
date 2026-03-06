using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class allParamTable : MonoBehaviour
{
    public GameObject CellPrefab;
    public XiGeShan xsg;
    public GeShan sg;
    public wuNiTuoShui wnts;
    public ziWaiXiaoDu zwxd;
    public gaoMiDuChenDian gmdcd;
    public shenChuangLvChi sclc;
    public erChenChi ecc;
    public AAO aao;

    private static int time = 0;

    private List<List<string>> tableData;

    void OnEnable()
    {
        tableData = new List<List<string>>()
        {

        new List<string>{"粗栅格", "栅条宽度",sg.GetS().ToString("F2")+"m"},
        new List<string>{"粗栅格", "栅隙宽度",sg.Getb().ToString("F2") +"m" },
        new List<string>{"细栅格", "栅隙宽度", xsg.GetS().ToString("F2") +"m"},
        new List<string>{"细栅格", "栅隙宽度",sg.Getb().ToString("F2") +"m" },
        new List<string>{"AAO池", "内回流比",aao.Getr().ToString("F2") +"%"},
        new List<string>{"AAO池", "污泥浓度", aao.GetMLSS().ToString("F2") +"mg/L"},
        new List<string>{"AAO池", "好氧池HRT", aao.GetHRT().ToString("F2") +"h"},
        new List<string>{"AAO池", "好氧池DO", aao.GetDO().ToString("F2") +"mg/L"},
        new List<string>{"AAO池", "污泥龄", aao.GetSRT().ToString("F2") +"d"},
        new List<string>{"二沉池", "污泥回流比", ecc.GetR().ToString("F2") +"%"},
        new List<string>{"二沉池", "排泥量", ecc.GetQw().ToString("F2") +"m³/d"},
        new List<string>{"二沉池", "水力停留时间", ecc.GetHRT().ToString("F2") +"h"},
        new List<string>{"二沉池", "表面负荷", ecc.Getq().ToString("F2") +"m³/m²·h"},
        new List<string>{"高密度沉淀池", "表面负荷",gmdcd.Getq().ToString("F2") +"m³/m²·h"},
        new List<string>{"高密度沉淀池", "搅拌强度",gmdcd.GetG().ToString("F2") +"s⁻¹"},
        new List<string>{"高密度沉淀池", "水力停留时间", gmdcd.GetHRT().ToString("F2") +"h"},
        new List<string>{"高密度沉淀池", "药剂投加量", gmdcd.GetC().ToString("F2") +"mg/L"},
        new List<string>{"深床滤池", "滤料层厚度",sclc.GetH().ToString("F2") +"m"},
        new List<string>{"深床滤池", "过滤速度", sclc.Getv().ToString("F2") +"m/h"},
        new List<string>{"深床滤池", "反冲洗强度", sclc.Getqb().ToString("F2") +"L/(m²·s)"},
        new List<string>{"深床滤池", "滤料粒径", sclc.Getd().ToString("F2") +"mm"},
        new List<string>{"深床滤池", "反冲洗周期", sclc.GetT().ToString("F2") +"h"},
        new List<string>{"紫外消毒池", "紫外线强度",zwxd.GetI().ToString("F2") +"mW/cm²"},
        new List<string>{"紫外消毒池", "接触时间", zwxd.Gett().ToString("F2") +"h"},
        new List<string>{"污泥脱水间", "设备压力",wnts.GetP().ToString("F2") +"MPa"},
        new List<string>{"污泥脱水间", "脱水时间", wnts.Gett().ToString("F2") +"h"},
        };
        if (time == 0)
        {
            PopulateTable();
            time++;
        }
    }

    void PopulateTable()
    {

        GridLayoutGroup gridLayout = GetComponentInChildren<GridLayoutGroup>();
        foreach (var row in tableData)
        {
            GameObject cell = Instantiate(CellPrefab);
            Text[] cellTexts = cell.GetComponentsInChildren<Text>();

            for (int i = 0; i < 3; i++)
            {

                cellTexts[i].text = row[i];

            }
            // Set the new cell as a child of the GridLayoutGroup
            cell.transform.SetParent(gridLayout.transform, false);
        }
    }
}
