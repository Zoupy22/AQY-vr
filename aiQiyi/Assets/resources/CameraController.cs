using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("相机移动速度")]
    public float moveSpeed = 10f;
    [Tooltip("按住Shift加速移动的倍数")]
    public float sprintMultiplier = 2f;

    [Header("旋转设置")]
    [Tooltip("鼠标灵敏度")]
    public float mouseSensitivity = 100f;
    [Tooltip("垂直旋转的最小角度限制")]
    public float minYAngle = -80f;
    [Tooltip("垂直旋转的最大角度限制")]
    public float maxYAngle = 80f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // 锁定鼠标到屏幕中心并隐藏
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 初始化旋转角度为当前相机的旋转
        Vector3 currentRotation = transform.eulerAngles;
        xRotation = currentRotation.x;
        yRotation = currentRotation.y;
    }

    void Update()
    {
        // 处理鼠标旋转
        HandleMouseRotation();

        // 处理键盘移动
        HandleMovement();
    }

    private void HandleMouseRotation()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 计算旋转角度
        yRotation += mouseX;
        xRotation -= mouseY;

        // 限制垂直旋转角度
        xRotation = Mathf.Clamp(xRotation, minYAngle, maxYAngle);

        // 应用旋转
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void HandleMovement()
    {
        // 基础移动速度
        float currentSpeed = moveSpeed;

        // 如果按住Shift键则加速
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // 获取WASD输入
        float forward = Input.GetKey(KeyCode.W) ? 1f : 0f;
        float backward = Input.GetKey(KeyCode.S) ? 1f : 0f;
        float left = Input.GetKey(KeyCode.A) ? 1f : 0f;
        float right = Input.GetKey(KeyCode.D) ? 1f : 0f;

        // 计算移动方向 (基于相机当前朝向)
        Vector3 moveDirection = new Vector3(right - left, 0f, forward - backward).normalized;

        // 如果有移动输入，应用移动
        if (moveDirection.magnitude >= 0.1f)
        {
            // 沿着相机的前向和右向移动
            Vector3 move = (transform.forward * moveDirection.z + transform.right * moveDirection.x) * currentSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }

        // 按ESC键解锁鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
