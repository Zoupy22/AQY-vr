using UnityEngine;
using UnityEditor;
using System; // 新增 using 语句
using System.Linq; // 用于 OrderBy 排序

public class TextureAtlasGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Horizontal Texture Atlas")]
    public static void GenerateHorizontalAtlas()
    {
        // 获取所有选中的 Texture2D
        Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets) as Texture2D[];
        if (textures == null || textures.Length == 0)
        {
            Debug.LogError("No textures selected! Please select some Texture2D assets.");
            return;
        }
        textures = textures.OrderBy(t => t.name).ToArray();
        // 假设所有图片都是 1024x2048 的大小，因此无需动态调整大小
        int textureWidth = textures[0].width;  // 每张图片的宽度 (假设所有图片宽度一致)
        int textureHeight = textures[0].height; // 每张图片的高度 (假设所有图片高度一致)

        // 动态计算图集的宽度和高度
        int atlasWidth = textureWidth * textures.Length; // 横向排列总宽度
        int atlasHeight = textureHeight; // 高度保持单张图片高度一致

        Debug.Log($"Generated atlas size: {atlasWidth}x{atlasHeight}");

        // 创建纹理图集
        Texture2D atlasTexture = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);

        // 填充纹理图集
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D texture = textures[i];
            // 确保纹理是可读的
            if (!texture.isReadable)
            {
                Debug.LogError($"Texture {texture.name} is not readable. Please enable Read/Write in the texture import settings.");
                return;
            }

            // 计算起始位置：按横向排列
            int startX = i * textureWidth; // 每张图片横向偏移
            int startY = 0; // Y 起点始终为 0，因为是一排横向排列

            // 拷贝当前纹理像素到图集中
            atlasTexture.SetPixels(startX, startY, textureWidth, textureHeight, texture.GetPixels());
        }

        // 应用变化
        atlasTexture.Apply();

        // 保存纹理图集
        string path = EditorUtility.SaveFilePanelInProject("Save Texture Atlas", "HorizontalTextureAtlas", "png", "Please specify where to save the atlas:");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllBytes(path, atlasTexture.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log($"Texture atlas created: {path}");
        }
    }

    [MenuItem("Tools/Generate Vertical Texture Atlas")]
    public static void GenerateVerticalAtlas()
    {
        GenerateAtlas(true); // 调用通用方法，垂直排列
    }
    private static void GenerateAtlas(bool isVertical)
    {
        // 获取并排序纹理
        Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        if (textures == null || textures.Length == 0)
        {
            Debug.LogError("No textures selected!");
            return;
        }
        textures = textures.OrderBy(t => t.name).ToArray();

        // 基础尺寸检查
        int textureWidth = textures[0].width;
        int textureHeight = textures[0].height;

        // 计算图集尺寸
        int atlasWidth = isVertical ? textureWidth : textureWidth * textures.Length;
        int atlasHeight = isVertical ? textureHeight * textures.Length : textureHeight;

        // 检查尺寸限制
        if (atlasWidth > SystemInfo.maxTextureSize || atlasHeight > SystemInfo.maxTextureSize)
        {
            Debug.LogError($"Atlas size {atlasWidth}x{atlasHeight} exceeds maximum texture size!");
            return;
        }

        // 创建图集
        Texture2D atlasTexture = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);

        // 填充图集
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D texture = textures[i];
            if (!texture.isReadable)
            {
                Debug.LogError($"Texture {texture.name} is not readable!");
                return;
            }

            // 计算位置
            int startX = isVertical ? 0 : i * textureWidth;
            int startY = isVertical ? (textures.Length - 1 - i) * textureHeight : 0;

            atlasTexture.SetPixels(startX, startY, textureWidth, textureHeight, texture.GetPixels());
        }

        atlasTexture.Apply();

        // 保存文件
        string typeName = isVertical ? "Vertical" : "Horizontal";
        string path = EditorUtility.SaveFilePanelInProject(
            $"Save {typeName} Texture Atlas",
            $"{typeName}TextureAtlas",
            "png",
            "Please specify save location");

        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllBytes(path, atlasTexture.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log($"{typeName} atlas created: {path}");
        }
    }
}
