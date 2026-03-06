using UnityEngine;

public class SequentialEManager : MonoBehaviour
{
    public static SequentialEManager Instance { get; private set; }

    // 当前允许解锁的顺序索引，初始值设为 0
    public int currentSequence = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
