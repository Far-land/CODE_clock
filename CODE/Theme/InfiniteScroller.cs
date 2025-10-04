using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class InfiniteScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private List<RectTransform> items = new List<RectTransform>();
    private float itemWidthPlusSpacing;
    private float halfContentWidth;

    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
        contentPanel = GetComponent<RectTransform>();

        // 监听滚动事件
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    /// <summary>
    /// 由外部调用，用于在列表填充后进行初始化
    /// </summary>
    public void Initialize()
    {
        items.Clear();
        foreach (Transform child in contentPanel)
        {
            items.Add(child as RectTransform);
        }

        if (items.Count < 2) return; // 至少需要2个才能循环

        var layoutGroup = contentPanel.GetComponent<HorizontalLayoutGroup>();
        itemWidthPlusSpacing = items[0].rect.width + (layoutGroup != null ? layoutGroup.spacing : 0);
        halfContentWidth = (items.Count * itemWidthPlusSpacing) / 2f;
    }

    private void OnScroll(Vector2 position)
    {
        foreach (RectTransform item in items)
        {
            // 将列表项的本地位置转换为相对于Scroll View中心的位置
            float itemPosInViewport = contentPanel.anchoredPosition.x + item.anchoredPosition.x;

            // 如果卡片向左滑出视野太远 (超过一半总长度)
            if (itemPosInViewport < -halfContentWidth)
            {
                // 将它的位置移动到列表的最右边
                item.anchoredPosition += new Vector2(items.Count * itemWidthPlusSpacing, 0);
            }
            // 如果卡片向右滑出视野太远
            else if (itemPosInViewport > halfContentWidth)
            {
                // 将它的位置移动到列表的最左边
                item.anchoredPosition -= new Vector2(items.Count * itemWidthPlusSpacing, 0);
            }
        }
    }
}