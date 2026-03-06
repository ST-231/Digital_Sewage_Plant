using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class PauseUI : MonoBehaviour
{
    public Camera playerCam;
    public Camera pauseCam;

    public GameObject player;  // 玩家角色

    private Image img;
    private Image BG;
    private bool isPaused = false;  // 是否暂停的状态

    private bool closeUI = false;  // 是否暂停的状态
    private bool closeFromUI;  // 是否从其他界面关闭

    private Transform Left;
    private Transform Right;
    private GameObject settingUI;
    private GameObject controlUI;

    Coroutine current;


    // Start is called before the first frame update
    void Awake()
    {
        Transform child = transform.Find("Pause");
        img = child.GetComponent<Image>();
        Debug.Log(img);

        Left = this.transform.GetChild(0).GetChild(1);
        Right = this.transform.GetChild(0).GetChild(2);

        BG = this.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();

        settingUI = GameObject.FindGameObjectWithTag("SettingUI");

        // 开始时将 Alpha 设置为 0，暂停界面不可见
        Color color = img.color;
        color.a = 0;
        img.color = color;
    }

    private void OnEnable()
    {
        controlUI = GameObject.FindGameObjectWithTag("ControlUI");
        if (controlUI == null)
        {
            Debug.Log("no");
        }

    }


    // Update is called once per frame
    void Update()
    {
        // 按下 P 键时切换暂停和恢复
        if (Input.GetKeyDown(KeyCode.Escape)||closeUI)
        {
            StartCoroutine( TogglePause());
        }
    }

    public void closePause()
    {
        closeUI= true;
    }

    public void closePauseFast()
    {
        isPaused = false;

        closeUI = false;

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        this.transform.GetChild(0).gameObject.SetActive(false);

        pauseCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);

        Color color = img.color;
        color.a = 0;
        img.color = color;

        TogglePlayerInput(true);
    }

    public void guideclosePause()
    {
        pauseCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
    }

    IEnumerator TogglePause()
    {

        if (current != null)               //等待上一协程运行完毕
        {
            Debug.Log("wait");
            yield return current;
        }
        else
        {

            if (isPaused)
            {
                // 恢复游戏
                settingUI.transform.GetChild(0).gameObject.SetActive(false);
                current = StartCoroutine(ResumeGame());
            }
            else
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                // 暂停游戏
                current = StartCoroutine(PauseGame());
            }
        }
    }

    IEnumerator PauseGame()
    {

        TogglePlayerInput(false);
        playerCam.gameObject.SetActive(false);
        pauseCam.gameObject.SetActive(true);

        isPaused = true;

        // 渐变显示暂停界面
        BG.DOFade(1f, 1f);
        img.DOFade(1f, 1f).OnComplete(() =>
        {
            current = null;
            //Time.timeScale = 0f;
            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(true);

            });

        // 显示鼠标光标

         Cursor.visible = true;
         Cursor.lockState = CursorLockMode.None;
        
        yield return null;

    }

    IEnumerator ResumeGame()
    {

        isPaused = false;

        closeUI = false;

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 渐变隐藏暂停界面
        BG.DOFade(0f, 1f);
        img.DOFade(0f, 1f).OnComplete(() =>
        {
            current = null;
            this.transform.GetChild(0).gameObject.SetActive(false);

            pauseCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);

        });

        TogglePlayerInput(true);

        yield return null;
    }

    private void TogglePlayerInput(bool enable)
    {
        if (player != null)
        {
            var playerController = player.GetComponent<PlayerController>();  // 获取玩家控制脚本

            if (playerController != null)
            {
                playerController.SetInputEnabled(enable);  // 启用或禁用输入
                
            }
        }
    }
}


