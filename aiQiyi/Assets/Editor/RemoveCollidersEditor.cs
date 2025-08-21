using UnityEngine;
using UnityEditor;

public class RemoveCollidersEditor : EditorWindow
{
    [MenuItem("Tools/Remove All Colliders")]
    public static void RemoveAllColliders()
    {
        // 获取场景中所有的 GameObjects
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        int colliderCount = 0;

        // 遍历每个对象，找到所有的 Collider 并移除
        foreach (GameObject obj in allGameObjects)
        {
            // 获取该对象上的所有 Collider
            Collider[] colliders = obj.GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                // 删除碰撞体
                Undo.DestroyObjectImmediate(collider);
                colliderCount++;
                Debug.Log($"Removed Collider from GameObject: {obj.name}");
            }
        }

        // 输出结果
        Debug.Log($"Successfully removed {colliderCount} Colliders from the scene.");
    }

    [MenuItem("Tools/Remove All 2D Colliders")]
    public static void RemoveAll2DColliders()
    {
        // 获取场景中所有的 GameObjects
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        int colliderCount = 0;

        // 遍历每个对象，找到所有的 2D Collider 并移除
        foreach (GameObject obj in allGameObjects)
        {
            // 获取该对象上的所有 2D Collider
            Collider2D[] colliders2D = obj.GetComponents<Collider2D>();

            foreach (Collider2D collider in colliders2D)
            {
                // 删除碰撞体
                Undo.DestroyObjectImmediate(collider);
                colliderCount++;
                Debug.Log($"Removed 2D Collider from GameObject: {obj.name}");
            }
        }

        // 输出结果
        Debug.Log($"Successfully removed {colliderCount} 2D Colliders from the scene.");
    }
}
