using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelHoverEffect : MonoBehaviour
{
    public RectTransform[] panels; // 拖入三个面板
    public string[] sceneNames; // 对应的场景名称
    public float hoverScaleFactor = 1.2f; // 悬停时的放大比例
    public float fadeAmount = 0.5f; // 其他面板变暗程度
    private Vector3[] originalScales; // 记录原始缩放比例
    private int currentHovered = -1;

    void Start()
    {
        // 记录面板的原始缩放比例
        originalScales = new Vector3[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            originalScales[i] = panels[i].localScale;
        }
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        int newHovered = -1;

        for (int i = 0; i < panels.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(panels[i], mousePosition))
            {
                newHovered = i;
                break;
            }
        }

        if (newHovered != currentHovered)
        {
            currentHovered = newHovered;
            ApplyHoverEffect();
        }

        if (currentHovered != -1 && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(sceneNames[currentHovered]);
        }
    }

    void ApplyHoverEffect()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == currentHovered)
            {
                panels[i].localScale = originalScales[i] * hoverScaleFactor; // 放大
                SetPanelAlpha(panels[i], 1f);
            }
            else
            {
                panels[i].localScale = originalScales[i]; // 还原原始比例
                SetPanelAlpha(panels[i], fadeAmount);
            }
        }
    }

    void SetPanelAlpha(RectTransform panel, float alpha)
    {
        Image img = panel.GetComponent<Image>();
        if (img != null)
        {
            Color color = img.color;
            img.color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}
