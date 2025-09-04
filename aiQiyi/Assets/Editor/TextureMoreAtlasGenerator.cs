using UnityEngine;
using UnityEditor;

public class TextureMoreAtlasGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Grid Texture Atlas")]
    public static void GenerateGridAtlas()
    {
        Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        if (textures == null || textures.Length == 0)
        {
            Debug.LogError("No textures selected! Please select some Texture2D assets.");
            return;
        }

        // 检查所有纹理尺寸是否一致
        int textureWidth = textures[0].width;
        int textureHeight = textures[0].height;
        foreach (Texture2D tex in textures)
        {
            if (tex.width != textureWidth || tex.height != textureHeight)
            {
                Debug.LogError($"Texture {tex.name} has different dimensions. All textures must be {textureWidth}x{textureHeight}.");
                return;
            }
        }

        // 计算网格排列方式（4行×5列）
        int columns = 5; // 每行5张
        int rows = Mathf.CeilToInt(textures.Length / (float)columns); // 计算行数（20/5=4行）

        // 计算图集尺寸
        int atlasWidth = textureWidth * columns;
        int atlasHeight = textureHeight * rows;

        // 检查是否超出 Unity 最大纹理尺寸限制
        int maxTextureSize = SystemInfo.maxTextureSize;
        if (atlasWidth > maxTextureSize || atlasHeight > maxTextureSize)
        {
            Debug.LogError($"Atlas size {atlasWidth}x{atlasHeight} exceeds maximum supported texture size ({maxTextureSize}). Reduce the number of textures or their dimensions.");
            return;
        }

        Debug.Log($"Generated atlas size: {atlasWidth}x{atlasHeight} (Grid: {rows}x{columns})");

        // 创建可读的临时纹理
        Texture2D atlasTexture = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);

        try
        {
            // 填充图集（按网格排列）
            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D texture = textures[i];
                Texture2D readableTexture = MakeTextureReadable(texture); // 确保纹理可读

                // 计算当前纹理在图集中的位置
                int col = i % columns;
                int row = i / columns;
                int startX = col * textureWidth;
                int startY = (rows - 1 - row) * textureHeight; // Y轴从下往上排列

                atlasTexture.SetPixels(startX, startY, textureWidth, textureHeight, readableTexture.GetPixels());
            }

            atlasTexture.Apply();

            // 保存图集
            string path = EditorUtility.SaveFilePanelInProject("Save Texture Atlas", "GridTextureAtlas", "png", "Please specify where to save the atlas:");
            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.WriteAllBytes(path, atlasTexture.EncodeToPNG());
                AssetDatabase.Refresh();
                Debug.Log($"Texture atlas created: {path}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to generate atlas: {e.Message}");
        }
    }

    // 确保纹理可读（临时复制一份）
    private static Texture2D MakeTextureReadable(Texture2D texture)
    {
        if (texture.isReadable) return texture;

        // 如果纹理不可读，创建一个临时的可读副本
        RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(texture, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;
        Texture2D readableTexture = new Texture2D(texture.width, texture.height);
        readableTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTexture.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);
        return readableTexture;
    }
}
