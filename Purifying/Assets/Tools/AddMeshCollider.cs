using UnityEngine;

public class AddMeshCollider : MonoBehaviour
{
    void Start()
    {
        // 查找名为“污水厂布局”的对象
        GameObject sewagePlant = GameObject.Find("污水厂布局");

        if (sewagePlant != null)
        {
            // 遍历所有子对象
            foreach (Transform child in sewagePlant.transform)
            {
                // 检查子对象是否已有 MeshCollider，没有则添加
                if (child.GetComponent<MeshCollider>() == null)
                {
                    child.gameObject.AddComponent<MeshCollider>();
                    Debug.Log($"已添加 MeshCollider: {child.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("找不到名为 '污水厂布局' 的对象！");
        }
    }
}
