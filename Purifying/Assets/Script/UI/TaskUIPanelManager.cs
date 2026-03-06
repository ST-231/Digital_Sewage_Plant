using UnityEngine;

public class TaskUIPanelManager : MonoBehaviour
{
    // 在 Inspector 中设置任务面板数组，下标与 globalSequence 对应
    public GameObject[] taskPanels;

    /// <summary>
    /// 更新任务面板显示：只有当前 globalSequence 对应的面板显示，其它面板隐藏
    /// </summary>
    public void UpdatePanels(int currentSequence)
    {
        for (int i = 0; i < taskPanels.Length; i++)
        {
            if (taskPanels[i] != null)
            {
                taskPanels[i].SetActive(i == currentSequence);
            }
        }
    }
}
