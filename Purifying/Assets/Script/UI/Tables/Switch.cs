using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    private GameObject[] charts=new GameObject[12];
    public Dropdown dropDown;
    public GameObject taskUI;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            charts[i] = this.transform.GetChild(0).GetChild(i).gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void dropdownControl()
    {
        Debug.Log("DropdownChange:" + dropDown.value);
        if (dropDown.value == 0) openpanel(0);
        else openpanel(dropDown.value+3);
    }

    public void open()
    {
        this.gameObject.SetActive(true);
    }

    public void openpanel(int index)
    {


        for (int i = 0; i < charts.Length; i++)
        {
            charts[i].SetActive(false);
        }
        charts[index].SetActive(true);
    }

}
