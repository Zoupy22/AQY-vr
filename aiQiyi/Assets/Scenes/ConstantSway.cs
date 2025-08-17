using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantSway : MonoBehaviour
{
    [Header("风参数")]
    public float windStrength = 0.4f;     // 风最大推力
    public float windSpeed = 0.6f;      // 风向变化速度
    [Range(0.90f, 0.999f)]
    public float damper = 0.98f;          // 角速度衰减

    private Rigidbody rb;
    private Vector3 noiseOffset;          // 随机偏移量

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 每盏灯独有的噪声坐标偏移，保证完全随机
        noiseOffset = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
    }

    void FixedUpdate()
    {
        // 用不同噪声坐标采样，得到独立平滑风向
        float nx = Mathf.PerlinNoise(
            noiseOffset.x + Time.time * windSpeed,
            noiseOffset.y
        ) * 2f - 1f;

        float nz = Mathf.PerlinNoise(
            noiseOffset.z + Time.time * windSpeed,
            noiseOffset.x
        ) * 2f - 1f;

        Vector3 wind = new Vector3(nx, 0, nz).normalized * windStrength;

        rb.AddForce(wind, ForceMode.Force);
        rb.angularVelocity *= damper;
    }
}