using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMove : MonoBehaviour
{
    public float amplitude = 1.0f; // 振幅
    public float frequency = 10.0f; // 频率
    public Vector3 direction = Vector3.right; // 运动方向

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // 记录初始位置
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency) * amplitude; // 计算偏移量
        transform.position = startPosition + direction.normalized * offset; // 应用正弦运动

    }
}
