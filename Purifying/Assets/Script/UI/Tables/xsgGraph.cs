using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class xsgGraph : MonoBehaviour
{
    private BarChart bar;
    public XiGeShan xsg;
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
        var series1 = bar.GetSerie(1);
        series1.ClearData();

        
        
        AddDataToChart(0, 1, xsg.B);
        AddDataToChart(0, 2, xsg.h1);
        AddDataToChart(0, 3, xsg.h);
        AddDataToChart(0, 4, xsg.v);



    }

    void AddDataToChart(int num,float xValue, float yValue)
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
