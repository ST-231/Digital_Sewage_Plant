using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    public int index;
    public UIManage split;

    private GameObject thisUI;
    public GameObject PauseUI;
    public GameObject paraPanel;
    private bool hideParaPanel;


    private bool hide;
    // Start is called before the first frame update
    void Awake()
    {
        thisUI = this.gameObject;
        hideParaPanel = false;
        hide = false;
    }

    private void OnEnable()
    {
        thisUI.transform.GetChild(0).gameObject.SetActive(true);
        thisUI.transform.GetChild(1).gameObject.SetActive(true);
        thisUI.transform.GetChild(2).gameObject.SetActive(true);
        thisUI.transform.GetChild(3).gameObject.SetActive(true);
        thisUI.transform.GetChild(4).gameObject.SetActive(true);
        thisUI.transform.GetChild(5).gameObject.SetActive(true);
        thisUI.transform.GetChild(6).gameObject.SetActive(true);
        thisUI.transform.GetChild(8).gameObject.SetActive(false);
        thisUI.transform.GetChild(9).gameObject.SetActive(false);
        hide = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PauseUI.SetActive(false);

    }

    private void OnDisable()
    {
        PauseUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(!hide)
            {
                thisUI.transform.GetChild(0).gameObject.SetActive(false);
                thisUI.transform.GetChild(1).gameObject.SetActive(false);
                thisUI.transform.GetChild(2).gameObject.SetActive(false);
                thisUI.transform.GetChild(3).gameObject.SetActive(false);
                thisUI.transform.GetChild(4).gameObject.SetActive(false);
                hide = true;
            }
            else
            {
                thisUI.transform.GetChild(0).gameObject.SetActive(true);
                thisUI.transform.GetChild(1).gameObject.SetActive(true);
                thisUI.transform.GetChild(2).gameObject.SetActive(true);
                thisUI.transform.GetChild(3).gameObject.SetActive(true);
                thisUI.transform.GetChild(4).gameObject.SetActive(true);
                hide = false;
            }

        }
    }

    public void closeThis()
    {
        split.index = this.index;

        thisUI.SetActive(false);
    }

    public void openParaText()
    {
        if (hideParaPanel == true)
        {
            hideParaPanel = false;
        }
        else
        {
            hideParaPanel = true;
        }
        paraPanel.SetActive(hideParaPanel);
        Debug.Log("paraPanel" + hideParaPanel);
    }

}
