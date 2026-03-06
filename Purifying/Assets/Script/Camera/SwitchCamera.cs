using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;


//玩家视角切换到观察机器视角

public class CameraSwitchOnProximity : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;  // 当前相机
    public CinemachineVirtualCamera targetCamera;  // 目标相机

    public GameObject ControlUI;
    private GameObject PauseUI;  //最外层

    public GameObject tipUI;

    public GameObject player;  // 玩家角色
    private BoxCollider interactionZone;  // 目标物体上的 BoxCollider 区域
    public bool canPressE = false;  // 是否可以按下 E 键

    private SequentialEButton seqButton;//确保交互顺序

    private void Start()
    {
        interactionZone = GetComponent<BoxCollider>();
        PauseUI = GameObject.FindGameObjectWithTag("PauseUI");
        seqButton = GetComponent<SequentialEButton>();  // 获取顺序交互组件

        // 默认让提示面板显示（E 按钮始终显示）
        if (tipUI != null)
            tipUI.SetActive(true);
    }

    void Update()
    {
        if (canPressE && Input.GetKeyDown(KeyCode.E))
        {
            // 先判断顺序交互条件
            if (seqButton != null)
            {
                seqButton.TryInteract();
            }
            else
            {
                onSwitch();
            }
        }

        if (tipUI != null && tipUI.activeSelf)
        {
            tipUI.transform.LookAt(player.transform);
            tipUI.transform.Rotate(0, 180, 0);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("jinle");
            canPressE = true;  // 玩家进入交互范围，允许按 E 键
                               // 不再调用 tipUI.SetActive(true)（因为已经默认显示）
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("zoule");
            canPressE = false;  // 玩家离开交互范围，不允许按 E 键
                                // 不隐藏提示面板，让 E 按钮始终显示
        }
    }

    public void onSwitch()
    {


            Debug.Log("延迟 2 秒后执行的操作");
            PauseUI.transform.GetChild(0).gameObject.SetActive(false);

            int flag = SwitchCamera();  // 切换相机

            if (flag != 0)
            {
                TogglePlayerInput(true);
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                TogglePlayerInput(false);
                //Cursor.visible = true;
                //Cursor.lockState = CursorLockMode.Locked;
            }// 禁用玩家输入


    }

    // 切换到目标相机
    private int SwitchCamera()
    {
        if (mainCamera != null && targetCamera != null)
        {
            if (mainCamera.isActiveAndEnabled)    //进入界面
            {
                Debug.Log("jinru");
                MonoBehaviour[] allScripts = this.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour script in allScripts)
                {
                    if (script.GetType().Name != "CameraSwitchOnProximity")
                    {
                        script.enabled = true;
                    }
                }

                mainCamera.gameObject.SetActive(false);  // 禁用当前相机
                targetCamera.gameObject.SetActive(true);  // 启用目标相机

                PauseUI.SetActive(false);

                ControlUI.SetActive(true);
                tipUI.SetActive(false);

                //PauseUI.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                return 0;
            }
            else if (targetCamera.isActiveAndEnabled)   //退出界面
            {
                Debug.Log("tuichu");

                MonoBehaviour[] allScripts = this.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour script in allScripts)
                {
                    if (script.GetType().Name != "CameraSwitchOnProximity")
                    {
                        script.enabled = false;
                    }
                }

                mainCamera.gameObject.SetActive(true);
                targetCamera.gameObject.SetActive(false);

                ControlUI.SetActive(false);
                tipUI.SetActive(true);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                return 1;
            }

        }
        return 1;
    }

    // 控制玩家输入（启用或禁用）
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


    public void tripOpen(bool midTrip)
    {
        MonoBehaviour[] allScripts = this.GetComponents<MonoBehaviour>();
        WaterData.Instance.SetisTrip(true);

        if (!midTrip)
        {
            foreach (MonoBehaviour script in allScripts)
            {
                if (script.GetType().Name != "CameraSwitchOnProximity")
                {
                    script.enabled = true;
                }
            }
        }


        ControlUI.SetActive(true);
        ControlUI.transform.GetChild(0).gameObject.SetActive(false);
        ControlUI.transform.GetChild(2).gameObject.SetActive(false);
        ControlUI.transform.GetChild(3).gameObject.SetActive(false);
        ControlUI.transform.GetChild(4).gameObject.SetActive(false);
        ControlUI.transform.GetChild(5).gameObject.SetActive(false);
        ControlUI.transform.GetChild(6).gameObject.SetActive(false);
        ControlUI.transform.GetChild(7).gameObject.SetActive(false);
        ControlUI.transform.GetChild(8).gameObject.SetActive(true);
        ControlUI.transform.GetChild(9).gameObject.SetActive(true);
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void tripClose()
    {
        MonoBehaviour[] allScripts = this.GetComponents<MonoBehaviour>();
        WaterData.Instance.SetisTrip(false);
        foreach (MonoBehaviour script in allScripts)
        {
            if (script.GetType().Name != "CameraSwitchOnProximity")
            {
                script.enabled = false;
            }
        }

        ControlUI.SetActive(false);

    }


    // 通用的延迟协程
    private IEnumerator DelayAction(float delayTime, System.Action callback)
    {
        yield return new WaitForSeconds(delayTime);
        callback?.Invoke();
    }
}