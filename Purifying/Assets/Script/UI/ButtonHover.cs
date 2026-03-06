using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public ParticleSystem uiParticle;
    public GameObject intro;
    public List<string> errMsg;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.MinMaxGradient orig;

    private void Start()
    {
        uiParticle.transform.localPosition = this.transform.localPosition;

        mainModule = uiParticle.main;
        orig = mainModule.startColor;
        uiParticle.Stop();
    }

    private void OnEnable()
    {
        uiParticle.Stop();
        intro.SetActive(false);
    }

    // 鼠标进入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("errnum:" + errMsg.Count);
        string allmsg = "";
        if (errMsg.Count != 0)
        {
            mainModule.startColor = Color.red;
            foreach (string msg in errMsg)
            {
                allmsg += msg + "\n\n";
            }
        }
        else
        {
            allmsg = "设备运行正常";
            mainModule.startColor = orig;
        }

        

        intro.GetComponentInChildren<Text>().text = allmsg;

        Debug.Log("鼠标悬浮在按钮上！");
        uiParticle.Play();
        intro.SetActive(true);

        // 其他逻辑（如播放音效、显示提示等）
    }

    // 鼠标离开时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        uiParticle.Stop();
        uiParticle.Clear();
        intro.SetActive(false);
        Debug.Log("鼠标离开按钮！");
    }
}