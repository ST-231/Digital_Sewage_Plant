using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//主菜单UI切换

public class UISwitch : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject settingUI;
    private GameObject levelUI;
    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        settingUI = GameObject.FindGameObjectWithTag("SettingUI");
        levelUI = GameObject.FindGameObjectWithTag("LevelUI");
    }


    public void closeSetting()
    {
        mainMenu.transform.GetChild(0).gameObject.SetActive(true);
        settingUI.transform.GetChild(0).gameObject.SetActive(false);
        
    }

    public void openSetting()
    {
        mainMenu.transform.GetChild(0).gameObject.SetActive(false);
        settingUI.transform.GetChild(0).gameObject.SetActive(true);

    }

    public void closeLevel()
    {
        mainMenu.transform.GetChild(0).gameObject.SetActive(true);
        levelUI.transform.GetChild(0).gameObject.SetActive(false);

    }

    public void openLevel()
    {
        mainMenu.transform.GetChild(0).gameObject.SetActive(false);
        levelUI.transform.GetChild(0).gameObject.SetActive(true);

    }

    public void exitGame()
    {
#if UNITY_EDITOR
        // 在 Unity 编辑器中停止运行游戏
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在打包的游戏中退出游戏
        Application.Quit();
#endif
    }
}
