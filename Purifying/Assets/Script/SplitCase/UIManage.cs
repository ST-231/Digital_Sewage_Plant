using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


public class UIManage : MonoBehaviour
{
    public GameObject[] retUI;
    public int index;

    //拆解的toggle
    public FunctionToggle disassembleFunctionToggle;
    public FunctionToggle introduceFunctionToggle;
    
    private bool guide=false;

    //按钮的集合，在unity面板进行赋值
    public List<Button> selectBtnList;
    public List<GameObject> introduceText;

    //preIndex用来保存上一个Index，在点击新的模型时，对旧模型进行不可视的设置
    private int preIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        disassembleFunctionToggle.myToggle.onValueChanged.AddListener(OnDisassembleFunctionToggle);
        introduceFunctionToggle.myToggle.onValueChanged.AddListener(OnIntroduceFunctionToggle);
        //遍历监听按钮数组
        for(int i = 0; i < selectBtnList.Count; i++)
        {
            //分别监听每一个按钮，按下即进行OnSelectModel方法的使用
            Button btn = selectBtnList[i];
            btn.onClick.AddListener(() => OnSelectModel(btn));
        }
    }

    

    private void OnSelectModel(Button btn)
    {
        //当切换时清空自身的active记录
        if (GameController.Instance.models[preIndex].activeSelf)
        {
            GameController.Instance.models[preIndex].SetActive(false);
            introduceText[preIndex].gameObject.SetActive(false);
        }
        var index = selectBtnList.IndexOf(btn);
        GameController.Instance.engineAnimator = GameController.Instance.modelAnimators[index];
        GameController.Instance.models[index].SetActive(true);

        preIndex = index;
        
    }

    private void OnDisassembleFunctionToggle(bool arg0)
    {
        GameController.Instance.PlayDisassembleAnimation(arg0);
    }

    private void OnIntroduceFunctionToggle(bool arg0)
    {
        introduceText[preIndex].gameObject.SetActive(arg0);
    }


    void Update()
    {

    }

    public void open()
    {
        GameController.Instance.engineAnimator = GameController.Instance.modelAnimators[index];
        GameController.Instance.models[0].SetActive(false);
        GameController.Instance.models[index].SetActive(true);
        preIndex = index;
    }

    public void open(bool gd)
    {
        GameController.Instance.engineAnimator = GameController.Instance.modelAnimators[index];
        GameController.Instance.models[0].SetActive(false);
        GameController.Instance.models[index].SetActive(true);
        preIndex = index;
        guide = gd;
    }

    public void back()
    {
        GameController.Instance.models[index].SetActive(false);
        GameController.Instance.models[0].SetActive(true);
        retUI[index].SetActive(true);
        if(guide)
        {
            retUI[index].transform.GetChild(0).gameObject.SetActive(false);
            retUI[index].transform.GetChild(1).gameObject.SetActive(true);
            retUI[index].transform.GetChild(2).gameObject.SetActive(false);
            retUI[index].transform.GetChild(3).gameObject.SetActive(false);
            retUI[index].transform.GetChild(4).gameObject.SetActive(false);
            retUI[index].transform.GetChild(5).gameObject.SetActive(false);
            retUI[index].transform.GetChild(6).gameObject.SetActive(false);
            retUI[index].transform.GetChild(7).gameObject.SetActive(false);
            retUI[index].transform.GetChild(8).gameObject.SetActive(true);
            retUI[index].transform.GetChild(9).gameObject.SetActive(true);

        }
        guide = false;
    }
}
