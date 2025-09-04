using UnityEngine;

[CreateAssetMenu(fileName = "New Color Palette", menuName = "Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color[] colors; // 存储颜色的数组
    public float[] lightIntensities; // 灯光强度数组，与颜色数组一一对应
    public Color[] emissionColors;   // 自发光颜色数组，与颜色数组一一对应
    public Color[] fogColors;   // 自发光颜色数组，与颜色数组一一对应
    public Color[] blankColors;   // 自发光颜色数组，与颜色数组一一对应
}
