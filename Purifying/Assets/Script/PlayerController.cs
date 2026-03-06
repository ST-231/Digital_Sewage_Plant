using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;

    //public float moveSpeed;

    private float horizontalMove, verticalMove;

    private Vector3 dir;
    private bool isInputEnabled = true;  // 是否启用输入

    //private bool isPauseOpen = false;

    private GameObject pauseUI;

    //--------------------------------------------
    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    public float rotationSpeed = 3f;
    public float groundDrag = 5f;
    public ForceMode moveForceMode = ForceMode.Acceleration;

    [SerializeField] private Rigidbody rb;



    // Start is called before the first frame update
    private void Start()
    {
        cc= GetComponent<CharacterController>();
        pauseUI = GameObject.FindGameObjectWithTag("PauseUI");

        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false; // 隐藏鼠标

        //--------------------------------------------------
        if (!rb) rb = GetComponent<Rigidbody>();

        // 设置物理参数
        rb.freezeRotation = true; // 防止物理旋转
        rb.drag = groundDrag;
    }

    // Update is called once per frame
    /*
    private void Update()
    {
        /*
        if (isInputEnabled)
        {
            horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
            verticalMove = Input.GetAxis("Vertical") * moveSpeed;
            dir = transform.forward * verticalMove + transform.right * horizontalMove;
            cc.Move(dir * Time.deltaTime);
        }
        

        if (isInputEnabled)
        {
            HandleMovement();
            HandleRotation();
            LimitSpeed();
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    isPauseOpen = !isPauseOpen;

        //    pauseUI.transform.GetChild(0).gameObject.SetActive(isPauseOpen);

        //    //if (isPauseOpen)
        //    //{
        //    //    // 暂停游戏
        //    //    //Time.timeScale = 0;
        //    //    Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        //    //    Cursor.visible = true; // 显示鼠标
        //    //}
        //    //else
        //    //{
        //    //    // 恢复游戏
        //    //    Time.timeScale = 1;
        //    //    Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        //    //    Cursor.visible = false; // 隐藏鼠标
        //    //}
        //}
    }*/

    // 在FixedUpdate中处理物理逻辑
    private void FixedUpdate()
    {
        if (isInputEnabled)
        {
            HandleMovement();
            HandleRotation();
            LimitSpeed();
        }
    }

    void LimitSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 添加时间增量计算
        float speedMultiplier = Time.fixedDeltaTime * 50f;

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 worldDirection = transform.TransformDirection(moveDirection);

        // 修改力的施加方式
        rb.AddForce(worldDirection * moveSpeed * speedMultiplier, moveForceMode);
    }
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
    }



    public void SetInputEnabled(bool enable)
    {
        isInputEnabled = enable;
    }
}
