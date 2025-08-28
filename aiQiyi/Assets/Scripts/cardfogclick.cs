using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class cardfogclick : fogclick
{

    public Texture texturePalette;
    public float transitionDuration = 1.0f; // 渐变持续时间（如果有动画切换）

    private int currentIndex = 0; // 当前贴图索引
    private int nextIndex = 1; // 下一个贴图索引
    private float transitionProgress = 0f; // 渐变进度
    private bool isTransitioning = false; // 标记是否正在过渡

    private Material targetMaterial; // 目标材质
    private string texturePropertyName = "_BaseMap"; // 材质的贴图属性名 (默认是 _MainTex)
    private string textureIndexPropertyName = "_Index"; // 材质的贴图属性名 (默认是 _MainTex)
    private int totalIndex = 5;
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
            }
        }
    }

    public override void Press()
    {
        // 如果当前没有正在切换的过程
        if (!isTransitioning)
        {
            nextIndex = (currentIndex + 1) % totalIndex; 

            isTransitioning = true; 
            Debug.Log($"Switching to texture: {nextIndex}");
        }
    }
}
