using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class CostLine : MonoBehaviour
{
    private LineChart lineChart;
    private List<float> Costmin;
    private List<float> Qmax;

    void Start()
    {
        
        var series1 = lineChart.GetSerie(1);
        series1.ClearData();

        var series2 = lineChart.GetSerie(2);
        series2.ClearData();


        lineChart.GetChartComponent<Tooltip>().numericFormatter = "F2";

        if (LoadingScreenManager.nextSceneIndex == 0)
        {
            Costmin = new List<float>() { 17.49f,
                85.38f,2771.21f,197.08f,630.99f,120.94f,193.58f,34.22f};

            Qmax = new List<float>() { 34.98f,
                170.75f,5632.98f,662.20f,2969.94f,309.17f,618.94f,45.79f};

        }
        else if (LoadingScreenManager.nextSceneIndex == 1)
        {
            Costmin = new List<float>() { 17.49f,
                85.38f,3067.32f,154.20f,523.64f,114.33f,193.64f,29.98f};

            Qmax = new List<float>() { 34.98f,
                170.75f,6348.13f,595.33f,1790.11f,312.11f,328.48f,91.51f};
        }
        else if (LoadingScreenManager.nextSceneIndex == 2)
        {
            Costmin = new List<float>() { 17.49f,
                85.38f,3906.98f,129.98f,469.12f,116.75f,200.04f,22.15f};

            Qmax = new List<float>() { 34.98f,
                170.75f,7945.49f,576.33f,1675.19f,306.04f,607.58f,90.92f};
        }

        float cost = 0;
        for (int i = 0; i < 8; i++)
        {
            cost += Costmin[i];
            AddDataToChart(1, i + 1, cost);
        }

        cost = 0;
        for (int i = 0; i < 8; i++)
        {
            cost += Qmax[i];
            AddDataToChart(2, i + 1, cost);
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


        float cost = 0;
        for (int i = 0;i<8;i++)
        {
            cost +=WaterData.Instance.GetCost(i);
            AddDataToChart(0,i + 1, cost);
        }

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


    void AddSeriesData()
    {
        // 添加新系列
        var series = lineChart.AddSerie<Line>("sales");

        // 基础设置
        series.symbol.show = true;
        series.symbol.type = SymbolType.Circle;
        series.symbol.size = 8;
        series.lineStyle.width = 2;
        series.lineStyle.type = LineStyle.Type.Solid;

        // 动画设置
        series.animation.enable = true;

        // 数据填充
        series.AddData(new List<double> { 20, 34, 45, 55, 70, 85 }, "销售额");

        // 颜色设置
        series.itemStyle.color = new Color32(65, 105, 225, 255); // 皇家蓝
        series.areaStyle.show = true;
        series.areaStyle.color = new Color(0.25f, 0.54f, 0.89f, 0.3f);
    }

}

