using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Texture Palette", menuName = "Texture Palette")]
public class TexturePalette : ScriptableObject
{
    public Texture[] textures; // 一个贴图数组，存储所有可切换的贴图
}