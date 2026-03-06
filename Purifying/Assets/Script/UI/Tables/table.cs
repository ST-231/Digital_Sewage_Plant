using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class table : MonoBehaviour
{
    public GameObject CellPrefab;
    private static int time=0; 

    private List<List<string>> tableData;

    void OnEnable()
    {
        tableData = new List<List<string>>()
        {

        new List<string>{ "BOD5",WaterData.Instance.GetBOD5In().ToString("F2") +"mg/L",WaterData.Instance.GetBOD5OUT().ToString("F2") +"mg/L"},
        new List<string>{ "COD", WaterData.Instance.GetCODIn().ToString("F2") +"mg/L",WaterData.Instance.GetCODOUT().ToString("F2") +"mg/L"},
        new List<string>{ "总氮", WaterData.Instance.GetTNIn().ToString("F2") +"mg/L",WaterData.Instance.GetTNOUT().ToString("F2") +"mg/L"},
        new List<string>{ "总磷",WaterData.Instance.GetTPIn().ToString("F2") +"mg/L",WaterData.Instance.GetTPOUT().ToString("F2") +"mg/L"},
        new List<string>{ "SS", WaterData.Instance.GetSSIn().ToString("F2") +"mg/L",WaterData.Instance.GetSSOUT().ToString("F2") +"mg/L"},
        new List<string>{ "粪大肠菌群", WaterData.Instance.GetGermIn().ToString("F2") +"mg/L",WaterData.Instance.GetGermOUT().ToString("F2") +"mg/L"},
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
