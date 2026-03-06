using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateModel : MonoBehaviour
{
    public Transform modelTransform; // 模型的 Transform
    private bool isRotate; // 是否正在旋转
    private Vector3 startPoint; // 鼠标起始点
    private float horizontalAngle; // 水平旋转角度
    private float verticalAngle; // 竖直旋转角度
    private Vector3 startPosition; // 模型起始位置
    public float rotateScale = 0.2f; // 旋转速度
    public float panSpeed = 0.05f; // 拖动平移速度
    public float zoomSpeed = 2f; // 缩放速度
    public float verticalAngleLimit = 80f; // 限制竖直旋转角度范围
    public float horizontalAngleLimit = 90f; // 限制水平旋转角度范围

    public Affilliate modelSelect;

    void Start()
    {
       

        // 初始化角度
        horizontalAngle = modelTransform.eulerAngles.y;
        verticalAngle = modelTransform.eulerAngles.x;

    }

    void Update()
    {
        // 鼠标左键按下开始旋转
        if (Input.GetMouseButtonDown(0) && !isRotate && modelSelect.IsOnAffiliate())
        {
            isRotate = true;
            startPoint = Input.mousePosition;
        }

        // 鼠标左键抬起停止旋转
        if (Input.GetMouseButtonUp(0))
        {
            isRotate = false;
        }

        // 左键旋转模型
        if (isRotate && modelSelect.IsOnAffiliate())
        {
            var currentPoint = Input.mousePosition;
            var delta = currentPoint - startPoint;

            // 更新水平和竖直角度
            horizontalAngle += delta.x * rotateScale;
            verticalAngle = Mathf.Clamp(verticalAngle + delta.y * rotateScale, -verticalAngleLimit, verticalAngleLimit);
            // 限制水平旋转角度在 -90 到 90 度之间
            horizontalAngle = Mathf.Clamp(horizontalAngle, -horizontalAngleLimit, horizontalAngleLimit);

            modelTransform.rotation = Quaternion.Euler(verticalAngle, -horizontalAngle, 0);


            startPoint = currentPoint; // 更新起始点
        }

        // 右键拖动模型
        if (Input.GetMouseButtonDown(1) && modelSelect.IsOnAffiliate())
        {
            startPoint = Input.mousePosition;
            startPosition = modelTransform.position;
        }

        if (Input.GetMouseButton(1) && modelSelect.IsOnAffiliate())
        {
            var currentPoint = Input.mousePosition;
            var delta = (currentPoint - startPoint) * panSpeed;
            modelTransform.position = startPosition + new Vector3(delta.x, delta.y, 0);
        }

        // 鼠标滚轮缩放模型
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && modelSelect.IsOnAffiliate())
        {
            modelTransform.localScale += Vector3.one * scroll * zoomSpeed;
            modelTransform.localScale = Vector3.Max(modelTransform.localScale, Vector3.one * 0.1f); // 防止缩放过小
        }
    }
}
