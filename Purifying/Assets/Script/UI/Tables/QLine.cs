using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class QLine : MonoBehaviour
{
    private LineChart lineChart;
    public XiGeShan xsg;
    public GeShan sg;
    public wuNiTuoShui wnts;
    public ziWaiXiaoDu zwxd;
    public gaoMiDuChenDian gmdcd;
    public shenChuangLvChi sclc;
    public erChenChi ecc;
    public AAO aao;

    private List<float> Qmax;
    private List<float> Costmin;

    void Start()
    {
        
        var series1 = lineChart.GetSerie(1);
        series1.ClearData();
        var series2 = lineChart.GetSerie(2);
        series2.ClearData();

        

        lineChart.GetChartComponent<Tooltip>().numericFormatter = "F2";

        if (LoadingScreenManager.nextSceneIndex == 0)
        {
            Costmin = new List<float>() { 10000f,
                10000f,10000f,9000f,7680.43f,6668.27f,5579.95f};
            Qmax = new List<float>() { 20000f,
                20000f,20000f,19172.39f,18215.99f,18183.07f,18028.21f};
        }
        else if (LoadingScreenManager.nextSceneIndex == 1)
        {
            Costmin = new List<float>() { 10000f,
                10000f,10000f,9000f,7680.43f,6668.27f,5579.95f};
            Qmax = new List<float>() { 20000f,
                20000f,20000f,19258.24f,18302.55f,18270.51f,18119.32f};
        }
        else if(LoadingScreenManager.nextSceneIndex == 2)
        {
            Costmin = new List<float>() { 10000f,
                10000f,10000f,9000f,7680.43f,6668.27f,5579.95f};
            Qmax = new List<float>() { 20000f,
                20000f,20000f,18887.37f,18008.55f,17976.21f,17796.38f};
        }

        for (int i = 0; i < 7; i++)
        {
            AddDataToChart(2, i + 1, Qmax[i]);
        }

        for (int i = 0; i < 7; i++)
        {
            AddDataToChart(1, i+1, Costmin[i]);
        }
    }

    private void OnEnable()
    {
       

        lineChart = gameObject.GetComponent<LineChart>();
        if (lineChart == null)
        {
            Debug.LogError("chart null");
        }

        var series = lineChart.GetSerie(0);
        series.ClearData();


        AddDataToChart(0,1, sg.getQ());
        AddDataToChart(0,2, xsg.getQ());
        AddDataToChart(0,3, aao.getQ());
        AddDataToChart(0,4, ecc.getQ());
        AddDataToChart(0,5, gmdcd.getQ());
        AddDataToChart(0,6, sclc.getQ());
        AddDataToChart(0,7, zwxd.getQ());

    }

    void AddDataToChart(int serie,float xValue, float yValue)
    {
        // 获取或创建默认的数据系列（Series）
        if (lineChart == null)
        {
            Debug.LogError("图表未绑定！");
            return;
        }

        var series = lineChart.GetSerie(serie);
        if (series == null)
        {
            Debug.Log("已创建新系列");
        }

        // 添加数据点
        series.AddXYData(xValue, yValue);

        // 刷新图表显示
        lineChart.RefreshChart();
    }
}
