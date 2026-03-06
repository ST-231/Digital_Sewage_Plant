using UnityEngine;
using UnityEngine.UI;

public class MapIconTeleport : MonoBehaviour
{
    [Header("跳转目标设置")]
    public Vector3 teleportTarget; // 目标位置
    public Quaternion targetRotation; // 目标旋转角度
    public GameObject player; // 玩家对象
    public CameraSwitchOnProximity camswitch;

    public void OnIconClicked()
    {
        Debug.Log("地图图标点击，尝试传送玩家...");
        
        if (player != null)
        {
            /*
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.SetPositionAndRotation(teleportTarget, targetRotation);
                camswitch.canPressE = false;
                cc.enabled = true;
            }
            else
            {
                player.transform.SetPositionAndRotation(teleportTarget, targetRotation);
                camswitch.canPressE = false;
            }
            Debug.Log("传送成功，新坐标：" + teleportTarget + "，新朝向：" + targetRotation.eulerAngles);
            */

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 停止所有物理运动
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // 保持运动学状态（如果原本是运动学）
                bool wasKinematic = rb.isKinematic;
                rb.isKinematic = true;

                // 执行传送
                player.transform.SetPositionAndRotation(teleportTarget, targetRotation);

                // 强制同步刚体位置
                rb.position = teleportTarget;
                rb.rotation = targetRotation;

                // 恢复运动学状态
                rb.isKinematic = wasKinematic;
            }
            else
            {
                player.transform.SetPositionAndRotation(teleportTarget, targetRotation);
            }

            camswitch.canPressE = false;
            Debug.Log("传送成功，新坐标：" + teleportTarget + "，新朝向：" + targetRotation.eulerAngles);
        }


        else
        {
            Debug.LogWarning("未设置玩家对象！");
        }
    }
}
