using UnityEngine;

public class TaskbarController : MonoBehaviour
{
    public GameObject targetObject; // 需要隐藏/显示的对象
    public GameObject[] targets; // 监视的对象列表

    void Update()
    {
        bool anyActive = false;

        foreach (GameObject obj in targets)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                anyActive = true;
                break;
            }
        }

        // 只隐藏/显示目标对象，而不是自己
        if (targetObject != null)
        {
            targetObject.SetActive(!anyActive);
        }
    }
}
