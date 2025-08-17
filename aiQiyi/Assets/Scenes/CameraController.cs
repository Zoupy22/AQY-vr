using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("����ƶ��ٶ�")]
    public float moveSpeed = 10f;
    [Tooltip("��סShift�����ƶ��ı���")]
    public float sprintMultiplier = 2f;

    [Header("��ת����")]
    [Tooltip("���������")]
    public float mouseSensitivity = 100f;
    [Tooltip("��ֱ��ת����С�Ƕ�����")]
    public float minYAngle = -80f;
    [Tooltip("��ֱ��ת�����Ƕ�����")]
    public float maxYAngle = 80f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // ������굽��Ļ���Ĳ�����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ��ʼ����ת�Ƕ�Ϊ��ǰ�������ת
        Vector3 currentRotation = transform.eulerAngles;
        xRotation = currentRotation.x;
        yRotation = currentRotation.y;
    }

    void Update()
    {
        // ���������ת
        HandleMouseRotation();

        // ��������ƶ�
        HandleMovement();
    }

    private void HandleMouseRotation()
    {
        // ��ȡ�������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ������ת�Ƕ�
        yRotation += mouseX;
        xRotation -= mouseY;

        // ���ƴ�ֱ��ת�Ƕ�
        xRotation = Mathf.Clamp(xRotation, minYAngle, maxYAngle);

        // Ӧ����ת
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void HandleMovement()
    {
        // �����ƶ��ٶ�
        float currentSpeed = moveSpeed;

        // �����סShift�������
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // ��ȡWASD����
        float forward = Input.GetKey(KeyCode.W) ? 1f : 0f;
        float backward = Input.GetKey(KeyCode.S) ? 1f : 0f;
        float left = Input.GetKey(KeyCode.A) ? 1f : 0f;
        float right = Input.GetKey(KeyCode.D) ? 1f : 0f;

        // �����ƶ����� (���������ǰ����)
        Vector3 moveDirection = new Vector3(right - left, 0f, forward - backward).normalized;

        // ������ƶ����룬Ӧ���ƶ�
        if (moveDirection.magnitude >= 0.1f)
        {
            // ���������ǰ��������ƶ�
            Vector3 move = (transform.forward * moveDirection.z + transform.right * moveDirection.x) * currentSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }

        // ��ESC���������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
