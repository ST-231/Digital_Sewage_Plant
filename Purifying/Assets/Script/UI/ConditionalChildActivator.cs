using UnityEngine;

public class ConditionalChildActivator : MonoBehaviour
{
    [Header("条件对象配置")]
    public GameObject conditionObject1; // 拖入第一个条件对象
    public GameObject conditionObject2; // 拖入第二个条件对象

    [Header("状态监控")]
    [SerializeField] private bool _currentActivationState; // 显示当前激活状态

    private bool _lastConditionState; // 记录上一帧的条件状态

    private void Update()
    {
        // 空引用检查
        if (conditionObject1 == null || conditionObject2 == null)
        {
            Debug.LogWarning("条件对象未赋值！");
            return;
        }

        // 计算当前条件状态
        bool currentCondition = conditionObject1.activeInHierarchy &&
                              !conditionObject2.activeInHierarchy;

        // 当条件状态变化时执行操作
        if (currentCondition != _lastConditionState)
        {
            UpdateChildrenActivation(currentCondition);
            _lastConditionState = currentCondition;

            // 调试输出
            Debug.Log($"子物体激活状态已切换为：{currentCondition}");
        }
    }

    /// <summary>
    /// 更新所有子物体激活状态
    /// </summary>
    private void UpdateChildrenActivation(bool activate)
    {
        _currentActivationState = activate;

        foreach (Transform child in transform)
        {
            // 跳过被锁定的对象
            if ((child.gameObject.hideFlags & HideFlags.NotEditable) == 0)
            {
                child.gameObject.SetActive(activate);
            }
        }
    }

    /// <summary>
    /// 编辑器状态下的验证
    /// </summary>
    //private void OnValidate()
    //{
    //    if (conditionObject1 == null)
    //        Debug.LogError("请拖入条件对象1", this);

    //    if (conditionObject2 == null)
    //        Debug.LogWarning("建议拖入条件对象2", this);
    //}
}