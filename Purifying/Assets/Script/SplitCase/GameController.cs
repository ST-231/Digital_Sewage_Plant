using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Camera splitCamera;
    public Camera playerCam;
    public Camera uiCam;
    public GameObject splitUI;

    //使用实例就可以在UIManage类中引用该类的变量
    public static GameController Instance;
    public Animator engineAnimator;

    //用来存储模型数组和动画数组，每一个模型对应相应的动画
    public GameObject[] models;
    public Animator[] modelAnimators;
    public int index = 0;

    private void Awake()
    {
        engineAnimator = modelAnimators[index];
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //engineAnimator = modelAnimators[index];
    }

    public void PlayDisassembleAnimation(bool isForward)
    {
        if (isForward)
        {
            engineAnimator.Play("Disassemble");
        }
        else
        {
            engineAnimator.Play("PlayBack");
        }
    }

    public void openSplit()
    {   
        
        playerCam.gameObject.SetActive(false);
        splitCamera.gameObject.SetActive(true);
        uiCam.gameObject.SetActive(true);
        splitUI.gameObject.SetActive(true);
    }

    public void closeSplit()
    {
        splitCamera.gameObject.SetActive(false);
        uiCam.gameObject.SetActive(false);
        splitUI.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
    }

}
