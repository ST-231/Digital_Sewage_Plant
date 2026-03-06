using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FunctionToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public Toggle myToggle;
    //用于点亮时显示
    public Affilliate myAffilliate;

    private CanvasGroup myAffilliateCanvasGroup;
    private CanvasGroup myCanvasGroup;

    //全局管理所有 Toggle
    public static List<FunctionToggle> allToggles = new List<FunctionToggle>();

    public bool isEnterAffiliate;

    private void Awake()
    {
        myToggle = GetComponent<Toggle>();
    }

    void Start()
    {
        myToggle.onValueChanged.AddListener(OnMyToggleValueChanged);
        myCanvasGroup = GetComponent<CanvasGroup>();
        if (myAffilliate)
        {
            myAffilliateCanvasGroup = myAffilliate.gameObject.AddComponent<CanvasGroup>();
        }
        


        // 添加当前脚本实例到管理列表
        allToggles.Add(this);
    }

    private void OnMyToggleValueChanged(bool arg0)
    {
        if (myAffilliate)
        {
            myAffilliate.gameObject.SetActive(arg0);
        }
        if (arg0)
        {
            
            myCanvasGroup.DOFade(1f, 0.3f);

            // 点击当前 Toggle 后，关闭其他 Toggle
            foreach (var toggle in allToggles)
            {
                if (toggle != this && toggle.myToggle.isOn)
                {
                    toggle.myToggle.isOn = false;
                }
            }
        }
        else
        {
            StopHideCoroutine();
            myCanvasGroup.DOFade(0.3f, 0.3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Coroutine hideCoroutine;

    //监测鼠标在图标上
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myAffilliateCanvasGroup)
        {
            myAffilliateCanvasGroup.DOFade(1f, 0.3f);
        }
        StopHideCoroutine();
        
    }

    //监测鼠标不在图标上
    public void OnPointerExit(PointerEventData eventData)
    {
        hideCoroutine = StartCoroutine(HideAffilliateDelay());
        
    }

    private void StopHideCoroutine()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    //等待五秒执行
    private IEnumerator HideAffilliateDelay()
    {
        yield return new WaitForSeconds(5f);
        if (myAffilliateCanvasGroup)
        {
            myAffilliateCanvasGroup.DOFade(0f, 0.3f);
        }
    }

    public void OnMouseEnterAffiliate(bool isenter)
    {
        if (isenter)
        {
            StopHideCoroutine();
        }
        else
        {
            hideCoroutine = StartCoroutine(HideAffilliateDelay());
        }
    }



}

