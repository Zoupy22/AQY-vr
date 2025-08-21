using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic; // 用于缓存已创建材质

public class MaterialReplacerURPLit : EditorWindow
{
    public Material shaderGraphMaterialOpaque;  // 用于 Opaque 模式
    public Material shaderGraphMaterialTransparent; // 用于 Transparent 模式
    public string targetFolder = "Assets/Materials/MyNewMaterials/";

    // 缓存已创建的材质
    private Dictionary<string, Material> createdMaterialsCache = new Dictionary<string, Material>();

    [MenuItem("Tools/Replace Materials with URP ShaderGraph")]
    public static void ShowWindow()
    {
        GetWindow<MaterialReplacerURPLit>("Replace URP Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("场景材质替换工具 (仅支持 URP Lit)", EditorStyles.boldLabel);

        shaderGraphMaterialOpaque = (Material)EditorGUILayout.ObjectField(
            "Opaque ShaderGraph Material", shaderGraphMaterialOpaque, typeof(Material), false);

        shaderGraphMaterialTransparent = (Material)EditorGUILayout.ObjectField(
            "Transparent ShaderGraph Material", shaderGraphMaterialTransparent, typeof(Material), false);


        targetFolder = EditorGUILayout.TextField("输出路径", targetFolder);

        if (GUILayout.Button("替换 URP Lit 材质"))
        {
            if (shaderGraphMaterialOpaque == null || shaderGraphMaterialTransparent == null)
            {
                Debug.LogError("[错误] 请指定两个替换材质！（Opaque 和 Transparent）");
                return;
            }

            if (string.IsNullOrEmpty(targetFolder))
            {
                Debug.LogError("[错误] 请指定材质输出路径！");
                return;
            }

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                Debug.Log($"[信息] 未找到目录，已创建目标路径：{targetFolder}");
            }

            ReplaceURPLitMaterials();
        }
    }

    private void ReplaceURPLitMaterials()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        int replacedCount = 0;
        createdMaterialsCache.Clear(); // 清空缓存，避免影响其他操作

        foreach (Renderer renderer in renderers)
        {
            Material[] originalMaterials = renderer.sharedMaterials;
            Material[] newMaterials = new Material[originalMaterials.Length];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                Material originalMaterial = originalMaterials[i];

                if (originalMaterial == null)
                {
                    newMaterials[i] = null; // 保持原材质为空
                    continue;
                }

                // 检查是否是 URP Lit 材质
                if (originalMaterial.shader.name == "Universal Render Pipeline/Lit")
                {
                    bool isTransparent = IsTransparent(originalMaterial);
                    Material baseShaderGraphMaterial = isTransparent ? shaderGraphMaterialTransparent : shaderGraphMaterialOpaque;

                    string materialName = originalMaterial.name;
                    string newMaterialPath = $"{targetFolder}/{materialName}.mat";

                    // 如果已在缓存中，则直接复用
                    if (createdMaterialsCache.ContainsKey(materialName))
                    {
                        newMaterials[i] = createdMaterialsCache[materialName];
                        Debug.Log($"[缓存] 复用已创建的材质：{materialName}");
                        continue;
                    }

                    // 如果材质文件已存在，加载并添加到缓存
                    if (File.Exists(newMaterialPath))
                    {
                        Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(newMaterialPath);
                        if (existingMaterial != null)
                        {
                            createdMaterialsCache[materialName] = existingMaterial;
                            newMaterials[i] = existingMaterial;
                            Debug.Log($"[加载] 复用已存在的材质：{newMaterialPath}");
                            continue;
                        }
                    }

                    Material newMaterial = new Material(baseShaderGraphMaterial);
                    AssetDatabase.CreateAsset(newMaterial, newMaterialPath);

                    // 确保新材质加载成功
                    newMaterial = AssetDatabase.LoadAssetAtPath<Material>(newMaterialPath);
                    if (newMaterial == null)
                    {
                        Debug.LogError($"[错误] 无法加载新材质：{newMaterialPath}");
                        newMaterials[i] = originalMaterial; // 继续保留原始材质
                        continue;
                    }

                    // 迁移材质属性
                    CopyMaterialProperties(originalMaterial, newMaterial);

                    // 添加到缓存并替换材质
                    createdMaterialsCache[materialName] = newMaterial;
                    newMaterials[i] = newMaterial;
                    replacedCount++;
                }
                else
                {
                    newMaterials[i] = originalMaterial; // 保留原始材质
                }
            }

            // 将替换后的材质应用回 Renderer
            renderer.sharedMaterials = newMaterials;
        }

        // 更新并保存新创建的资产
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"替换完成，共替换了 {replacedCount} 个 URP Lit 材质！");
    }
    private bool IsTransparent(Material material)
    {
        // 检查 Surface Type 属性
        if (material.HasProperty("_Surface"))
        {
            Debug.Log($"IsTransparent 1！"+ material.name+ material.GetFloat("_Surface"));
            // 0 = Opaque, 1 = Transparent
            return material.GetFloat("_Surface") == 1.0f;
        }

        // 检查 Render Queue（备用）
        if (material.renderQueue >= 3000)
        {
            Debug.Log($"IsTransparent 2！" + material.name);
            return true; // Transparent 队列值大于等于 3000
        }
        Debug.Log(material.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT")+$"IsTransparent 3！" + material.name);
        // 检查透明相关关键字
        if (material.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT"))
        {
            return true;
        }

        return false; // 默认返回 Opaque
    }
    private void CopyMaterialProperties(Material originalMaterial, Material newMaterial)
    {
        // 基础颜色和主贴图
        if (originalMaterial.HasProperty("_BaseMap") && newMaterial.HasProperty("_BaseMap"))
        {
            newMaterial.SetTexture("_BaseMap", originalMaterial.GetTexture("_BaseMap"));
        }
        if (originalMaterial.HasProperty("_BaseColor") && newMaterial.HasProperty("_BaseColor"))
        {
            newMaterial.SetColor("_BaseColor", originalMaterial.GetColor("_BaseColor"));
        }

        // 法线贴图
        if (originalMaterial.HasProperty("_BumpMap") && newMaterial.HasProperty("_BumpMap"))
        {
            newMaterial.SetTexture("_BumpMap", originalMaterial.GetTexture("_BumpMap"));
        }

        // 金属度贴图和光滑度
        if (originalMaterial.HasProperty("_MetallicGlossMap") && newMaterial.HasProperty("_MetallicGlossMap"))
        {
            newMaterial.SetTexture("_MetallicGlossMap", originalMaterial.GetTexture("_MetallicGlossMap"));
        }
        if (originalMaterial.HasProperty("_Smoothness") && newMaterial.HasProperty("_Smoothness"))
        {
            newMaterial.SetFloat("_Smoothness", originalMaterial.GetFloat("_Smoothness"));
        }

        // 高度贴图
        if (originalMaterial.HasProperty("_ParallaxMap") && newMaterial.HasProperty("_ParallaxMap"))
        {
            newMaterial.SetTexture("_ParallaxMap", originalMaterial.GetTexture("_ParallaxMap"));
        }

        // 环境遮蔽贴图
        if (originalMaterial.HasProperty("_OcclusionMap") && newMaterial.HasProperty("_OcclusionMap"))
        {
            newMaterial.SetTexture("_OcclusionMap", originalMaterial.GetTexture("_OcclusionMap"));
        }
        // 发光关键字和贴图
        if (originalMaterial.IsKeywordEnabled("_EMISSION"))
        {
            newMaterial.SetFloat("_EMISSION_ON", 1.0f);
        }
        else
        {
            newMaterial.SetFloat("_EMISSION_ON", 0.0f);
        }

        if (originalMaterial.HasProperty("_EmissionMap") && newMaterial.HasProperty("_EmissionMap"))
        {
            newMaterial.SetTexture("_EmissionMap", originalMaterial.GetTexture("_EmissionMap"));
        }

        if (originalMaterial.HasProperty("_EmissionColor") && newMaterial.HasProperty("_EmissionColor"))
        {
            newMaterial.SetColor("_EmissionColor", originalMaterial.GetColor("_EmissionColor"));
        }
    }
}
