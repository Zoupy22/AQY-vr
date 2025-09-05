using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class cardfogclick : fogclick
{
    private static List<cardfogclick> cardFogClicks = new List<cardfogclick>();
    public Texture texturePalette;
    private float transitionDuration = 0.3f; // 渐变持续时间（如果有动画切换）
    public bool isVerticalLayout = false; // 是否垂直排列（默认水平排列）
    public int rowrandom = 1; // 垂直排列时的行数（默认1行）

    private int currentIndex = 0; // 当前贴图索引
    private int nextIndex = 1; // 下一个贴图索引
    private float transitionProgress = 0f; // 渐变进度
    private bool isTransitioning = false; // 标记是否正在过渡

    private Material targetMaterial; // 目标材质
    private string texturePropertyName = "_BaseMap"; // 材质的贴图属性名 (默认是 _MainTex)
    private string textureIndexPropertyName = "_Index"; // 材质的贴图属性名 (默认是 _MainTex)
    private string textureyIndexPropertyName = "_yIndex"; // 材质的贴图属性名 (默认是 _MainTex)
    public int totalIndex = 5;
    void Awake()
    {
        // 将当前实例添加到静态列表中
        cardFogClicks.Add(this);
    }

    void Start()
    {
        if (texturePalette == null)
        {
            Debug.LogWarning("Texture Palette is not assigned or empty!");
            return;
        }
        // 从当前 GameObject 的 MeshRenderer 上动态获取材质
        MeshRenderer renderer = transform.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogError("MeshRenderer not found on the GameObject!");
            return;
        }

        // 获取材质的实例
        targetMaterial = renderer.material; // 注意，这里会生成材质实例
        targetMaterial.SetTexture(texturePropertyName, texturePalette);
    }

    void Update()
    {
        // 如果有渐变过程
        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;

            // 检查过渡是否完成
            if (transitionProgress >= 1f)
            {
                isTransitioning = false;
                transitionProgress = 0f;
                currentIndex = nextIndex; 
                targetMaterial.SetInt(textureIndexPropertyName, currentIndex);
                // 初始化 UV 偏移
                UpdateTextureOffset(currentIndex);
            }
        }
    }
    // 更新 UV 偏移（支持水平和垂直排列）
    private void UpdateTextureOffset(int index)
    {
        if (texturePalette == null) return;

        if (isVerticalLayout)
        {
            int columnrandom = Random.Range(0, rowrandom);

            targetMaterial.SetInt(textureIndexPropertyName, index);
            targetMaterial.SetInt(textureyIndexPropertyName, columnrandom);
            Debug.LogError("zhangtao-----" +columnrandom+"--"+ index);
        }
        else
        {
            targetMaterial.SetInt(textureIndexPropertyName, index);
        }
    }
    public override void Press()
    {
        // 遍历并对所有的 cardfogclick 实例调用其 PressOnce 方法
        foreach (cardfogclick cardFogClick in cardFogClicks)
        {
            cardFogClick.PressOnce();
        }
    }
    private void PressOnce()
    {
        // 如果当前没有正在切换的过程
        if (!isTransitioning)
        {
            nextIndex = (currentIndex + 1) % totalIndex;

            isTransitioning = true;
            Debug.Log($"[{gameObject.name}] Switching to texture: {nextIndex}");
        }
    }
}
