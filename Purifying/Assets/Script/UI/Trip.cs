using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trip : MonoBehaviour
{

    public GameObject pauseUI;
    public GameObject cebianUI;
    public GameObject taskUI;
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera[] cameras;
    public CameraSwitchOnProximity[] targets;

    public GameObject player;  // 玩家角色

    private int currentCameraIndex = 0;
    private float speed ;

    private CinemachineTrackedDolly _currentDolly;     // 当前相机的轨道组件
    private float _pathLength;                         // 轨道总长度
    private bool _isMoving = false;                    // 是否正在移动

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void open()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void close()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void trip()
    {
        currentCameraIndex = 0;
        TogglePlayerInput(false);
        pauseUI.SetActive(false);
        StartCoroutine(SwitchCameras(false,false));

    }

    public void skiptrip()
    {
        TogglePlayerInput(false);
        pauseUI.SetActive(false);
        StartCoroutine(SwitchCameras(true,false));

    }

    public void midtrip()
    {

        currentCameraIndex = 0;
        TogglePlayerInput(false);
        pauseUI.SetActive(false);
        StartCoroutine(SwitchCameras(false,true));

    }

    void camBack()
    {
        foreach (var vcam in cameras)
        {
            vcam.gameObject.SetActive(false);
        }
        mainCamera.gameObject.SetActive(true);
        TogglePlayerInput(true);
        pauseUI.SetActive(true);
        cebianUI.SetActive(false);
        taskUI.SetActive(true);
        taskUI.transform.GetChild(0).gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator SwitchCameras(bool skip,bool midtrip)
    {
        cebianUI.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        while (true)
        {
            Debug.Log("timehh");
            speed = 1000f;
            pauseUI.SetActive(false);
            // 循环切换镜头
            if (currentCameraIndex != 0)
            {
                targets[currentCameraIndex-1].tripOpen(midtrip);
            }
            else
            {
                speed = 2f;   //第一个上升自动
            }

            cameras[currentCameraIndex].gameObject.SetActive(true);

            _currentDolly = cameras[currentCameraIndex].GetCinemachineComponent<CinemachineTrackedDolly>();
            if (_currentDolly == null || _currentDolly.m_Path == null)
            {
                Debug.LogError("目标相机未绑定轨道！");
                yield break;
            }

            _pathLength = _currentDolly.m_Path.MaxPos;
            

            // 记录移动的起始位置
            float startPosition = 0;
            _isMoving = true;


            _currentDolly.m_PathPosition = 0;

            float temp = 0;
            if (skip)
            {
                temp = 8;
            }

            Debug.Log("_isMoving " + _isMoving);
            // 持续移动，直到绕完一圈
            while (_isMoving)
            {
                // 更新位置
                float upd = Time.deltaTime / speed;
                temp += upd;
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    speed = 0.1f;
                }
                _currentDolly.m_PathPosition = temp;
                

                // 检测是否完成一圈（位置超过起点+轨道长度）
                if (Mathf.Abs(_currentDolly.m_PathPosition) >= startPosition + _pathLength)
                {
                    _isMoving = false;
                    Debug.Log("一圈");
                    break;
                }
                yield return null;
            }

            cameras[currentCameraIndex].gameObject.SetActive(false);

            if (currentCameraIndex != 0)
            {
                targets[currentCameraIndex-1].tripClose();
            }

            Debug.Log("iii" + currentCameraIndex);

            currentCameraIndex = currentCameraIndex + 1;
            if(currentCameraIndex >= cameras.Length)
            {
                camBack();
                yield break;
            }
            //yield return new WaitForSeconds(3f);
        }
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
