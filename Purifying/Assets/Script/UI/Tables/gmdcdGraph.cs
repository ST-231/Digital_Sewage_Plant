using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class gmdcdGraph : MonoBehaviour
{
    private BarChart bar;
    public gaoMiDuChenDian gmdcd;
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



        var series1 = bar.GetSerie(1);
        series1.ClearData();
        series1.itemStyle.color = new Color(0.3f, 1, 0, 1f);

        var series2 = bar.GetSerie(2);
        series2.ClearData();

        var series3 = bar.GetSerie(3);
        series3.ClearData();

        AddDataToChart(0, 1, gmdcd.TPIn);
        AddDataToChart(0, 2, gmdcd.SSIn);

        AddDataToChart(1, 1, gmdcd.TPOut);
        AddDataToChart(1, 2, gmdcd.SSOut);

        series2.AddXYData(gmdcd.Rtp, 100, "总磷去除率");

        series3.AddXYData(gmdcd.Rs, 100, "SS去除率");

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
