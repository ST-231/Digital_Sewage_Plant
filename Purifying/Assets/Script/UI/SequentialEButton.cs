using UnityEngine;
using System.Collections;

public class SequentialEButton : MonoBehaviour
{
    // 交互按钮的顺序索引，例如 A 为 0，B 为 1，C 为 2
    public int sequenceIndex = 0;
    // 可选：按钮对应要显示的界面或执行的其他逻辑
    public GameObject interactionPanel;
    // 交互失败时显示的面板（提示语），持续 5 秒
    public GameObject failPrompt;

    /// <summary>
    /// 当玩家按下 E 触发交互时调用此方法
    /// </summary>
    public void TryInteract()
    {
        var seqManager = SequentialEManager.Instance;
        // 获取交互的摄像机切换组件（假设用于进入/退出界面）
        CameraSwitchOnProximity csp = GetComponent<CameraSwitchOnProximity>();
        if (csp == null)
        {
            Debug.LogWarning("找不到 CameraSwitchOnProximity 组件");
            return;
        }

        // 如果设备已经解锁，允许重复交互（不改变全局顺序）
        if (seqManager.currentSequence > sequenceIndex)
        {
            csp.onSwitch();
            Debug.Log($"交互成功 (重复交互): {gameObject.name}");
        }
        // 如果当前设备正处于解锁顺序上（globalSequence 等于设备序号），则执行交互并推进全局顺序
        else if (seqManager.currentSequence == sequenceIndex)
        {
            csp.onSwitch();
            Debug.Log($"交互成功 (解锁新设备): {gameObject.name}");
            seqManager.currentSequence++; // 更新全局顺序，解锁下一个设备

            // 更新任务面板显示
            TaskUIPanelManager taskUI = FindObjectOfType<TaskUIPanelManager>();
            if (taskUI != null)
            {
                taskUI.UpdatePanels(seqManager.currentSequence);
            }
        }
        // 如果设备未解锁，则显示提示
        else
        {
            Debug.Log("请按任务流程执行");
            if (failPrompt != null)
            {
                failPrompt.SetActive(true);
                StartCoroutine(HideFailPrompt());
            }
        }
    }

    private IEnumerator HideFailPrompt()
    {
        yield return new WaitForSeconds(5f);
        failPrompt.SetActive(false);
    }
}
