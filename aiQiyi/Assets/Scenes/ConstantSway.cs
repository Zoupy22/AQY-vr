using UnityEngine;

/// <summary>
/// ���ڵ��ƣ������壩�ϣ�������Ը��ڵ㣨�����㣩��Ȼҡ�ڡ�
/// ֻӰ�챾����ת�� X��Y �ᣬZ ��ʼ��Ϊ 0��
/// </summary>
[DisallowMultipleComponent]
public class ConstantSway : MonoBehaviour
{
    [Header("�ڶ�ǿ��")]
    [Tooltip("���ڶ��Ƕ�")]
    public float maxAngle = 10f;

    [Tooltip("�������屶��")]
    public float windStrength = 1f;

    [Header("�糡�仯")]
    [Tooltip("�糡ƽ������ʱ��")]
    public float windChangeSpeed = 0.5f;

    [Header("�����")]
    [Tooltip("Խ�����Խ��")]
    public float spring = 30f;
    [Tooltip("Խ������Խǿ")]
    public float damping = 5f;

    /*�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C�C*/

    // ��ǰ���ٶȣ����ؿռ�ŷ���ǣ�
    private Vector2 velocity;

    // ���������α�
    private float noiseTimeX, noiseTimeY;

    // Ŀ��Ƕȣ�ƽ����ķ�������
    private Vector2 targetAngle;

    void Awake()
    {
        // �����㣬��ֹ���е�ͬ��ҡ��
        noiseTimeX = Random.Range(0f, 100f);
        noiseTimeY = Random.Range(0f, 100f);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        /* 1. ���ɷ糡Ŀ��Ƕȣ�Perlin ������ */
        noiseTimeX += dt * windChangeSpeed;
        noiseTimeY += dt * windChangeSpeed * 1.3f; // �� XY ��ͬ��

        // ��Ƶ��� + ��ƵС��
        float windX =
            Mathf.PerlinNoise(noiseTimeX, 0) * 2f - 1f +
            (Mathf.PerlinNoise(noiseTimeX * 4f, 0.5f) * 2f - 1f) * 0.3f;

        float windY =
            Mathf.PerlinNoise(noiseTimeY, 0) * 2f - 1f +
            (Mathf.PerlinNoise(noiseTimeY * 4f, 0.7f) * 2f - 1f) * 0.3f;

        Vector2 newTarget = new Vector2(windX, windY) * maxAngle * windStrength;

        // ƽ����Ŀ��
        targetAngle = Vector2.Lerp(targetAngle, newTarget, dt * 5f);

        /* 2. ����-����ϵͳ */
        Vector2 current = new Vector2(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y);

        // �ѽǶ�ӳ�䵽 -180~180
        if (current.x > 180) current.x -= 360;
        if (current.y > 180) current.y -= 360;

        Vector2 springForce = -spring * (current - targetAngle);
        Vector2 dampingForce = -damping * velocity;

        Vector2 accel = springForce + dampingForce;

        velocity += accel * dt;
        current += velocity * dt;

        /* 3. Ӧ����ת��Z ��ʼ��Ϊ 0�� */
        transform.localEulerAngles = new Vector3(current.x, current.y, 0f);
    }
}