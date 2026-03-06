using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObserveController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // 拖拽虚拟相机到这里
    public float fovChangeSpeed = 0.1f; // FOV 变化速度
    private float verticalMove;
    public float moveSpeed;
    private float Rotation;
    private CinemachineComposer composer;
    private CinemachineTrackedDolly dolly;
    private float currentPathPosition;
    private Vector3 offset;


    private void Start()
    {
        if (virtualCamera != null)
        {
            composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            currentPathPosition = dolly.m_PathPosition;
        }
    }



    private void Update()
    {
        float scrollValue = Mouse.current.scroll.ReadValue().y;
        verticalMove = Input.GetAxis("Vertical") * moveSpeed*Time.deltaTime;
        Rotation += verticalMove;
        Rotation = Mathf.Clamp(Rotation, -20f, 20f);
        offset = new Vector3(0f, Rotation, 0f);

        float horizontalInput = Input.GetAxis("Horizontal") *moveSpeed * Time.deltaTime/100f;
        currentPathPosition -= horizontalInput;
        

        if (virtualCamera != null)
        {

            // 获取当前 FOV
            float currentFOV = virtualCamera.m_Lens.FieldOfView;

            // 修改 FOV
            currentFOV -= scrollValue * fovChangeSpeed;

            //限制 FOV 范围
            currentFOV = Mathf.Clamp(currentFOV, 5f, 100f);

            // 应用新的 FOV
            virtualCamera.m_Lens.FieldOfView = currentFOV;

            if (composer != null)
            {
                dolly.m_PathPosition = currentPathPosition;
                composer.m_TrackedObjectOffset=offset;
            }
        }


    }
}
