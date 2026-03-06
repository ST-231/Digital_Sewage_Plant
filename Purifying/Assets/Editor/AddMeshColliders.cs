using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class AddMeshColliders : MonoBehaviour
{
    [MenuItem("Tools/Add Mesh Colliders to All Models")]
    static void AddMeshCollidersToAllModels()
    {
        // 获取场景中所有的 GameObject
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 检查对象是否有 MeshRenderer 组件
            if (obj.GetComponent<MeshRenderer>() != null)
            {
                // 如果对象没有 MeshCollider，则添加一个
                if (obj.GetComponent<MeshCollider>() == null)
                {
                    obj.AddComponent<MeshCollider>();
                    Debug.Log("Added MeshCollider to: " + obj.name);
                }
            }
        }

        Debug.Log("Mesh Colliders added to all models.");
    }
}