using UnityEngine;
using UnityEditor;

public class AddMeshColliderEditor : EditorWindow
{
    [MenuItem("工具/添加 MeshCollider")]
    public static void AddMeshColliders()
    {
        GameObject sewagePlant = GameObject.Find("污水厂布局");

        if (sewagePlant == null)
        {
            Debug.LogWarning("找不到名为 '污水厂布局' 的对象！");
            return;
        }

        int count = AddMeshCollidersRecursive(sewagePlant.transform);
        Debug.Log($"已添加 {count} 个 MeshCollider！");
    }

    private static int AddMeshCollidersRecursive(Transform parent)
    {
        int count = 0;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) // 遍历所有子层级
        {
            if (child.GetComponent<MeshCollider>() == null && child.GetComponent<MeshFilter>() != null)
            {
                child.gameObject.AddComponent<MeshCollider>();
                count++;
            }
        }

        return count;
    }
}
