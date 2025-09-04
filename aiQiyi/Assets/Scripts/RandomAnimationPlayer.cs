using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RandomAnimationPlayer : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2f;
    public float hideAtX = 10f;
    public Vector2 respawnTime = new Vector2(1f, 20f);

    private Vector3 startPos;
    private MeshRenderer mesh;
    private float timer;
    private float nextRespawnTime;
    private bool isVisible; // 新增：可见状态标志

    void Start()
    {
        startPos = transform.position;
        mesh = GetComponent<MeshRenderer>();
        isVisible = mesh.enabled; // 初始化状态
        ResetTimer();
    }

    void Update()
    {
        if (isVisible)
        {
            MoveObject();
            CheckHideCondition();
        }
        else
        {
            UpdateRespawnTimer();
        }
    }

    void MoveObject()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void CheckHideCondition()
    {
        if (transform.position.x >= hideAtX)
        {
            Hide();
        }
    }

    void UpdateRespawnTimer()
    {
        timer += Time.deltaTime;
        if (timer >= nextRespawnTime)
        {
            Respawn();
        }
    }

    void Hide()
    {
        isVisible = false;
        mesh.enabled = false;
        timer = 0f;
        ResetTimer();
    }

    void Respawn()
    {
        transform.position = startPos;
        isVisible = true;
        mesh.enabled = true;
    }

    void ResetTimer() => nextRespawnTime = Random.Range(respawnTime.x, respawnTime.y);

    public void Restart() => Respawn();
    // 物体被禁用时自动调用
    void OnDisable()
    {
        Respawn();
    }
}
