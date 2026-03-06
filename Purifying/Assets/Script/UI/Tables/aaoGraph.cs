using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class aaoGraph : MonoBehaviour
{
    private BarChart bar;
    public AAO aao;
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

        var series4 = bar.GetSerie(4);
        series4.ClearData();

        var series5 = bar.GetSerie(5);
        series5.ClearData();

        AddDataToChart(0, 1, aao.TNIn);
        AddDataToChart(0, 2, aao.BOD5In);
        AddDataToChart(0, 3, aao.CODIn);
        AddDataToChart(0, 4, aao.TPIn);

        AddDataToChart(1, 1, aao.TNOUT);
        AddDataToChart(1, 2, aao.BOD5OUT);
        AddDataToChart(1, 3, aao.CODOUT);
        AddDataToChart(1, 4, aao.TPOUT);

        series2.AddXYData(aao.Rtn, 100, "总氮去除率");
        series3.AddXYData(aao.Rbod, 100, "BOD5去除率");
        series4.AddXYData(aao.Rcod, 100, "COD去除率");
        series5.AddXYData(aao.Rtp, 100, "总磷去除率");

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
