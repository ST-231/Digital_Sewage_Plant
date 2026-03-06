using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUISwitch : MonoBehaviour
{
    private GameObject settingUI;
    private GameObject pauseUI;
    private GameObject controlUI;
    public GameObject graphUI;
    public GameObject taskUI;
    public Trip tripUI;

    void Start()
    {
        settingUI = GameObject.FindGameObjectWithTag("SettingUI");
        pauseUI = GameObject.FindGameObjectWithTag("PauseUI");
        controlUI = GameObject.FindGameObjectWithTag("ControlUI");
        tripUI.pauseUI = pauseUI;
        tripUI.open();
        if (graphUI == null) Debug.Log("ashkzc");
    }

    private void Update()
    {
        if(WaterData.Instance.GetProgress()==100f)
        {
            Debug.Log("asdasfghd");
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("ou");
                graphUI.SetActive(true);

                taskUI.SetActive(false);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }


    public void closeSetting()
    {
        settingUI.transform.GetChild(0).gameObject.SetActive(false);
        pauseUI.transform.GetChild(0).gameObject.SetActive(true);

    }

    public void openSetting()
    {
        settingUI.transform.GetChild(0).gameObject.SetActive(true);
        pauseUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void closeGraph()
    {
        graphUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void exitScene()
    {
        SceneManager.LoadScene("MainMap");
    }

}
