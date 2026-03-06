using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class jiesuan : MonoBehaviour
{

    private Transform Up;
    // Start is called before the first frame update
    void Start()
    {
        Up = this.transform.GetChild(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log(WaterData.Instance.GetProgress());
        if (WaterData.Instance.GetProgress() == 100f)
        {
            Up.gameObject.SetActive(true);
            Up.GetComponentInChildren<Text>().text = $"净化污水流量: {WaterData.Instance.GetQ():F2} m³/d\n\n" +
                            $"流水线运行成本: {WaterData.Instance.GetCost():F2} 元/d";
        }
    }
}
