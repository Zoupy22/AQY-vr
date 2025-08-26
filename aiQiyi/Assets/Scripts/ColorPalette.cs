using UnityEngine;

[CreateAssetMenu(fileName = "New Color Palette", menuName = "Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color[] colors; // 存储颜色的数组
}
