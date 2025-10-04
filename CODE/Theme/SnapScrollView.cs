using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ScrollRect))]
public class SnapScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("吸附设置")]
    [Tooltip("卡片吸附到中心的速度")]
    public float snapSpeed = 10f;
    [Tooltip("卡片之间的间距，必须和HorizontalLayoutGroup中的Spacing一致")]
    public float itemSpacing = 20f;
    [Tooltip("左右两边的额外边距，用于让第一张和最后一张能居中")]
    public float padding = 100f;

    // 当中心项发生变化时，触发此事件并传出中心项的索引
    public Action<int> OnCenterItemChanged;

    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private List<RectTransform> items = new List<RectTransform>();
    private bool isDragging = false;
    private int currentCenterIndex = -1;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentPanel = scrollRect.content;
    }

    /// <summary>
    /// 由外部调用，用于初始化列表
    /// </summary>
    public void Initialize(int itemCount)
    {
        items.Clear();
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            items.Add(contentPanel.GetChild(i) as RectTransform);
        }

        if (items.Count == 0) return;

        // 设置左右边距，确保第一张和最后一张能居中
        var layoutGroup = contentPanel.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            float panelWidth = GetComponent<RectTransform>().rect.width;
            float itemWidth = items[0].rect.width;
            padding = (panelWidth - itemWidth) / 2f;
            layoutGroup.padding.left = Mathf.RoundToInt(padding);
            layoutGroup.padding.right = Mathf.RoundToInt(padding);
        }

        // 立即检查一次中心项
        FindNewCenterItem();
    }

    void Update()
    {
        // 只有在用户没有拖拽时，才进行自动吸附
        if (!isDragging && items.Count > 0)
        {
            float targetX = -items[currentCenterIndex].anchoredPosition.x;
            float newX = Mathf.Lerp(contentPanel.anchoredPosition.x, targetX, Time.deltaTime * snapSpeed);
            contentPanel.anchoredPosition = new Vector2(newX, contentPanel.anchoredPosition.y);
        }

        // --- 无限循环逻辑 ---
        if (items.Count > 1) // 至少需要2个以上才能循环
        {
            for (int i = 0; i < items.Count; i++)
            {
                float itemPosInViewport = transform.InverseTransformPoint(items[i].position).x;
                float contentWidth = contentPanel.rect.width;

                // 如果卡片向左滑出视野太多
                if (itemPosInViewport < -contentWidth / 2f)
                {
                    items[i].SetAsLastSibling();
                }
                // 如果卡片向右滑出视野太多
                else if (itemPosInViewport > contentWidth / 2f)
                {
                    items[i].SetAsFirstSibling();
                }
            }
        }
    }

    // 当开始拖拽时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // 当结束拖拽时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        FindNewCenterItem();
    }

    /// <summary>
    /// 查找距离中心最近的项
    /// </summary>
    private void FindNewCenterItem()
    {
        float minDistance = float.MaxValue;
        int newCenterIndex = 0;

        for (int i = 0; i < items.Count; i++)
        {
            // 计算每个项相对于屏幕中心的距离
            float distance = Mathf.Abs(transform.InverseTransformPoint(items[i].position).x);
            if (distance < minDistance)
            {
                minDistance = distance;
                newCenterIndex = i;
            }
        }

        if (newCenterIndex != currentCenterIndex)
        {
            currentCenterIndex = newCenterIndex;
            // 触发事件，通知外部“中心项变了！”
            OnCenterItemChanged?.Invoke(currentCenterIndex);
        }
    }
}