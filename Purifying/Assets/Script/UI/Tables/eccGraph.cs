using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class eccGraph : MonoBehaviour
{
    private BarChart bar;
    public erChenChi ecc;
    private void OnEnable()
    {
        bar = gameObject.GetComponent<BarChart>();
        if (bar == null)
        {
            Debug.LogError("chart null");
        }

        bar.GetChartComponent<Tooltip>().numericFormatter = "F2";
        var series = bar.GetSerie(0);
        series.ClearData();
        series.itemStyle.color = new Color(1, 0, 0, 0.5f);

        var series2 = bar.GetSerie(2);
        series2.ClearData();

        var series1 = bar.GetSerie(1);
        series1.ClearData();
        series1.itemStyle.color = new Color(0.3f, 1, 0, 1f);


        AddDataToChart(0, 1, ecc.SSIn);

        AddDataToChart(1, 1, ecc.SSOUT);

        series2.AddXYData(ecc.SSRatio, 100, "SS去除率");


    }

    void AddDataToChart(int num, float xValue, float yValue)
    {
        // 获取或创建默认的数据系列（Series）
        if (bar == null)
        {
            Debug.LogError("图表未绑定！");
            return;
        }

        var series = bar.GetSerie(num);
        if (series == null)
        {
            Debug.Log("已创建新系列");
        }

        // 添加数据点
        series.AddXYData(xValue, yValue);

        // 刷新图表显示
        bar.RefreshChart();
    }
}
