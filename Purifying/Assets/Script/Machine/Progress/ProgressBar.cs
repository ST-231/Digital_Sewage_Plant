using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider progressBar;  // 绑定进度条Slider
    public float currentProgress = 0; // 当前进度值

    public float smoothTime = 1.5f; // 过渡时间
    private float velocity = 0;

    float targetProgress = 0;

    void Update()
    {
        //WaterData.Instance.SetProgress(targetProgress);
        // 计算目标值（替换为你的逻辑）
        targetProgress = WaterData.Instance.GetProgress();

        // 平滑过渡
        currentProgress = Mathf.SmoothDamp(
            currentProgress,
            targetProgress,
            ref velocity,
            smoothTime
        );
        progressBar.value = currentProgress;
    }
}
