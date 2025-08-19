using UnityEngine;

/// <summary>
/// 挂在吊灯（子物体）上，让它相对父节点（悬吊点）自然摇摆。
/// 只影响本地旋转的 X、Y 轴，Z 轴始终为 0。
/// </summary>
[DisallowMultipleComponent]
public class ConstantSway : MonoBehaviour
{
    [Header("摆动强度")]
    [Tooltip("最大摆动角度")]
    public float maxAngle = 10f;

    [Tooltip("风力整体倍率")]
    public float windStrength = 1f;

    [Header("风场变化")]
    [Tooltip("风场平滑过渡时间")]
    public float windChangeSpeed = 0.5f;

    [Header("物理感")]
    [Tooltip("越大回拉越快")]
    public float spring = 30f;
    [Tooltip("越大阻力越强")]
    public float damping = 5f;

    /*–––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––*/

    // 当前角速度（本地空间欧拉角）
    private Vector2 velocity;

    // 噪声采样游标
    private float noiseTimeX, noiseTimeY;

    // 目标角度（平滑后的风推力）
    private Vector2 targetAngle;

    void Awake()
    {
        // 随机起点，防止所有灯同步摇摆
        noiseTimeX = Random.Range(0f, 100f);
        noiseTimeY = Random.Range(0f, 100f);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        /* 1. 生成风场目标角度（Perlin 噪声） */
        noiseTimeX += dt * windChangeSpeed;
        noiseTimeY += dt * windChangeSpeed * 1.3f; // 让 XY 不同步

        // 低频大风 + 高频小抖
        float windX =
            Mathf.PerlinNoise(noiseTimeX, 0) * 2f - 1f +
            (Mathf.PerlinNoise(noiseTimeX * 4f, 0.5f) * 2f - 1f) * 0.3f;

        float windY =
            Mathf.PerlinNoise(noiseTimeY, 0) * 2f - 1f +
            (Mathf.PerlinNoise(noiseTimeY * 4f, 0.7f) * 2f - 1f) * 0.3f;

        Vector2 newTarget = new Vector2(windX, windY) * maxAngle * windStrength;

        // 平滑到目标
        targetAngle = Vector2.Lerp(targetAngle, newTarget, dt * 5f);

        /* 2. 弹簧-阻尼系统 */
        Vector2 current = new Vector2(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y);

        // 把角度映射到 -180~180
        if (current.x > 180) current.x -= 360;
        if (current.y > 180) current.y -= 360;

        Vector2 springForce = -spring * (current - targetAngle);
        Vector2 dampingForce = -damping * velocity;

        Vector2 accel = springForce + dampingForce;

        velocity += accel * dt;
        current += velocity * dt;

        /* 3. 应用旋转（Z 轴始终为 0） */
        transform.localEulerAngles = new Vector3(current.x, current.y, 0f);
    }
}