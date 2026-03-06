using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public static CharacterMove instance;
    public Transform player;
    private float mouseX;
    private float mouseY;
    public float mouseSensetivity;
    private float xRotation;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 如果已有实例，则销毁当前对象
        }
    }

    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensetivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensetivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);
        player.Rotate(Vector3.up * mouseX);
        transform.localRotation=Quaternion.Euler(xRotation,0,0);
        
    }
}
