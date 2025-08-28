using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightogclick : fogclick
{
    public Light targetLight; // 目标灯光
    public Material targetMaterial; // 目标材质，用于设置自发光颜色

    public ColorPalette colorPalette;
    public float transitionDuration = 1.0f; // 渐变持续时间

    private int currentIndex = 2; // 当前颜色索引
    private int nextIndex = 1; // 下一个颜色索引
    private float transitionProgress = 0f; // 渐变进度标记
    private bool isTransitioning = false; // 标记是否正在过渡
    void Start()
    {
        // 设置灯光初始颜色为第一个颜色
        // 如果引用的颜色表为空或数量为 0，警告用户
        if (colorPalette == null || colorPalette.colors.Length == 0)
        {
            Debug.LogWarning("Color Palette is not assigned or empty!");
            return;
        }
    }
    public override void Press()
    {
        if (!isTransitioning && colorPalette.colors.Length > 0)
        {
            // 获取下一个颜色索引
            nextIndex = (currentIndex + 1) % colorPalette.colors.Length;
            isTransitioning = true;
        }
    }
    void Update()
    {
        if (isTransitioning && colorPalette.colors.Length > 0)
        {
            transitionProgress += Time.deltaTime / transitionDuration;

            // 渐变灯光颜色
            targetLight.color = Color.Lerp(colorPalette.colors[currentIndex], colorPalette.colors[nextIndex], transitionProgress);
            targetLight.intensity = Mathf.Lerp(colorPalette.lightIntensities[currentIndex], colorPalette.lightIntensities[nextIndex], transitionProgress);

            // 材质的自发光颜色渐变
            if (targetMaterial != null)
            {
                Color emissionColor = Color.Lerp(colorPalette.emissionColors[currentIndex], colorPalette.emissionColors[nextIndex], transitionProgress);
                targetMaterial.SetColor("_EmissionColor", emissionColor);
            }
            if (transitionProgress >= 1f)
            {
                isTransitioning = false;
                transitionProgress = 0f;
                currentIndex = nextIndex; // 更新当前颜色索引
            }
        }
    }

}
