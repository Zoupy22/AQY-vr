using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantSway : MonoBehaviour
{
    [Header("�����")]
    public float windStrength = 0.4f;     // ���������
    public float windSpeed = 0.6f;      // ����仯�ٶ�
    [Range(0.90f, 0.999f)]
    public float damper = 0.98f;          // ���ٶ�˥��

    private Rigidbody rb;
    private Vector3 noiseOffset;          // ���ƫ����

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // ÿյ�ƶ��е���������ƫ�ƣ���֤��ȫ���
        noiseOffset = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
    }

    void FixedUpdate()
    {
        // �ò�ͬ��������������õ�����ƽ������
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